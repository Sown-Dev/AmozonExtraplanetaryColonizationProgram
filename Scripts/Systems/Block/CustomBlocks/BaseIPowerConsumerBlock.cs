using System.Linq;
using System.Text;
using Systems.Block.CustomBlocks;

public class BaseIPowerConsumerBlock: BaseIPowerBlock, IPowerConsumer{
    

    public int needed{ get; set; }
    public int providedPower{ get; set; }

    public override StringBuilder GetDescription(){
        return base.GetDescription().AppendFormat("\nConsuming: {0}W/{1}W", providedPower, needed);
    }

}