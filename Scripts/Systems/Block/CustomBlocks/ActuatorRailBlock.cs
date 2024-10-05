namespace Systems.Block.CustomBlocks{
    public class ActuatorRailBlock:RailBlock{
        public override void OnCartEnter(Cart cart){
            base.OnCartEnter(cart);
            //actuate all adjascent
            foreach (var block in TerrainManager.Instance.GetAdjacentBlocks(origin, properties.size.x, properties.size.y)){
                block.Actuate();
            
            }
        
        }
    }
}