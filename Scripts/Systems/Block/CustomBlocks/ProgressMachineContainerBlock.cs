using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Systems.Items;

namespace Systems.Block.CustomBlocks{
    public class ProgressMachineContainerBlock:ProgressMachineBlock, IContainerBlock{

        //public new ProgressMachineContainerBlockData data => (ProgressMachineContainerBlockData)base.data;
        public ContainerProperties outputProperties;
    
        public Container output;


        public override void Init(Orientation orientation){
            base.Init(orientation);
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
            data.lootTable.AddRange(output.GetItems());
            return base.BlockDestroy();
        }
        
        public override void Load(BlockData d){
            base.Load(d);
            output = JsonSerializer.Deserialize<Container>(d.data.GetString("output"), GameManager.JSONoptions);
        }

        public override BlockData Save(){
            BlockData b = base.Save();
            b.data.SetString( "output", JsonSerializer.Serialize(output, GameManager.JSONoptions));
            return b;

        }

    }
    [Serializable]
    public class ProgressMachineContainerBlockData: ProgressMachineBlockData{
        public Container output;
    }
}