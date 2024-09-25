using Systems.Items;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class DeepveinMinerBlock:ProgressMachineContainerBlock{
        
        protected override void Awake(){
            base.Awake();
            progressBar.maxProgress = 40;
        }

        public override bool CanProgress(){
            return base.CanProgress() && !output.isEmpty();
        }

        public override void CompleteCycle(){
            base.CompleteCycle();
            if(output.isEmpty()) return;
            
            ItemStack itemStack = new ItemStack(ItemManager.Instance.ores[Random.Range(0,ItemManager.Instance.ores.Count)], 1);
            output.Insert(ref itemStack);
             
        }
    }
}