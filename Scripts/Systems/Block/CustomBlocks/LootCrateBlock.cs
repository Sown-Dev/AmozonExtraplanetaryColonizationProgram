using System.Collections.Generic;
using Systems.Items;

namespace Systems.Block.CustomBlocks{
    public class LootCrateBlock: Block,IContainerBlock{
        public List<ItemStack> loot;
        
        //generation logic
        public override void Use(Unit user){
            //base.Use(user)
            BlockDestroy(false);
            //ui 
            foreach (ItemStack s in loot){
                user.Inventory.Insert(s);
            }
        }

        public bool Insert(ref ItemStack s, bool simulate = false){
            if(!simulate && loot?.Count<32){ //dont want it to be infinite storage
                loot.Add(s);
                return true;
            }

            return false;
        }
        //Can't extract
        public ItemStack Extract(){
            return null;
        }
    }
}