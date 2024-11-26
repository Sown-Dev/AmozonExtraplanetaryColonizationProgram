using System;
using NewRunMenu;
using Systems;
using Systems.Items;
using Systems.Round;
using Unity.Mathematics;
using UnityEngine;

public partial class Player : Unit{
    public static Player Instance;

    [SerializeField] public Cursor myCursor;
    
    [SerializeField] public SpriteRenderer buildingPreview;
    [SerializeField] private SlotVisualizer handVisualizer;
    [SerializeField] private Camera myCam;

    [SerializeField] private GameObject OnDeath;

    public Character myCharacter;

   
    public Slot SelectedSlot;

    protected override void Awake(){
        //myCharacter = GameManager.Instance.selectedChar;
        base.Awake();
        Instance = this;
        myCursor.OnLeftClick.AddListener(ClickPos);
        myCursor.OnRightClick.AddListener(RightClickPos);
        myCursor.OnCTRLClick.AddListener(CTRLClickPos);
        SelectSlot(Inventory.GetSlot(0)); 
        Inventory.AddOnInsert(OnInsert);
    }

    public void OnInsert(ItemStack s){
        try{
        RoundManager.Instance.myStats.itemsPickedUp+=s.amount;
        RoundManager.Instance.myStats.AddOrUpdateItem(s.item, true);
        }catch{

        }
    }

    private void Start(){
        
#if ALLITEMS1
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
        if(        TerrainManager.Instance.GetBlock(pos) is IContainerBlock containerBlock){
            //extract to my inventory
            bool b = true;
            while(b)
             b=CU.Transfer(containerBlock, Inventory);
        }
    }

    public void Die(){
        Instantiate(OnDeath, transform.position,quaternion.identity);
            myCam.transform.SetParent(transform.parent.parent);
        Destroy(gameObject);
    }

    public override Stats CalculateStats(){
        finalStats= base.CalculateStats();
        finalStats.Combine(myCharacter.stats);

        return finalStats;
    }
}