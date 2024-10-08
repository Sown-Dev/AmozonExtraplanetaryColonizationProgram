using Systems.Block.CustomBlocks;

public class BaseIPowerProducerBlock: BaseIPowerBlock{
    public int producing{ get; set; }
    public bool neededOn{ get; set; }

    public int baseRate = 50;

    public override string GetDescription(){
        return $"{base.GetDescription()}\nProducing: {producing}W";
    }
}