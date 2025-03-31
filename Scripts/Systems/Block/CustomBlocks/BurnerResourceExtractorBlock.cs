using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Systems.Items;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class BurnerResourceExtractorBlock : ResourceExtractorBlock{

        //public new BurnerResourceExtractorBlockData data => (BurnerResourceExtractorBlockData)myData;

        public Burner burner;


        public override void Init(Orientation orientation){
            base.Init(orientation);
            burner = new Burner();
            burner.Init();
            burner.Priority = 21;
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
            data.lootTable.AddRange(burner.fuelContainer.GetItems());
            return base.BlockDestroy(dropItems);
        }
        
        public override BlockData Save(){
            BlockData d= base.Save();
            d.data.SetString("burner", JsonConvert.SerializeObject(burner, GameManager.JSONsettings));
            return d;
        }
        public override void Load(BlockData d){
            base.Load(d);
            burner = JsonConvert.DeserializeObject<Burner>(d.data.GetString("burner"), GameManager.JSONsettings);
        }
    }
    
    [System.Serializable]
    public class BurnerResourceExtractorBlockData : ContainerBlockData{
        public Burner burner;
    }
}