﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Systems.BlockUI;
using Systems.Items;
using UnityEngine;

namespace Systems.Items{
    [Serializable]
    public class Container : IBlockUI, IContainer{
        public List<Item> filterList = new List<Item>();
        public bool blackList = true;

        public Slot[] containerList;
        public ContainerProperties properties;

        
        [JsonIgnore]
        public int Size{
            get => containerList.Length;
            private set{ } //TODO add resize
        }

        //Action<ItemStack> OnInsert;

        //TODO: jenk workaround, but i only need to worry about this if i change the other constructor or how containers work
        public Container(ContainerProperties props, List<Slot> slots){
            this.properties = props;
            containerList = slots.ToArray();
            /*if (_OnInsert != null){ //not doing this incase of memory leaks. Don't have a good way to unsubscribe
                AddOnInsert(_OnInsert);
            }*/
        }

        public Container(Container c){
            this.properties = c.properties;
            containerList = new Slot[c.containerList.Length];
            for (int i = 0; i < c.containerList.Length; i++){
                containerList[i] = c.containerList[i];
                if(String.IsNullOrEmpty(containerList[i].ItemStack.itemID)){
                    containerList[i].ItemStack = null;
                }
            }
            
            
        }

        /*public void AddOnInsert(Action<ItemStack> _OnInsert){
            OnInsert += _OnInsert;
        }*/
        [JsonConstructor]
        public Container(){
            /*properties = new ContainerProperties();
            containerList = new Slot[properties.size];
            for (int i = 0; i < properties.size; i++){
                containerList[i] = new Slot(null);
            }*/
        }

        public Container(ContainerProperties props){
            this.properties = props;

            containerList = new Slot[properties.size];
            for (int i = 0; i < properties.size; i++){
                containerList[i] = new Slot(null);
            }
        }

        public bool Insert(ItemStack s, bool simulate = false){
            return Insert(ref s, simulate);
        }

        //only return true if the whole thing was inserted
        public bool Insert(ref ItemStack s, bool simulate = false){
            if (s == null){
                return false;
            }

            // Filter check
            if (blackList){
                if (filterList.Contains(s.item)){
                    return false;
                }
            }
            else{
                if (!filterList.Contains(s.item)){
                    return false;
                }
            }

            
            //new code section
            if (!simulate){
                if (GetInsertionSlot(s).Insert(ref s)){
                    return true;
                }
            }

            for (int i = 0; i < containerList.Length; i++){
                var currentStack = containerList[i].ItemStack;

                if (!simulate){
                    // If we're not simulating and the slot is empty, directly insert the item stack

                    if (containerList[i].Insert(ref s, simulate))

                        //OnInsert?.Invoke(containerList[i].ItemStack);
                        return true;
                    continue;
                }
                else{
                    //SIMULATE TRUE

                    if (containerList[i].Insert(ref s, simulate)){
                        return true;
                    }

                    /* OLD SIMULATE CODE
                    // Calculate space available in the current stack
                    int spaceAvailable = currentStack.item.stackSize - currentStack.amount;
                    if (spaceAvailable > 0){
                        simAmount -= Math.Min(spaceAvailable, simAmount);
                    }

                    if (simAmount <= 0){
                        return true;
                    }*/
                }
            }

            return false;
        }
        
        /// <summary>
        /// Returns the next EMPTY slot for another container to be insert into
        /// </summary>
        public Slot GetInsertionSlot(ItemStack insertItem = null){
            switch (properties.type){
                case ContainerType.FIFO:
                    for (int i = 0; i < containerList.Length; i++){
                        if (containerList[i].ItemStack != null){
                            if (containerList[i].ItemStack.item == insertItem.item && containerList[i].Combine(ref insertItem, true)){
                                return containerList[i];
                            }
                        }
                    }

                    for (int i = 0; i < containerList.Length; i++){
                        if (containerList[i].ItemStack == null){
                            return containerList[i];
                        }
                    }

                    break;
                case ContainerType.LIFO:
                    for (int i = containerList.Length - 1; i >= 0; i--){
                        if (containerList[i].ItemStack != null){
                            if (containerList[i].ItemStack.item == insertItem.item && containerList[i].Combine(ref insertItem, true)){
                                return containerList[i];
                            }
                        }
                    }

                    for (int i = containerList.Length - 1; i >= 0; i--){
                        if (containerList[i].ItemStack == null){
                            return containerList[i];
                        }
                    }

                    break;
                default:
                    return null;
            }

            return null;
        }


        public ItemStack Extract(){
            Slot extractionSlot = GetExtractionSlot();
            if (extractionSlot != null){
                ItemStack ret = extractionSlot.ItemStack;
                extractionSlot.ItemStack = null;
                if (ret.amount == 0)
                    return null;
                return ret;
            }

            return null;
        }


