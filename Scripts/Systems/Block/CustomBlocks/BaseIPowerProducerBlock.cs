using System.Text;
using Systems.Block.CustomBlocks;

public class BaseIPowerProducerBlock: BaseIPowerBlock, IPowerProducer{
    public int producing{ get; set; }
    public bool neededOn{ get; set; }

    public int baseRate = 50;

    public override StringBuilder GetDescription(){
        return base.GetDescription().Append("\nProducing: ").Append(producing);
    }
}