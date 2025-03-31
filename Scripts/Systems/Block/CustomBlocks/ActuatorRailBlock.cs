namespace Systems.Block{
    public class ActuatorRailBlock:RailBlock{
        public override void OnCartEnter(Cart cart){
            base.OnCartEnter(cart);
            //actuate all adjascent
            foreach (var block in TerrainManager.Instance.GetAdjacentBlocks(data.origin, properties.size.x, properties.size.y)){
                block.Actuate();
            
            }
        
        }
    }
}