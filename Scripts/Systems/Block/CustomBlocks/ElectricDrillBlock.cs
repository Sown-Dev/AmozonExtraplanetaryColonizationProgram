namespace Systems.Block.CustomBlocks{
    public class ElectricDrillBlock : DrillBlock, IPowerConsumer{
        public int Priority{ get; set; }
        public bool Hidden{ get; set; }
        public PowerGrid myGrid{ get; set; }
        public Block myBlock => this;
        public IPowerConnector myConnector{ get; set; }
        
        public int needed{ get; set; }
        public int providedPower{ get; set; }

        public int baseUsage = 120;

        public override void Init(Orientation orientation){
            base.Init(orientation);

            foreach (var pos in TerrainManager.Instance.GetBlockPositions(origin, properties.size.x, properties.size.y)){
                if (TerrainManager.Instance.powerClaims.ContainsKey(pos)){
                    TerrainManager.Instance.powerClaims[pos].Connect(this);
                }

                break;

            }
            
        }
        public override string GetDescription(){
            return $"{base.GetDescription()}\nConsumes {baseUsage}ϟW";
        }

        public override bool BlockDestroy(bool dropLoot = true){
            if (!base.BlockDestroy(dropLoot)){
                return false;
            }
            myConnector?.Disconnect(this);
            

            return true;
        }



        public override bool CanMine(){
            if (myGrid == null){
                return false;
            }

            needed = 100;
            if (providedPower < needed){
                return false;
            }

            return base.CanMine();
        }

     
    }
}