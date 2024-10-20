using UnityEngine;

namespace Systems.Items{
    [CreateAssetMenu(fileName = "ItemBase", menuName = "ScriptableObjects/Items/Item", order = 0)]
    public class Item: ScriptableObject{
        public int stackSize= 32;
        public Sprite icon;
        public ItemCategory category;
        public bool useCursor;

        public int value;
        public int tier;

        public int fuelValue = 0;
        
        public virtual void Use(Vector2Int pos, Unit user, Slot slot){
        }
        
         [TextArea(3,5)]public string description;

         public virtual string ToString(){
             return description;
         }
         
         


    }
    public enum ItemCategory{
        Material=1,
        Building=2,
        Misc=3,
    }
}