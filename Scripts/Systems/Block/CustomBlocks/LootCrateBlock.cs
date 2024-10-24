using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Items;
using Systems.Round;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Block.CustomBlocks{
    public class LootCrateBlock: Block,IContainerBlock{
        [HideInInspector] private List<ItemStack> loot;

        public Drop[] drops;   
        //generation logic
        public void GenerateLoot(){
            
            int lootAmount  = 2+Random.Range(0,3) + RoundManager.Instance.roundNum;
            
            //add loot going from curent tier to 0
            drops.OrderBy( x => x.tier);
            
            for (int i = drops.Length-1; i >= 0 && lootAmount>0; i--){
                if(drops[i].tier>RoundManager.Instance.roundNum) continue;
                
                //if(Random.value<0.3f) continue; //random chance to skip
                if (drops[i].chance < Random.value){
                    ItemStack s = drops[i].item;
                    loot.Add(s);
                    lootAmount--;
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