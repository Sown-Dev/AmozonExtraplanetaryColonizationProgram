using System.Collections.Generic;
using Systems.Items;

namespace Systems.Block.CustomBlocks{
    public class BurnerDrillBlock:DrillBlock{
    
        public Burner burner;
        public Item[] fuelFilter;

        protected override void Awake(){
            burner = new Burner(2, 1, fuelFilter);
            burner.Priority = 4;
            base.Awake();
        }

        public override bool CanMine(){
            if (base.CanMine())
                return burner.Burn();
            return false;
        }

        public override bool Insert(ref ItemStack mySlot, bool simulate = false){
            if (burner.Insert(ref mySlot, simulate)){
                return true;
            }
            return base.Insert(ref mySlot, simulate);
        }


        public override string GetDescription(){
            return $"{base.GetDescription()}\nFuel Remaining: {burner.fuelTime}";
        }
    }
}