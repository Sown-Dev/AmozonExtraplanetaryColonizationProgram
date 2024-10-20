using System;
using Systems.Items;
using UnityEngine;

namespace Crafting{
    [Serializable]
    public class ItemWrapper{

         public bool isTag;//{
            //get{ return items != null && items.Length > 1; }
        //} 
        public Item[] items = new Item[1];

        
        [ConditionalField("isTag")]public string name;
        [ConditionalField("isTag")]public Sprite icon;
        
        public ItemWrapper(Item item){
            items = new Item[1];
            items[0] = item;
        }
        
        public Sprite GetIcon(){
            return isTag ? icon : items[0].icon;
        }
        
    }
}