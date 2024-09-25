using System;
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

            foreach (ShopOffer offer in RoundManager.Instance.shopList){
                if (offer.stock <= 0)
                    continue;
                ShopButton button = Instantiate(shopItemPrefab, layoutGroup.transform).GetComponent<ShopButton>();
                button.Init(offer);
            }
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