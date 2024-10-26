using Systems.Items;

namespace Systems.Block{
    public class BurnerGeneratorBlock: BaseIPowerProducerBlock, IContainerBlock{
        public Burner burner;
        
        protected override void Awake(){
            base.Awake();
            burner.Init();
            burner.Priority = 4;
        }
        public override void Tick(){
            base.Tick();
            if(neededOn)
                producing = burner.Burn()? baseRate: 0;
        }
        
        public bool Insert(ref ItemStack mySlot, bool simulate = false){
            if (burner.Insert(ref mySlot, simulate)){
                return true;
            }
            return false;
        }

        public ItemStack Extract(){
            return null;
        }


       
    }
}