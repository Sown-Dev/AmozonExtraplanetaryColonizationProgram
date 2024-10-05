using Systems.Block;

public class WindturbineBlock : Block, IPowerProducer{
    public int providedPower{ get; set; }
    public PowerGrid myGrid{ get; set; }
    public Block myBlock{
        get{ return this; }
    }

    public int Priority{ get; set; }
    public bool Hidden{ get; set; }

    public int producing{ get; set; }
    public bool neededOn{ get; set; }
    
    
}