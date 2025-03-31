using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Items{
    [Serializable]
    [CanBeNull]
    public class ItemStack{
        public string itemID=null;

        [FormerlySerializedAs("item")][JsonIgnore]public Item itemSO;
        [JsonIgnore]public Item item{
            get{
                if (itemSO != null && String.IsNullOrEmpty(itemID))
                    itemID = itemSO.name;
                if(ItemManager.Instance)
                    return ItemManager.Instance.GetItemByID(itemID);
                else{
                    return itemSO;
                }
            }
            set => itemID = ItemManager.Instance.GetItemID(value);
        }

        public int amount;

        /* public ItemStack(Item it, int amt){
        item = it;
        amount = amt;
        }*/
        [JsonConstructor]
        public ItemStack(){
            
        }

        public ItemStack(Item it, int amt){
            itemID = ItemManager.Instance.GetItemID(it);
            
            amount = amt;
        }


        [Obsolete]
        public bool AttemptStack(ItemStack other, int SlotSize = 256){
            if (other.amount == 0){
                return false;
            }

            int stackSize = Mathf.Min(item.stackSize, SlotSize);

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
            return "|" + item.name + ", " + amount;
        }
    }
}