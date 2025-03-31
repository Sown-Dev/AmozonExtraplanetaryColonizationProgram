using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Items;
using Systems.Round;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Block.CustomBlocks{
    public class LootCrateBlock : ContainerBlock{
        
        public int extraTier = 0;
        public int baseDrops = 2;

        public bool hasGenerated = false;
        public Drop[] drops;

        //generation logic
        public void GenerateLoot(){
            if (hasGenerated) return;
            hasGenerated = true;
            int myTier = RoundManager.Instance.roundNum + extraTier;
            int lootAmount = baseDrops + Random.Range(0, 2) + (myTier / 2);

            Utils.Shuffle(drops);
            //drops.OrderBy( x => x.tier); used to order by tier, but whatever we can just skip over non matching tiers. no other code changes required

            //add loot going from curent tier to 0
            while (lootAmount > 0){
                for (int i = drops.Length - 1; i >= 0 && lootAmount > 0; i--){
                    if (drops[i].tier > myTier) continue;

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
        
        public override void InitializeData(){
            myData = new LootCrateBlockData();
        }


        /*//Can't extract
        public override ItemStack Extract(){
            return null;
        }*/

        public override void OnUIClose(){
            base.OnUIClose();
            /*if (output.isEmpty()){  //Would rather keep them existing, as it opens emergent gameplay to use them as storeage
                BlockDestroy(false);
            }*/
        }

        public override bool BlockDestroy(bool dropItems = true){
            GenerateLoot();
            return base.BlockDestroy(dropItems);
        }

        public override BlockData Save(){
            BlockData d =base.Save();
            d.data.SetBool("hasGenerated", hasGenerated);
            return d;
        }
        
        
        public override void Load(BlockData d){
            base.Load(d);
            hasGenerated = d.data.GetBool("hasGenerated");
        }
        
    }

    [Serializable]
    public struct Drop{
        public ItemStack item;
        public float chance;
        public int tier;
    }
    [Serializable]
    public class LootCrateBlockData : ContainerBlockData{
        public bool hasGenerated = false;

    }
}