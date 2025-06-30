using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ElectricRecipeBlock : RecipeBlock, IPowerConsumer{
        public int Priority{ get; set; }
        public bool Hidden{ get; set; }
        public PowerGrid myGrid{ get; set; }
        public Block myBlock => this;
        public IPowerConnector myConnector{ get; set; }

         public int needed{ get; set; } = 0;
        public int providedPower{ get; set; }

        public int baseUsage = 100;

        protected override void Start(){
            base.Start();
            GetConnected();

        }
        
        public override void Init(Orientation orientation){
            base.Init(orientation);
            
        }

        public void GetConnected(){
            foreach (var pos in TerrainManager.Instance.GetBlockPositions(data.origin, properties.size.x, properties.size.y)){
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
        
        public override void Tick(){
            needed = CanCraft() ? baseUsage: 0;
            base.Tick();
        }

        public override bool CanProgress(){
            if (!base.CanProgress()){
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