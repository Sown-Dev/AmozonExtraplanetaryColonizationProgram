using System.Text;

namespace Systems.Block.CustomBlocks{
    public class ElectricDrillBlock : DrillBlock, IPowerConsumer{
        public int Priority{ get; } = 10;
        public bool Hidden{ get; set; }
        public PowerGrid myGrid{ get; set; }
        public Block myBlock => this;
        public IPowerConnector myConnector{ get; set; }
        
        public int needed{ get; set; }
        public int providedPower{ get; set; }

        public int baseUsage = 120;

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
        
        public override StringBuilder GetDescription(){
            return base.GetDescription().AppendFormat("\nConsuming: {0}W/{1}W", providedPower, needed);
        }

        public override bool BlockDestroy(bool dropLoot = true){
            if (!base.BlockDestroy(dropLoot)){
                return false;
            }
            myConnector?.Disconnect(this);
            

            return true;
        }



        public override bool CanMine(){
            needed = baseUsage;
            if (myGrid == null){
                return false;
            }

            needed = baseUsage;
            if (providedPower < needed){
                return false;
            }

            return base.CanMine();
        }

     
    }
}