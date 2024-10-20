using System.Text;
using Systems.Items;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ResourceBlock : Block{
        public Item item;
        public int baseYield=1;
        public int baseAmount;
        public int range;
    
        private int amount;
        public Color color;

        public int hardness = 0;


        protected override void Awake(){
            base.Awake();
            amount = baseAmount + Random.Range(-range, range);
        }

        public ItemStack Extract(int amt){
            if (amount - amt <= 0) return null;
            amount -= amt;
            //create block debris
            TerrainManager.Instance.CreateBlockDebris(origin, color);
            return new ItemStack(item, amt * baseYield);    
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
        
    }
}