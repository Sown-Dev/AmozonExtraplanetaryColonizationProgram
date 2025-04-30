namespace Systems.BlockUI{
    public interface IBlockUI
    {
        public int Priority { get; set; }
        
        public bool Hidden { get; set; }
    }
    
}