using System;
using System.Collections.Generic;
using Systems.Items;
using Systems.Round;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Block.CustomBlocks{
    public class LootCrateBlock: Block,IContainerBlock{
        [HideInInspector] private List<ItemStack> loot;

        public Drop[][] drops;   
        //generation logic
        public void GenerateLoot(){
            
            int lootAmount  = 2+Random.Range(0,3) + RoundManager.Instance.roundNum;
            for(int i=RoundManager.Instance.roundNum; i>=0; i--){
                
                Utils.Shuffle(drops[i]);
                foreach(Drop d in drops[i]){
                    if(Random.value<d.chance && lootAmount>0){
                        lootAmount--;
                        loot.Add(d.item);
                    }
                }
            }
        }
        
        
        public override void Use(Unit user){
            //base.Use(user)
            BlockDestroy(false);
            //ui 
            foreach (ItemStack s in loot){
                user.Inventory.Insert(s);
            }
        }
        
        public bool Insert(ref ItemStack s, bool simulate = false){
            if(!simulate && loot?.Count<32){ //dont want it to be infinite storage
                loot.Add(s);
                return true;
            }

            return false;
        }
        //Can't extract
        public ItemStack Extract(){
            return null;
        }
    }
    [Serializable]
    public struct Drop{
        public ItemStack item;
        public float chance;
        public int tier;
    }
}