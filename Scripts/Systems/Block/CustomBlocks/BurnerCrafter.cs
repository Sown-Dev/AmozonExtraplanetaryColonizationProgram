using Systems.Items;
using UI.BlockUI;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class BurnerCrafterBlock: RecipeBlock{
        public Burner burner;
        public Item[] fuelFilter;

        protected override void Awake(){
            burner = new Burner(1, 1, fuelFilter);
            burner.Priority = 4;
            base.Awake();
        }

        public override bool CanCraft(){
            if (base.CanCraft())
                return burner.Burn();
            return false;
        }
    }
}