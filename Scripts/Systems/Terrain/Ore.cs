using System;
using Systems.Items;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Systems.Terrain{
    [Serializable]
    
    public class Ore{

        public OreProperties myProperties;
        //Ore Info:
        public Item oreItem;

        public RuleTile tile;


        public int amount;

        public virtual ItemStack ExtractOre(int oreAmt){
            int stackAmount = 0;

            stackAmount = Mathf.Min(oreAmt, this.amount);
            
            
            //Stats stuff
            stackAmount = Mathf.RoundToInt( stackAmount* TerrainManager.Instance.finalStats[Statstype.OreYieldMult]);
            stackAmount +=  Mathf.RoundToInt(TerrainManager.Instance.finalStats[Statstype.OreYieldAdd]);
            if(Random.value < TerrainManager.Instance.finalStats[Statstype.OreUseChance])
                amount -= stackAmount;

            if (amount <= 0){
                amount = 0;
            }

            return new ItemStack(oreItem, stackAmount);
        }
        
        public Ore Clone(){
            return (Ore)this.MemberwiseClone();
        }
    }
}