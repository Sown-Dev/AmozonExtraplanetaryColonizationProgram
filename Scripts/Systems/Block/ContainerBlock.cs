using Systems.Block;
using Systems.Items;

public class ContainerBlock: TickingBlock<ContainerBlockData>, IContainerBlock{
    
    
    protected override void Awake(){
        base.Awake();
        data.output = new Container(data.outputProperties);
    }
    
    public virtual bool Insert(ref ItemStack mySlot, bool simulate = false){
        return data.output.Insert(ref mySlot, simulate);
        
    }

    public virtual  ItemStack Extract(){
        return data.output.Extract();

    }
    
    public Slot GetInsertionSlot( ItemStack s = null){
        return data.output.GetInsertionSlot(s);
    }

    public override bool BlockDestroy(bool dropItems = true){
        lootTable.AddRange(output.GetItems());
        return base.BlockDestroy(dropItems);
    }
}

public class ContainerBlockData: TickingBlockData{
    public Container output;
    public ContainerProperties outputProperties;
}