using Systems.Items;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class DeepveinMinerBlock:ElectricProgressBlock{
        public int miningTime = 50;
        protected override void Awake(){
            base.Awake();
            progressBar.maxProgress = miningTime;
        }

        public override bool CanProgress(){
            return base.CanProgress() && !output.isFull();
        }

        public override void CompleteCycle(){
            base.CompleteCycle();
            if(output.isFull()) return;
            
            ItemStack itemStack = new ItemStack(ItemManager.Instance.ores[Random.Range(0,ItemManager.Instance.ores.Count)], 1);
            Insert(ref itemStack);
             
        }
    }
}