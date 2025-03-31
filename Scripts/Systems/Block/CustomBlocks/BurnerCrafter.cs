using System;
using Newtonsoft.Json;
using Systems.Items;
using UI.BlockUI;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class BurnerCrafterBlock: RecipeBlock{

        //public new BurnerCrafterBlockData data => (BurnerCrafterBlockData)base.data;
        public Burner burner;

        public override void InitializeData(){
            myData = new BurnerCrafterBlockData();
        }

        public override void Init(Orientation orientation){
            base.Init(orientation);
            burner.Init();
            burner.Priority = 4;
        }
        
        public override bool CanProgress(){
            if (base.CanCraft())
                return burner.Burn();
            return false;
        }
        
        public override bool Insert(ref ItemStack mySlot, bool simulate = false){
            if (burner.Insert(ref mySlot, simulate)){
                return true;
            }
            return base.Insert(ref mySlot, simulate);
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
    public class BurnerCrafterBlockData : RecipeBlockData{
        public Burner burner;
    }
}