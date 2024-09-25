using System;
using Systems.Items;
using UnityEngine;

namespace Systems{
    public abstract class Unit: MonoBehaviour{
        public SpriteRenderer sr;
        public Rigidbody2D rb;
        public AudioSource audioSource;

        public int coins;

        public Container Inventory;
        public ContainerProperties InventoryProperties;


        protected virtual void Awake(){
            Inventory = new Container(InventoryProperties);

        }
    }
}