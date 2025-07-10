using System;
using MemoryPack;
using Systems.Block;
using Systems.Items;

public class ContainerBlock: TickingBlock, IContainerBlock{
    public ContainerProperties outputProperties;

    public Container output;

   // public new ContainerBlockData data => (ContainerBlockData)base.data;
    

    public override void Init(Orientation orientation){
        base.Init(orientation);
        output = new Container(outputProperties);
        output.Priority = 1;
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
        data.lootTable.AddRange(output.GetItems());
        return base.BlockDestroy(dropItems);
    }

    public override void Load(BlockData d){
        base.Load(d);
        output = d.data.Get<Container>("output");
    }

    public override BlockData Save(){
        BlockData b = base.Save();
        b.data.Set("output", output);
        return b;

    }
}
[Serializable]
public class ContainerBlockData: TickingBlockData{
    public Container output;
}