namespace Systems.Block.CustomBlocks{
    public class DisposableDrillBlock:DrillBlock{
    
        
        public int Durability = 100;

        public override void Drill(){
            if (Durability < 0){
                TerrainManager.Instance.RemoveBlock(origin);    
                return;
            }
            base.Drill();
            Durability--;
        }

        public override bool BlockDestroy(bool dropItems = true){
            return base.BlockDestroy(false);
        }


        public override string GetDescription(){
            return $"{base.GetDescription()}\nDurability: {Durability}";
        }
    }
}