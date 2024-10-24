using UnityEngine;

namespace Systems.Items{
    public class ItemDrop: MonoBehaviour{
        public ItemStack myItem;

        [SerializeField] private SpriteRenderer sr;
        
        
        public void Init(ItemStack item){
            myItem = item;
            sr.sprite = item.item.icon;
            transform.localPosition += new Vector3(Random.Range(-0.125f, 0.125f), Random.Range(-0.125f, 0.125f), 0);
        }

        public virtual ItemStack Collect(){
            return myItem;
        }
        
    }
}