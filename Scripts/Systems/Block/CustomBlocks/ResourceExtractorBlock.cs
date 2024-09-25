using Systems.Items;

namespace Systems.Block.CustomBlocks{
    public class ResourceExtractorBlock: ContainerBlock{
        public int DrillTime = 80;
        public int DrillAmount;

        public int hardness = 0;


        public ProgressBar progressBar = new ProgressBar();

        private ResourceBlock myResource;
        
        public override void Tick(){
            base.Tick();
            if (myResource == null) return;     
            
            progressBar.progress++;

            if (progressBar.progress >= progressBar.maxProgress){
                progressBar.progress = 0;
                Extract();
            }
        }

        public void Extract(){
            ItemStack s = myResource.Extract(DrillAmount);
            if (s != null)
                output.Insert(ref s);
        }

        public override void Init( Orientation orientation){
            base.Init( orientation);
            myResource = GetDirection(orientation) as ResourceBlock;
        }

        
    }
}