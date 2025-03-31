

using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class WindActuator: ProgressMachineBlock{
        protected override void Awake(){
            base.Awake();
            progressPerCycle = Random.Range(30, 50);
        }

        public override void CompleteCycle(){
            base.CompleteCycle();
            foreach(Block b in TerrainManager.Instance.GetAdjacentBlocks(data.origin, properties.size.x, properties.size.y)){
                b.Actuate();
            }
        }
    }
}