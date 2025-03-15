using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Items{
    public class ItemDrop: MonoBehaviour{
        public ItemStack myItem;

        [SerializeField] private SpriteRenderer sr;
        
        
        public void Init(ItemStack item){
            myItem = item;
            sr.sprite = item.item.icon;
            transform.localPosition += new Vector3(Random.Range(-0.125f, 0.125f), Random.Range(-0.125f, 0.125f), 0);
        }

        private void OnCollisionEnter2D(Collision2D other){
            if( myItem.amount == 0) return;
            if(other.gameObject.CompareTag("ItemDrop")){
                ItemStack otherItem = other.gameObject.GetComponent<ItemDrop>().myItem;
                if (otherItem.item == myItem.item){
                    myItem.amount += otherItem.amount;
                    otherItem.amount = 0;
                    transform.position = (transform.position + other.transform.position) / 2;
                    Destroy(other.gameObject);
                   
                }
              
            }
        }

        public virtual ItemStack Collect(){
            return myItem;
        }
        
    }
}