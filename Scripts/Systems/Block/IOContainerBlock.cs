using Systems.Items;

namespace Systems.Block{
    public class IOContainerBlock: ContainerBlock, IContainerBlock{
        public Container input;
        public ContainerProperties inputProperties;
        
        protected override void Awake(){
            base.Awake();
            input = new Container(inputProperties);
        }
        //extract is the same
        public override bool Insert(ref ItemStack s, bool simulate = false){
            return input.Insert(ref s, simulate);
        }
        
        
        public override bool BlockDestroy(bool dropItems){
            lootTable.AddRange(input.GetItems());
            return base.BlockDestroy();
        }
    }
}