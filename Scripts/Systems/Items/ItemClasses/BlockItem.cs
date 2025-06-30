using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Items{
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items/BlockItem", order = 0)]
    public class BlockItem: Item{
        [Header("BlockItem Fields")]
        public Block.Block blockPrefab;
         public BlockCategory blockCategory;
        
        public override void Use(Vector2Int pos, Unit user, Slot slot){
            base.Use(pos, user,slot);
            if (TerrainManager.Instance.PlaceBlock(blockPrefab, pos,  Cursor.Instance.cursorRotation, makeSound:true)){
                slot.ItemStack.amount--;
                if (slot.ItemStack.amount <= 0){
                    slot.ItemStack = null;
                }
                
            }
        }

        public override string ToString(){
            return blockPrefab.properties.description;
        }

        private void OnValidate(){
            if(icon == null){
                icon = blockPrefab.sr.sprite;
            }
        }
    }
}
[Flags][Serializable]
public enum BlockCategory{
    Other=0,
    Logistics=1,//moving stuff, including both inserting and extracting
    Production=2, //Creates things, including drills
    Refining=4, //crafting/refining
    Storage=8, //stores items
    Electrical=16, //power generation and distribution
    
    
}