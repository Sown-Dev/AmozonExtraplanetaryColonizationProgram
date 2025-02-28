using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Items;
using Systems.Round;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Block.CustomBlocks{
    public class LootCrateBlock:ContainerBlock{
bool hasGenerated = false;
        public Drop[] drops;   
        //generation logic
        public void GenerateLoot(){
            if(hasGenerated) return;
            hasGenerated = true;
            int lootAmount  = 2+Random.Range(0,3) + RoundManager.Instance.roundNum;
            
            Utils.Shuffle(drops);
            drops.OrderBy( x => x.tier);
            
            //add loot going from curent tier to 0
            while (lootAmount > 0){


                for (int i = drops.Length - 1; i >= 0 && lootAmount > 0; i--){
                    if (drops[i].tier > RoundManager.Instance.roundNum) continue;

                    //if(Random.value<0.3f) continue; //random chance to skip
                    if (drops[i].chance > Random.value){
                        ItemStack s = drops[i].item;
                        output.Insert(s);
                        lootAmount--;
                    }
                }
                
            }
        }
        
        
        public override void Use(Unit user){
            GenerateLoot();
            base.Use(user);
        }
        
        
        //Can't extract
        public override ItemStack Extract(){
            return null;
        }

        public override void OnUIClose(){
            base.OnUIClose();
            /*if (output.isEmpty()){  //Would rather keep them existing, as it opens emergent gameplay to use them as storeage
                BlockDestroy(false);
            }*/
        }
    }
    [Serializable]
    public struct Drop{
        public ItemStack item;
        public float chance;
        public int tier;
    }
}