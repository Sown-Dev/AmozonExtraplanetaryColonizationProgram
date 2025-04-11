using System;
using System.Text;
using Systems.Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Block.CustomBlocks{
    public class ResourceBlock : Block{
        
        public Item item;
        public int baseYield=1;
        public int baseAmount;
        public int range;

        public int amount;
    
        public Color color;

        public int hardness = 0;
        
        public override void InitializeData(){
            base.InitializeData();

        }
        
        public override void Init(Orientation orientation){
            base.Init(orientation);
            amount = baseAmount + Random.Range(-range, range);
        }

        public ItemStack Extract(int amt){
            amt *= baseYield;
            if (amount - amt <= 0){
                amt = amount;
                amount = 0;
                TerrainManager.Instance.RemoveBlock(data.origin, false);

                return new ItemStack(item, amt * baseYield);   
            }
            amount -= amt;
            //create block debris
            TerrainManager.Instance.CreateBlockDebris(data.origin, color);
            return new ItemStack(item, amt);    
        }

        public override bool BlockDestroy(bool dropLoot){
            if (amount > 0 && dropLoot){
            
                Utils.Instance.CreateItemDrop(Extract(1), transform.position + Vector3.down);
                return true;
            }
            else{
                return base.BlockDestroy(dropLoot);
            }

        }

        public override StringBuilder GetDescription(){
            
            return base.GetDescription().Append( "\nAmount: ").Append(amount);
        }

        public override void Load(BlockData d){
            base.Load(d);
            amount = d.data.GetInt("amount");
        }
        
        public override BlockData Save(){
            BlockData b = base.Save();
            b.data.SetInt("amount", amount);
            return b;
        }
    }
    [Serializable]
    public class ResourceBlockData : BlockData{
        public int amount;
    }
}