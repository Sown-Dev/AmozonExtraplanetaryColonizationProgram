using Systems.Block;
using Systems.BlockUI;

public interface IPowerBlock: IBlockUI{
    public PowerGrid myGrid{ get; set; } //the grid that the block is in. If null, then no grid and no power
    public Block myBlock{ get; } //stupid, but we need a reference to the block
}

public interface IPowerProducer: IPowerBlock{
    public int producing{internal get; set; }
    
}

public interface IPowerConsumer: IPowerBlock{
    public int consuming{ get; set; }
    public int providedPower{ get; set; }//power that the block has. is set outside but only used internally
}
