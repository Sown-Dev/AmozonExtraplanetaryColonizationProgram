using Systems.BlockUI;

namespace Systems.Items{
    public class Filter: IBlockUI{

        public Item filter;
        public int Priority{ get; set; }
        public bool Hidden{ get; set; }
        
        public Filter(){
            filter = null;
            Priority = 0;
            Hidden = false;
        }
        public Filter(Item filter){
            this.filter = filter;
            Priority = 0;
            Hidden = false;
        }
    }
}