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
        
        public ShopButton shopButton;
        public ShopOffer dynamiteOffer;

        public override void Awake(){
            base.Awake();
            Instance = this;
            DragAllow = false;
            shopButton.Init(dynamiteOffer);
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
        public int tier;
        public ShopOffer[] logistics;
        public ShopOffer[] electrical;
        public ShopOffer[] refinement;
        public ShopOffer[] production;
        public ShopOffer[] misc;

        public UpgradeOffer upgradeOffer;


        public ShopTier(ShopOffer[] _logistics, ShopOffer[] _electrical, ShopOffer[] _refinement, ShopOffer[] _production, ShopOffer[] _misc,
            UpgradeOffer _upgrade, int _tier){
            logistics = _logistics;
            electrical = _electrical;
            refinement = _refinement;
            production = _production;
            upgradeOffer = _upgrade;
            misc = _misc;
            tier = _tier;
        }
    }

    public class UpgradeOffer{
        public UpgradeSO upgrade;
        public int price;
        public bool bought;

        public UpgradeOffer(UpgradeSO upgrade, int price){
            this.upgrade = upgrade;
            //round price to nearest 5
            this.price = price / 5* 5;
            bought = false;
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
            stock = old.stock + Random.Range(1, old.stock/2) + Random.Range(-1, 1);
        }
    }
}