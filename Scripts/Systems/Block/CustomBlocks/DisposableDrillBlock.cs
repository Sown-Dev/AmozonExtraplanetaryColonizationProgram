using System.Text;

namespace Systems.Block.CustomBlocks{
    public class DisposableDrillBlock:DrillBlock{
    
        
        public int Durability = 100;

        public override void Drill(){
            if (Durability < 0){
                TerrainManager.Instance.RemoveBlock(data.origin);    
                return;
            }
            base.Drill();
            Durability--;
        }

        public override bool BlockDestroy(bool dropItems = true){
            //remove self from drops    
            data.lootTable = data.lootTable.FindAll(i => i.item != properties.myItem); 
            
            return base.BlockDestroy(dropItems);
        }


        public override StringBuilder GetDescription(){
            return base.GetDescription().Append("\nDurability: ").Append(Durability);
        }
    }
}