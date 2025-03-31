using System;
using System.Collections.Generic;
using Systems.Items;
using Unity.VisualScripting;
using UnityEngine;

namespace Systems{
    public abstract class Unit: MonoBehaviour{
        
        [Header("Unit Fields")] [SerializeField]
        private List<UpgradeSO>
            BaseUpgrades; // we use scriptable object so we can assign them in the inspector, and then have them be added to the player on awake;

        public List<Upgrade> upgrades= new List<Upgrade>();

        public SpriteRenderer sr;
        public Rigidbody2D rb;
        public AudioSource audioSource;


        
        private Container inventory;

        [DoNotSerialize] 
        public Container Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }
        public ContainerProperties InventoryProperties;


        protected virtual void Awake(){
            Inventory = new Container(InventoryProperties);
            
            foreach (UpgradeSO upgrade in BaseUpgrades){
                AddUpgrade(upgrade.u, recalculate: false);
            }
            
            CalculateStats();

        }
        
        public virtual void AddUpgrade(Upgrade upgrade, bool recalculate = true){
            upgrade.Init(this);
            upgrades.Add(upgrade);
            if(recalculate)
                CalculateStats();
        }

        // ---------------------------------------------------------- STATS --------------------------------------------
        public Stats finalStats;
        public Stats baseStats;
        public virtual Stats CalculateStats(){
            finalStats =new Stats(0);
            finalStats.Combine(baseStats); //Adds the base stats to the final stats
            foreach (Upgrade u in upgrades){
                finalStats.Combine(u.stats); //Adds the stats of every upgrade to the final stats
            }

            //rb.mass = finalStats[Statstype.Mass];
            //rb.drag = finalStats[Statstype.Drag];
            Inventory.SetSize(Mathf.RoundToInt( finalStats[Statstype.InventorySlots]));

            return finalStats; // this isn't actually going to be used most of the time, but it's good to have
        }
    }
}