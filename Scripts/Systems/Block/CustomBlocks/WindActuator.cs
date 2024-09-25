

using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class WindActuator: ProgressMachineBlock{
        protected override void Awake(){
            base.Awake();
            progressBar.maxProgress = Random.Range(20, 40);
            progressBar.progress = 0;
        }

        public override void CompleteCycle(){
            base.CompleteCycle();
            foreach(Block b in TerrainManager.Instance.GetAdjacentBlocks(origin, properties.size.x, properties.size.y)){
                b.Actuate();
            }
        }
    }
}