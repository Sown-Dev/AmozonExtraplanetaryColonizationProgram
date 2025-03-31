using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Systems.Items;

namespace Systems.Block.CustomBlocks{
    public class BurnerDrillBlock:DrillBlock{
    
        //public new BurnerDrillBlockData data => (BurnerDrillBlockData)base.data;
        public Burner burner= new Burner();

        
        public override void Init(Orientation orientation){
            base.Init(orientation);
            burner.Init();
            burner.Priority = 4;
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
    [Serializable]
    public class BurnerDrillBlockData : DrillBlockData{
        public Burner burner= new Burner();
    }
}