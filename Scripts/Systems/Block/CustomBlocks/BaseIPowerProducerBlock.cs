using System.Text;
using Systems.Block.CustomBlocks;

namespace Systems.Block{
    public class BaseIPowerProducerBlock: BaseIPowerBlock, IPowerProducer{
        public int producing{ get; set; }
        public int maxProduction{ get; set; }
        public bool neededOn{ get; set; }

        public int baseRate = 50;

        public override StringBuilder GetDescription(){
            return base.GetDescription().AppendFormat("\nProducing: {0}/{1}", producing, baseRate);
        }
    }
}