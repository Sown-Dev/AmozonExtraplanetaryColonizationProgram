using Systems.Block.CustomBlocks;
using UnityEngine;

namespace Systems.Block{
    // could maybe change from container to regular progress block, but ill do that when i need it (ie if i add a power consumer that doesnt have an inventory)
    public class ElectricProgressBlock: ProgressMachineContainerBlock, IPowerConsumer{
        //COPY BELOW TO ADD TO BLOCKS THAT CANT INHERIT
        public int Priority{ get; set; }
        public bool Hidden{ get; set; }
        public PowerGrid myGrid{ get; set; }
        public Block myBlock => this;
        public IPowerConnector myConnector{ get; set; }



        public int baseUsage;
        public int needed{ get; set; }
        public int providedPower{ get; set; }

        public override void Init(Orientation orientation){
            needed = baseUsage;
            base.Init(orientation);
            GetConnected();
            
            
        }

        public void GetConnected(){
            Debug.Log("size +"+TerrainManager.Instance.GetBlockPositions(data.origin, properties.size.x, properties.size.y).Count);
            foreach (var pos in TerrainManager.Instance.GetBlockPositions(data.origin, properties.size.x, properties.size.y)){
                if (TerrainManager.Instance.powerClaims.ContainsKey(pos)){
                    TerrainManager.Instance.powerClaims[pos].Connect(this);
                    
                }
            }
        }

        private void OnDrawGizmos(){
            Gizmos.color = Color.blue;
            foreach (var pos in TerrainManager.Instance.GetBlockPositions(data.origin, properties.size.x, properties.size.y)){
               
                Gizmos.DrawLine((Vector2)pos, (Vector2)data.origin);
                

            }
        }

        public override bool BlockDestroy(bool dropLoot = true){
            if (!base.BlockDestroy(dropLoot)){
                return false;
            }
            myConnector?.Disconnect(this);
            

            return true;
        }
        
        //STOP HERE


        public override bool CanProgress(){
            if(base.CanProgress()){
                if(myGrid!=null)
                    return providedPower >= needed;
            }

            return false;
        }
    }
}