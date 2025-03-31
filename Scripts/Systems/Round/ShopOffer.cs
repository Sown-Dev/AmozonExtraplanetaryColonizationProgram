using System;
using Newtonsoft.Json;
using Systems.Items;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Systems.Round{
    [Serializable]
    public class ShopOffer{
        public string itemID = null;

        [FormerlySerializedAs("item")] [JsonIgnore]
        public Item itemSO;

        [JsonIgnore]
        public Item item{
            get{
                if (itemSO != null && String.IsNullOrEmpty(itemID))
                    itemID = itemSO.name;
                if (ItemManager.Instance)
                    return ItemManager.Instance.GetItemByID(itemID);
                else{
                    return itemSO;
                }
            }
            set => itemID = ItemManager.Instance.GetItemID(value);
        }

        public int price;
        public int tier;

        public int stock;

        public bool overrideCategory;
        public BlockCategory blockCategory;

        public ShopOffer(){ }

        public ShopOffer(Item item, int price, int tier, int stock){
            this.item = item;
            this.price = price;
            this.tier = tier;
            this.stock = stock;
        }


        public ShopOffer(ShopOffer old, int addPrice, int addStock = 0){
            this.item = old.item;
            this.price = old.price + addPrice;
            this.tier = old.tier;
            stock = old.stock + addStock;
        }
    }

    [Serializable]
    public class UpgradeOffer{
        public Upgrade upgrade;
        public int price;
        public bool bought;
    
        public UpgradeOffer(){}
        
        public UpgradeOffer(Upgrade upgrade, int price){
            this.upgrade = upgrade;
            //round price to nearest 5
            this.price = price / 5 * 5;
            bought = false;
        }
    }
}