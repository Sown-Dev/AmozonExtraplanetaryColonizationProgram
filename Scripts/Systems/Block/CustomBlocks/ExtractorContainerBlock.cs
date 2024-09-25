using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ExtractorContainerBlock: ProgressMachineContainerBlock{
    
    
        public ProgressBar progressBar;

        public ParticleSystem ps;
    
        protected override void Awake(){
            base.Awake();
            progressBar.maxProgress = 80;
        }
        
        public override void CompleteCycle(){
            base.CompleteCycle();
            //extract all items from adjacent blocks using terrain manager
            foreach(Block b in TerrainManager.Instance.GetAdjacentBlocks(origin, properties.size.x, properties.size.y)){
                if(b is IContainerBlock containerBlock){
                    CU.Transfer(containerBlock, this);
                }
            }
            ps?.Play();
            
        } 
    }
}