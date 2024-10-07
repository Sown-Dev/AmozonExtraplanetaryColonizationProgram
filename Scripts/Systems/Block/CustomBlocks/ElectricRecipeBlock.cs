namespace Systems.Block.CustomBlocks{
    public class ElectricRecipeBlock : RecipeBlock, IPowerConsumer{
        public int Priority{ get; set; }
        public bool Hidden{ get; set; }

        public int needed{ get; set; }
        public int providedPower{ get; set; }

        public PowerGrid myGrid{ get; set; }

        public Block myBlock{
            get{ return this; }
        }


        public override bool CanCraft(){
            if (myGrid == null){
                return false;
            }

            needed = 100;
            if (providedPower < needed){
                return false;
            }

            return base.CanCraft();
        }
    }
}