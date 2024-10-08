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

            foreach (var pos in TerrainManager.Instance.GetBlockPositions(origin, properties.size.x, properties.size.y)){
                if (TerrainManager.Instance.powerClaims.ContainsKey(pos)){
                    TerrainManager.Instance.powerClaims[pos].Connect(this);
                }

                break;

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