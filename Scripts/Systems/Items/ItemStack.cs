using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Systems.Items{
    [Serializable]
    [CanBeNull]
    public class ItemStack{
        public Item item;
        public int amount = 1;

        public ItemStack(Item it, int amt){
            item = it;
            amount = amt;
        }
        
        
        [Obsolete]
        public bool AttemptStack(ItemStack other, int SlotSize=256){
            if(other.amount == 0){
                return false;
            }
            
            int stackSize = Mathf.Min(item.stackSize,SlotSize );
            
            if (item == other.item){
                if (amount + other.amount <= stackSize){
                    
                    amount += other.amount;
                    other.amount = 0;
                    other.item = null;
                    return true;
                }
                else{
                    int space = stackSize - amount;
                    amount += space;
                    other.amount -= space;
                    return true;
                }
            }
            return false;
        }

        public ItemStack Clone(){
            return (ItemStack)this.MemberwiseClone();
        }
        
        public override string ToString(){
            return "|"+item.name + ", " + amount;
        }
    }
}