using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Systems.Items;
using Systems.Block;

namespace Systems.Block.CustomBlocks{
    public class BurnerProgressBarBlock : ProgressMachineContainerBlock{
//public BurnerProgressBlockData data => (BurnerProgressBlockData)base.data;
        public Burner burner;


        public override void Init(Orientation orientation){
            base.Init(orientation);
            burner = new Burner();
            burner.Init();
            burner.Priority = 4;
        }

        public override bool CanProgress(){
            if (base.CanProgress())
                return burner.Burn();
            return false;
        }

        public override bool Insert(ref ItemStack mySlot, bool simulate = false){
            if (burner.Insert(ref mySlot, simulate))
                return true;

            return base.Insert(ref mySlot, simulate);
        }

        public override StringBuilder GetDescription(){
            return burner == null
                ? base.GetDescription()
                : base.GetDescription().Append("\nFuel Remaining: ").Append(burner.fuelTime);
        }

        public override bool BlockDestroy(bool dropItems = true){
            if (dropItems)
                data.lootTable.AddRange(burner.fuelContainer.GetItems());

            return base.BlockDestroy(dropItems);
        }
        
        public override BlockData Save(){
            BlockData d= base.Save();
            d.data.SetString("burner", JsonSerializer.Serialize(burner, GameManager.JSONoptions));
            return d;
        }
        public override void Load(BlockData d){
            base.Load(d);
            burner = JsonSerializer.Deserialize<Burner>(d.data.GetString("burner"), GameManager.JSONoptions);
        }
    }

    [Serializable]
    public class BurnerProgressBlockData : ProgressMachineContainerBlockData{
        public Burner burner;
    }
}