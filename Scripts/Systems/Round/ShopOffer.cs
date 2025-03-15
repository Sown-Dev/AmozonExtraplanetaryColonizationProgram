using System;
using Systems.Items;
using Random = UnityEngine.Random;

namespace Systems.Round{
    [Serializable]
    public class ShopOffer{
        public Item item;
        public int price;
        public int tier;

        public int stock;

        public bool overrideCategory;
        public BlockCategory blockCategory;


        public ShopOffer(Item item, int price, int tier, int stock){
            this.item = item;
            this.price = price;
            this.tier = tier;
            this.stock = stock;
        }


        public ShopOffer(ShopOffer old, int addPrice, int addStock=0){
            this.item = old.item;
            this.price = old.price + addPrice;
            this.tier = old.tier;
            stock = old.stock + addStock;
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
}