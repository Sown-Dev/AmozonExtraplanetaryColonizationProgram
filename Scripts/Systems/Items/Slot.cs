using System;
using System.ComponentModel;
using UnityEngine;

namespace Systems.Items{
    public class Slot{
        public ItemStack ItemStack;

        public Item filter = null;
        public int Stacksize = 64;

        public bool Selected;
        public Action OnChange;

        //Temp solution for rendering performance: when changing, we mark the slot as dirty, and when we render we clean, so we only render when we change the slot
        public bool
            dirty = true; //true by default so we atleast render once. worst case we render once more than needed on awake

        public Slot(){
            ItemStack = null;
            //OnChange += ()=>{dirty = true;};
        }

        public Slot(ItemStack i = null, int _Stacksize = 64){
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

        public void Combine(ref ItemStack other, bool simulate = false){
            if (other == null) return;

            int size = Mathf.Min(Stacksize, ItemStack?.item.stackSize ?? 512);
            int myAmount = ItemStack?.amount ?? 0;

            if (!simulate){
                ItemStack.amount = Mathf.Min(size, other.amount + myAmount);
                other.amount -= ItemStack.amount - myAmount;

                if (other.amount <= 0){
                    other = null;
                }

                OnChange?.Invoke();
            }
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
            if (filter != null && filter != other.ItemStack.item){
                Debug.Log("Can't accept due to filter mismatch.");
                return false;
            }

            return true;
        }

        public virtual bool CanAccept(ItemStack other, bool ignoreStackSize = true){
            if (other == null) return true;

            // Filter check
            if (filter != null && filter != other.item){
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

        public bool Decrement(){
            if (ItemStack == null) return false;
            OnChange?.Invoke();
            ItemStack.amount--;
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