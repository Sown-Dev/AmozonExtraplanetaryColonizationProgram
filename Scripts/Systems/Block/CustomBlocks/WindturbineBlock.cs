namespace Systems.Block.CustomBlocks{
    public class WindturbineBlock : BaseIPowerProducerBlock{
        public override void Tick(){
            base.Tick();
            int surrounding = 0;
            surrounding += GetAdjascent().Count;
            producing = (int)(baseRate * (10f-surrounding)/10f);
        }
    }
}