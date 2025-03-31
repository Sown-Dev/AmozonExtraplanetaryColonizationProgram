using System;
using System.ComponentModel;
using Newtonsoft.Json;
using UI;
using UnityEngine;

namespace Systems.Items{
    [Serializable]
    public class Slot{
        [JsonProperty("ItemStack", NullValueHandling = NullValueHandling.Include)]
        public ItemStack ItemStack = null;

        public Filter filter = null;
        
        public int Stacksize = 1000;

        [JsonIgnore]public bool Selected;
       [JsonIgnore]public Action OnChange;

        //Temp solution for rendering performance: when changing, we mark the slot as dirty, and when we render we clean, so we only render when we change the slot
        public bool
            dirty = true; //true by default so we atleast render once. worst case we render once more than needed on awake
        
        [JsonConstructor]
        public Slot(){
            //ItemStack = null;
            //OnChange += ()=>{dirty = true;};
        }

        public Slot(ItemStack i = null, int _Stacksize = 1000){
            ItemStack = i;
            Stacksize = _Stacksize;
            //OnChange += ()=>{dirty = true;};
        }

        public ItemStack GetStack(){
            return ItemStack;
        }

        public bool Swap(Slot other){
            if (this.CanSwap(other) && other.CanSwap(this)){
                // Swap logic
                (ItemStack, other.ItemStack) = (other.ItemStack, ItemStack);
                OnChange?.Invoke();
                return true;
            }

            return false;
        }


        public bool Insert(ref ItemStack other, bool simulate = false){
            if (other == null || !CanAccept(other)) return false;

            if (simulate){
                // Simulate the insertion without modifying anything
                if (ItemStack == null){
                    // If the slot is empty, the insertion is possible
                    return true;
                }
                else{
                    // If the slot is not empty, check if the items can be combined
                    int size = Mathf.Min(Stacksize, ItemStack.item.stackSize);
                    int myAmount = ItemStack.amount;
                    int potentialAmount = myAmount + other.amount;
                    return potentialAmount <= size;
                }
            }
            else{
                // Perform the actual insertion
                if (ItemStack == null){
                    // Directly set the ItemStack if the slot is empty
                    ItemStack = other.Clone();
                    ItemStack.amount = 0; // Start with zero and combine amounts
                }

                Combine(ref other, simulate: false); // Ensure Combine does not simulate
                OnChange?.Invoke();

                // Check if the itemstack is fully inserted and set to null
                if (other != null && other.amount <= 0){
                    other = null;
                }

                return other == null;
            }
        }

        public bool Combine(ref ItemStack other, bool simulate = false)
        {
            if (other == null || other.item != ItemStack.item) return false;

            // Determine the maximum stack size for the item
            int maxStackSize = Mathf.Min(Stacksize, ItemStack?.item.stackSize ?? 512);

            // Current amount in the slot
            int myAmount = ItemStack?.amount ?? 0;

            // Amount available to transfer
            int spaceAvailable = maxStackSize - myAmount;

            // Amount that can be transferred
            int transferAmount = Mathf.Min(spaceAvailable, other.amount);

            // Check if the entire stack can be transferred
            bool fullyTransferred = transferAmount == other.amount;

            if (!simulate && transferAmount > 0)
            {
                // Perform the actual transfer
                if (ItemStack == null)
                {
                    // If the slot is empty, clone the other stack and set the amount
                    ItemStack = other.Clone();
                    ItemStack.amount = transferAmount;
                }
                else
                {
                    // Otherwise, add the transfer amount to the existing stack
                    ItemStack.amount += transferAmount;
                }

                // Reduce the other stack's amount
                other.amount -= transferAmount;

                // Set other to null if fully transferred
                if (other.amount <= 0)
                {
                    other = null;
                }

                // Notify listeners of the change
                OnChange?.Invoke();
            }

            // Return true only if the other stack is fully transferred
            return fullyTransferred;
        }
        // This method now leverages the core Insert logic for ItemStack to maintain consistency
        public bool Insert(Slot other){
            if (!CanAccept(other.ItemStack)) return false;

            if (other.ItemStack == null) return false;

            //else

            if (ItemStack == null){
                ItemStack = other.ItemStack.Clone();
                ItemStack.amount = 0;


                CombineSlots(other);


                OnChange?.Invoke();
                return true;
            }
            else{
                CombineSlots(other);

                OnChange?.Invoke();
                return true;
            }
        }


        //this will do for now. use this to insert into containers instead of their insert
        public bool ExtractToContainerBlock(IContainerBlock other){
            if (other.Insert(ref this.ItemStack)){
                OnChange?.Invoke();
                return true;
            }

            return false;
        }


        public bool ExtractToContainer(Container other){
            if (other.Insert(ref this.ItemStack)){
                OnChange?.Invoke();
                return true;
            }

            return false;
        }

        //assume we can combine, ie items are the same
        public void CombineSlots(Slot other){
            Combine(ref other.ItemStack);
            other.OnChange?.Invoke();
        }


        public virtual bool CanSwap(Slot other){
            if (other.ItemStack == null) return true;

            // Filter check
            if (filter != null && filter.filter != other.ItemStack.item){
                Debug.Log("Can't accept due to filter mismatch.");
                return false;
            }

            return true;
        }

        public virtual bool CanAccept(ItemStack other, bool ignoreStackSize = true){
            if (other == null) return true;

            // Filter check
            if (filter != null && filter.filter != other.item){
                Debug.Log("Can't accept due to filter mismatch.");
                return false;
            }

            if (ItemStack != null && ItemStack?.item != other.item){
                return false;
            }


            // Ignore stack size for swap operations
            if (ignoreStackSize)
                return true;

            // Check against stacksize if not ignoring it
            if (ItemStack == null || ItemStack.item == other.item){
                int potentialAmount = (ItemStack?.amount ?? 0) + other.amount;
                return potentialAmount <= Stacksize;
            }

            return false;
        }

        public bool Decrement(int amount =1){
            if (ItemStack == null) return false;
            OnChange?.Invoke();
            ItemStack.amount-=amount;
            if (ItemStack.amount <= 0){
                ItemStack = null;
                return true;
            }

            return true;
        }


        public bool IsEmpty(){
            if (ItemStack?.amount <= 0) ItemStack = null;
            return ItemStack == null;
        }
    }
}