        public bool Insert(Slot s){
            if (s.ItemStack == null){
                return false;
            }

            if (blackList){
                if (filterList.Contains(s.ItemStack.item)){
                    return false;
                }
            }
            else{
                if (!filterList.Contains(s.ItemStack.item)){
                    return false;
                }
            }

            for (int i = 0; i < containerList.Length; i++){
                if (containerList[i].Insert(s)){
                    if (s.ItemStack == null || s.ItemStack.amount == 0){
                        //OnInsert?.Invoke(s.ItemStack);
                        return true;
                    }
                }
            }


            return false;
        }

        /// <summary>
        /// Returns the next FULL slot for extraction
        /// </summary>
        public Slot GetExtractionSlot(){
            switch (properties.type){
                case ContainerType.FIFO:
                    for (int i = containerList.Length - 1; i >= 0; i--){
                        if (containerList[i].ItemStack != null){
                            containerList[i].dirty = true;
                            return containerList[i];
                        }
                    }

                    break;
                case ContainerType.LIFO:
                    for (int i = 0; i < containerList.Length; i++){
                        if (containerList[i].ItemStack != null){
                            containerList[i].dirty = true;
                            return containerList[i];
                        }
                    }

                    break;
                default:
                    return null;
            }

            return null;
        }

       

        public bool ExtractToSlot(Slot s){
            Slot extractionSlot = GetExtractionSlot();
            if (s.ItemStack == null || s.ItemStack?.item == extractionSlot.ItemStack?.item){
                return extractionSlot.Insert(s);
            }

            return false;
        }


        public bool Contains(Item item){
            for (int i = 0; i < containerList.Length; i++){
                if (containerList[i].ItemStack != null && containerList[i].ItemStack.item == item){
                    return true;
                }
            }

            return false;
        }

        public bool Contains(Slot s){
            foreach (Slot slot in containerList){
                if (slot.ItemStack != null && slot == s){
                    return true;
                }
            }

            return false;
        }

        // only compares if item is the same and stacksize is higher
        public bool Contains(ItemStack itemStack){
            for (int i = 0; i < containerList.Length; i++){
                if (containerList[i].ItemStack != null && containerList[i].ItemStack.item == itemStack.item &&
                    containerList[i].ItemStack.amount >= itemStack.amount){
                    return true;
                }
            }

            return false;
        }

        public bool Contains(List<ItemStack> itemStacks){
            if (itemStacks == null || itemStacks.Count == 0){
                return true;
            }

            foreach (ItemStack itemStack in itemStacks){
                if (!Contains(itemStack)){
                    return false;
                }
            }

            return true;
        }


        //check if item is the same then remove the item
        public bool RemoveItem(ItemStack itemStackToRemove){
            for (int i = 0; i < containerList.Length; i++){
                ItemStack currentStack = containerList[i].ItemStack;
                if (currentStack != null && currentStack.item == itemStackToRemove.item){
                    // Subtract the amount
                    int newAmount = currentStack.amount - itemStackToRemove.amount;
                    if (newAmount > 0){
                        // Update the amount if there's some left
                        currentStack.amount = newAmount;
                    }
                    else{
                        containerList[i].ItemStack = null;
                    }

                    return true; // Return true because an item was found and removed or its amount reduced
                }
            }

            return false; // Return false if the item was not found in the container
        }


        public int Priority{ get; set; }
        public bool Hidden{ get; set; }

        public Slot GetSlot(int i){
            return containerList[i];
        }

        public bool isEmpty(){
            for (int i = 0; i < containerList.Length; i++){
                if (containerList[i].ItemStack != null){
                    return false;
                }
            }

            return true;
        }

        public bool isFull(){
            for (int i = 0; i < containerList.Length; i++){
                if (containerList[i].ItemStack == null){
                    return false;
                }
            }

            return true;
        }

        public List<ItemStack> GetItems(){
            List<ItemStack> items = new List<ItemStack>();
            for (int i = 0; i < containerList.Length; i++){
                if (containerList[i].ItemStack != null){
                    items.Add(containerList[i].ItemStack);
                }
            }

            return items;
        }

        public List<ItemStack> SetSize(int size){
            if (size == containerList.Length){
                return null;
            }

            List<ItemStack> overflow = new List<ItemStack>();
            Slot[] newContainerList = new Slot[size];
            int i = 0;
            for (; i < size; i++){
                if (i < containerList.Length){
                    newContainerList[i] = containerList[i];
                }
                else{
                    newContainerList[i] = new Slot(null);
                }
            }

            for (; i < containerList.Length; i++){
                if (containerList[i].ItemStack != null){
                    overflow.Add(containerList[i].ItemStack);
                }
            }


            containerList = newContainerList;
            return overflow;
        }
    }
}
[Serializable]
public enum ContainerType{
    FIFO,
    LIFO,
}