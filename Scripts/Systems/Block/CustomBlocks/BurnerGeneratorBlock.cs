namespace Systems.Block{
    public class BurnerGeneratorBlock: BaseIPowerProducerBlock{
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
    
    }
}