using System;
using System.Linq;
using NewRunMenu;
using Systems;
using Systems.Items;
using Systems.Round;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public partial class Player : Unit{
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
            AddUpgrade( (Upgrade)u.u,false);
        }
        
        base.Awake();
        Instance = this;
        myCursor.OnLeftClick.AddListener(ClickPos);
        myCursor.OnRightClick.AddListener(RightClickPos);
        myCursor.OnCTRLClick.AddListener(CTRLClickPos);
        SelectSlot(Inventory.GetSlot(0));
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
        if (Vector2.Distance(pos, transform.position) > 12f)
            return;
        TerrainManager.Instance.GetBlock(pos)?.Use(this);
    }

    private void CTRLClickPos(Vector2Int pos){
        Debug.Log("ctrl click");
        if (Vector2.Distance(pos, transform.position) > 12f)
            return;
        if (TerrainManager.Instance.GetBlock(pos) is IContainerBlock containerBlock){
            //extract to my inventory
            bool b = true;
            while (b)
                b = CU.Transfer(containerBlock, Inventory);
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
        popupList.AddPopup(s);
    }
}