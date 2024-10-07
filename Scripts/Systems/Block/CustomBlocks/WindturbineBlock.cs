using Systems.Block;

public class WindturbineBlock : TickingBlock, IPowerProducer{
    public int providedPower{ get; set; }
    public PowerGrid myGrid{ get; set; }
    public Block myBlock{
        get{ return this; }
    }

    public int Priority{ get; set; }
    public bool Hidden{ get; set; }

    public int producing{ get; set; }
    public bool neededOn{ get; set; }

    private int baseRate = 50;
    public override void Tick(){
        base.Tick();
        int surrounding = 0;
        surrounding += GetAdjascent().Count;
        producing = (int)(baseRate * (10f-surrounding)/10f);
    }
}