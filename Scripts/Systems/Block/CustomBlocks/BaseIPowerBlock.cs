using System;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class BaseIPowerBlock:TickingBlock, IPowerBlock{
        //COPY BELOW TO ADD TO BLOCKS THAT CANT INHERIT
        public int Priority{ get; set; }
        public bool Hidden{ get; set; }
        public PowerGrid myGrid{ get; set; }
        public Block myBlock => this;
        public IPowerConnector myConnector{ get; set; }

        public override void Init(Orientation orientation){
            base.Init(orientation);
            GetConnected();
            
            
        }

        public void GetConnected(){
            Debug.Log("size +"+TerrainManager.Instance.GetBlockPositions(origin, properties.size.x, properties.size.y).Count);
            foreach (var pos in TerrainManager.Instance.GetBlockPositions(origin, properties.size.x, properties.size.y)){
                if (TerrainManager.Instance.powerClaims.ContainsKey(pos)){
                    TerrainManager.Instance.powerClaims[pos].Connect(this);
                    
                }

                

            }
        }

        private void OnDrawGizmos(){
            Gizmos.color = Color.blue;
            foreach (var pos in TerrainManager.Instance.GetBlockPositions(origin, properties.size.x, properties.size.y)){
               
                Gizmos.DrawLine((Vector2)pos, (Vector2)origin);
                

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
    }
}