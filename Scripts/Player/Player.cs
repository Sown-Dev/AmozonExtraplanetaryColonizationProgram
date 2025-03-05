using System;
using System.Collections;
using System.Linq;
using NewRunMenu;
using Systems;
using Systems.Block;
using Systems.Items;
using Systems.Round;
using TMPro;
using UI;
using UI.BlockUI;
using Unity.Mathematics;
using UnityEngine;

public partial class Player : Unit, IContainer{
    public static Player Instance;

    [SerializeField] public Cursor myCursor;

    [SerializeField] public SpriteRenderer buildingPreview;
    [SerializeField] private SlotVisualizer handVisualizer;
    [SerializeField] private Camera myCam;

    [SerializeField] private GameObject OnDeath;
    [SerializeField] private PopupListUI popupList;

    public Character myCharacter;

    [HideInInspector] public Material myMat;

    public Slot SelectedSlot;

    protected override void Awake(){
        myCharacter = GameManager.Instance?.selectedChar ?? myCharacter;
        //add upgrades
        foreach (UpgradeSO u in myCharacter.startingUpgrades){
            AddUpgrade((Upgrade)u.u, false);
        }

        base.Awake();
        Instance = this;
        myCursor.OnLeftClick.AddListener(ClickPos);
        myCursor.OnRightClick.AddListener(RightClickPos);
        myCursor.OnCTRLClick.AddListener(CTRLClickPos);
        SelectSlot(Inventory.GetSlot(7));
        //Inventory.AddOnInsert(OnInsert); //shit doesnt work bruh


        myMat = sr.material;
        myMat.SetColor("_ReplaceColor1", myCharacter.shirtColor);
        myMat.SetColor("_ReplaceColor2", myCharacter.pantsColor);
        myMat.SetColor("_ReplaceColor3", myCharacter.skinColor);
    }

    public bool Insert(ref ItemStack s, bool popup = true){
        if (Inventory.Insert(ref s, true)){
            //RoundManager.Instance.myStats.itemsPickedUp += s.amount;
            if (popup){
                Popup($"+{s.amount} {s.item.name}");
            }
        }

        return Inventory.Insert(s);
    }

    public ItemStack Extract(){
        return Inventory.Extract();
    }


    private void Start(){
#if PLAYERITEMS1
        Debug.Log("Adding dev inventory");
        Inventory.Insert(new ItemStack(Utils.Instance.furnaceBlock, 12));
        Inventory.Insert(new ItemStack(Utils.Instance.drillBlock, 12));
        Inventory.Insert(new ItemStack(Utils.Instance.crateBlock, 6));
        Inventory.Insert(new ItemStack(Utils.Instance.refinerBlock, 8));
        Inventory.Insert(new ItemStack(Utils.Instance.inserterBlock, 24));
        Inventory.Insert(new ItemStack(Utils.Instance.workstation, 3));

        Inventory.Insert(new ItemStack(Utils.Instance.railBlock, 54));
        Inventory.Insert(new ItemStack(Utils.Instance.railCart, 6));


        Inventory.Insert(new ItemStack(Utils.Instance.copperOre, 24));
        Inventory.Insert(new ItemStack(Utils.Instance.copperOre, 24));

#endif

        //add starting items
        foreach (ItemStack i in myCharacter.startingItems){
            Inventory.Insert(i.Clone(), false);
        }

        spriteHolder.transform.localPosition = new Vector3(0, 170, 0);
        yVelocity = -10;

        myCursor.gameObject.SetActive(false); //disable at first, enable when we land
    }

   
    public void SelectSlot(Slot slot){
        if (SelectedSlot != null)
            SelectedSlot.Selected = false;

        SelectedSlot = slot;
        SelectedSlot.Selected = true;

        handVisualizer.mySlot = SelectedSlot;
    }


    private void ClickPos(Vector2Int pos){
        SelectedSlot.ItemStack?.item.Use(pos, this, SelectedSlot);
    }


    private void RightClickPos(Vector2Int pos){
        Block b = TerrainManager.Instance.GetBlock(pos);

        if (b){
            if (Vector2.Distance(b.transform.position, transform.position) > 10f){
                if (BlockUIManager.Instance.currentBlockUI?.block == b){
                    //b.Use(this); could do this also, same thing basically as using should just close it       
                    BlockUIManager.Instance.CloseBlockUI();
                }
                else{
                    Popup("Out of reach", new Color(0.9f, 0.4f, 0.4f));
                    return;
                }
            }
            else{
                b.Use(this);

            }

        }
    }

    private void CTRLClickPos(Vector2Int pos){
        Debug.Log("ctrl click");
        if (Vector2.Distance(pos, transform.position) > 12f)
            return;
        if (TerrainManager.Instance.GetBlock(pos) is IContainerBlock containerBlock){
            //extract to my inventory
            bool b = true;
            while (b)
                b = CU.Transfer(containerBlock, this);
        }
    }

    public void Die(){
        Instantiate(OnDeath, transform.position, quaternion.identity);
        myCam.transform.SetParent(transform.parent.parent);
        Destroy(gameObject);

    }
  
    public override Stats CalculateStats(){
        finalStats = base.CalculateStats();
        finalStats.Combine(myCharacter.stats);

        return finalStats;
    }

    public void Popup(string s){
        popupList.AddPopup(s, Color.white);
    }

    public void Popup(string s, Color c){
        popupList.AddPopup(s, c);
    }
}