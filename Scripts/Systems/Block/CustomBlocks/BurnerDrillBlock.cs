using System.Collections.Generic;
using System.Text;
using Systems.Items;

namespace Systems.Block.CustomBlocks{
    public class BurnerDrillBlock:DrillBlock{
    
        public Burner burner;

        protected override void Awake(){
            burner.Init();
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


        public override StringBuilder GetDescription(){
            return burner == null
                ? base.GetDescription()
                : base.GetDescription().Append("\nFuel Remaining:").Append(burner.fuelTime);
        }
        
        //burner drops
        public override bool BlockDestroy(bool dropItems = true){
            lootTable.AddRange(burner.fuelContainer.GetItems());
            return base.BlockDestroy(dropItems);
        }
    }
}