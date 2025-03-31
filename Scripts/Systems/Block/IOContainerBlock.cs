using System;
using Systems.Items;

namespace Systems.Block{
    public class IOContainerBlock: ContainerBlock, IContainerBlock{
        public ContainerProperties inputProperties;

        public new IOContainerBlockData data => (IOContainerBlockData)base.data;
        
        protected override void Awake(){
            base.Awake();
        }
        
        public override void Init(Orientation orientation){
            base.Init(orientation);
            data.input = new Container(inputProperties);
        }

      
        //extract is the same
        public override bool Insert(ref ItemStack s, bool simulate = false){
            return data.input.Insert(ref s, simulate);
        }
        
        
        public override bool BlockDestroy(bool dropItems){
            data.lootTable.AddRange(data.input.GetItems());
            return base.BlockDestroy();
        }
    }
    [Serializable]
    public class IOContainerBlockData: ContainerBlockData{
        public Container input;
    }
}