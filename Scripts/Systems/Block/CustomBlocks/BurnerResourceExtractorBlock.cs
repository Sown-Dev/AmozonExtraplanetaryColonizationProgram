using System.Collections.Generic;
using System.Text;
using Systems.Items;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class BurnerResourceExtractorBlock : ResourceExtractorBlock{
        public Burner burner;

        protected override void Awake(){
            burner.Init();
            burner.Priority = 4;
            base.Awake();
        }

        public override void Tick(){
            if (burner.Burn()){
                base.Tick();
            }
            else{
                progressBar.progress = 0; // Reset progress if no fuel
            }
        }

        public override bool Insert(ref ItemStack mySlot, bool simulate = false){
            //only insert into burner if empty
            if(burner.fuelContainer.GetSlot(0).ItemStack == null){
                if (burner.Insert(ref mySlot, simulate)){
                    return true;
                }
            }
            return base.Insert(ref mySlot, simulate);
        }

        public override StringBuilder GetDescription(){
            return burner == null
                ? base.GetDescription()
                : base.GetDescription().Append("\nFuel Remaining:").Append(burner.fuelTime);
        }

        public override bool BlockDestroy(bool dropItems = true){
            lootTable.AddRange(burner.fuelContainer.GetItems());
            return base.BlockDestroy(dropItems);
        }
    }
}