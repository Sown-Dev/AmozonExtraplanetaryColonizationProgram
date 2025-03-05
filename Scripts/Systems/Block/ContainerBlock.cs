using Systems.Block;
using Systems.Items;

public class ContainerBlock: TickingBlock, IContainerBlock{
    public Container output;
    public ContainerProperties outputProperties;
    
    protected override void Awake(){
        base.Awake();
        output = new Container(outputProperties);
    }
    
    public virtual bool Insert(ref ItemStack mySlot, bool simulate = false){
        return output.Insert(ref mySlot, simulate);
        
    }

    public virtual  ItemStack Extract(){
        return output.Extract();

    }
    
    public Slot GetInsertionSlot( ItemStack s = null){
        return output.GetInsertionSlot(s);
    }

    public override bool BlockDestroy(bool dropItems = true){
        lootTable.AddRange(output.GetItems());
        return base.BlockDestroy(dropItems);
    }
}