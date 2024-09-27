using System;
using System.Collections.Generic;
using Systems.Items;
using Systems.Round;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI{
    public class ShopUI : UIWindow{
        public static ShopUI Instance;
        public LayoutGroup layoutGroup;

        public GameObject shopItemPrefab;
        public GameObject shopTierPrefab;

        public override void Awake(){
            base.Awake();
            Instance = this;
        }

        void Start(){
            Hide();
        }

        public override void Refresh(){
            base.Refresh();
            foreach (Transform child in layoutGroup.transform){
                Destroy(child.gameObject);
            }

            foreach (ShopTier t in RoundManager.Instance.shopTiers){
                GameObject tier = Instantiate(shopTierPrefab, layoutGroup.transform);
                tier.GetComponent<TierShopUI>().Init(t);
                
            }
        }
    }

    public class ShopTier{
        int tier;
        public ShopOffer[] logistics;
        public ShopOffer[] refinement;
        public ShopOffer[] storage;
        
        
        public ShopTier(ShopOffer[] logistics, ShopOffer[] refinement, ShopOffer[] storage, int _tier){
            this.logistics = logistics;
            this.refinement = refinement;
            this.storage = storage;
            tier = _tier;
        }
        


    }

    [Serializable]
    public class ShopOffer{
        public Item item;
        public int price;
        public int tier;

        public int stock;
        
        

        public ShopOffer(Item item, int price, int tier, int stock){
            this.item = item;
            this.price = price;
            this.tier = tier;
            this.stock = stock;
        }


        public ShopOffer(ShopOffer old, int addPrice){
            this.item = old.item;
            this.price = old.price + addPrice;
            this.tier = old.tier;
            stock = Random.Range(3, 9);
        }
    }
}