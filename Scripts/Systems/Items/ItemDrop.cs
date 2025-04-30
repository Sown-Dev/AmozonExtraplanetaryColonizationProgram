using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Items{
    public class ItemDrop : MonoBehaviour{
        public ItemStack myItem;


        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D col;
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Animator am;
        [SerializeField] private AudioSource audiosource;

        public bool finishedAnimating = false;
        public bool enablePickup = true;

        public void Awake(){
            StartCoroutine(soundDelay());
        }

        public IEnumerator soundDelay(){
            yield return new WaitForSeconds(Random.Range(0, 0.02f));
            audiosource.Play();
        }

        public void Init(ItemStack item){
            myItem = item;
            sr.sprite = item.item.icon;
            transform.localPosition += new Vector3(Random.Range(-0.125f, 0.125f), Random.Range(-0.125f, 0.125f), 0);
        }

        private void Update(){ }

        private void OnCollisionEnter2D(Collision2D other){
            if (myItem.amount == 0) return;
            if (other.gameObject.CompareTag("ItemDrop")){
                ItemStack otherItem = other.gameObject.GetComponent<ItemDrop>().myItem;
                ItemDrop otherDrop = other.gameObject.GetComponent<ItemDrop>();
                if (otherDrop.enablePickup && otherItem.item == myItem.item){
                    if (finishedAnimating){
                        if (otherItem.item == myItem.item){
                            myItem.amount += otherItem.amount;
                            otherItem.amount = 0;
                            transform.position = (transform.position + other.transform.position) / 2;
                            Destroy(other.gameObject);
                        }
                    }
                    else{
                        otherDrop.GoToDrop(this);
                        StartCoroutine(StackWith(otherDrop));
                    }
                }
            }
        }

        //coroutine for delayed stacking (to finish animation)

        private float dur = 0.14f;

        public IEnumerator StackWith(ItemDrop other){
            enablePickup = false;
            yield return new WaitForSeconds(dur);
            myItem.amount += other.myItem.amount;
            other.myItem.amount = 0;
            transform.position = (transform.position + other.transform.position) / 2;
            Destroy(other.gameObject);
            enablePickup = true;
            am.SetTrigger("Drop");
            audiosource.Play();
        }

        public void GoToDrop(ItemDrop other){
            StartCoroutine(GoTo(other));
        }

        //gradually move to other itemdrop over 0.25 secs
        public IEnumerator GoTo(ItemDrop other){
            yield return new WaitForSeconds(dur/10);
            //disable rb and collider
            rb.simulated = false;
            col.enabled = false;
            enablePickup = false;
            //forloop to do something every for/10 increments
            for (int i = 0; i < 10; i++){
                transform.position = Vector3.Lerp(transform.position, other.transform.position, (float)i / 14);
                yield return new WaitForSeconds(dur / 10);
            }
        }


        public virtual ItemStack Collect(){
            return myItem;
        }
    }
}