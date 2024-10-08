using Systems.Items;

namespace Systems.Block.CustomBlocks{
    public class ProgressMachineContainerBlock:ProgressMachineBlock, IContainerBlock{
        public Container output;
        public ContainerProperties outputProperties;
    
        protected override void Awake(){
            base.Awake();
            output = new Container(outputProperties);
            output.Priority = 1;
        }
    
        public virtual bool Insert(ref ItemStack mySlot, bool simulate = false){
            return output.Insert(ref mySlot, simulate);
        
        }

        public virtual  ItemStack Extract(){
            return output.Extract();

        }

        public override bool BlockDestroy(bool dropItems = true){
            lootTable.AddRange(output.GetItems());
            return base.BlockDestroy();
        }

    }
}