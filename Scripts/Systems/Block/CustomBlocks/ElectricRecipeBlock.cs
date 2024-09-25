namespace Systems.Block.CustomBlocks{
    public class ElectricRecipeBlock : RecipeBlock, IPowerConsumer{
        public int Priority{ get; set; }
        public bool Hidden{ get; set; }

        public int providedPower{ get; set; }

        public PowerGrid myGrid{ get; set; }

        public Block myBlock{
            get{ return this; }
        }

        public int consuming{ get; set; } = 100;

        public override bool CanCraft(){
            if (myGrid == null){
                return false;
            }

            if (providedPower < consuming){
                return false;
            }

            return base.CanCraft();
        }
    }
}