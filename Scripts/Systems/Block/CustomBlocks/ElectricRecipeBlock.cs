using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ElectricRecipeBlock : RecipeBlock, IPowerConsumer{
        public int Priority{ get; set; }
        public bool Hidden{ get; set; }
        public PowerGrid myGrid{ get; set; }
        public Block myBlock => this;
        public IPowerConnector myConnector{ get; set; }

        [field:SerializeField] public int needed{ get; set; } = 100;
        public int providedPower{ get; set; }

        public override void Init(Orientation orientation){
            base.Init(orientation);
            GetConnected();
            
            
        }

        public void GetConnected(){
            foreach (var pos in TerrainManager.Instance.GetBlockPositions(origin, properties.size.x, properties.size.y)){
                if (TerrainManager.Instance.powerClaims.ContainsKey(pos)){
                    TerrainManager.Instance.powerClaims[pos].Connect(this);
                    break;
                }

                

            }
        }
        public override bool BlockDestroy(bool dropLoot = true){
            if (!base.BlockDestroy(dropLoot)){
                return false;
            }
            myConnector?.Disconnect(this);
            

            return true;
        }



        public override bool CanCraft(){
            if (!base.CanCraft()){
                return false;
            }
            if (myGrid == null){
                return false;
            }

            if (providedPower < needed){
                return false;
            }

            return true;
        }

     
    }
}