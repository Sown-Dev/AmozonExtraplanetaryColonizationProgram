using Systems.Items;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ResourceBlock : Block{
        public Item item;
        public int baseYield;
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
            return new ItemStack(item, amt * baseYield);
        }

        public override bool BlockDestroy(bool dropLoot){
            //if (hardness < 0){
                Utils.Instance.CreateItemDrop(Extract(1), transform.position + Vector3.down);
                TerrainManager.Instance.CreateBlockDebris(origin, color);
                return true;
            //}

            return false;
        }

        public override string ToString(){
            return base.ToString() + "\nAmount: " + amount;
        }
    }
}