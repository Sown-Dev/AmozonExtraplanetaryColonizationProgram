using UnityEngine;

namespace Systems.Items{
    public class ItemDrop: MonoBehaviour{
        public ItemStack myItem;

        [SerializeField] private SpriteRenderer sr;
        
        
        public void Init(ItemStack item){
            myItem = item;
            sr.sprite = item.item.icon;
        }

        public virtual ItemStack Collect(){
            return myItem;
        }
        
    }
}