using Systems.Items;
using Systems.Round;
using TMPro;
using UnityEngine;

namespace UI{
    public class ShopButton: MonoBehaviour{
        public ShopOffer myOffer;
        

        public Transform ItemIconTransform;
        public TMP_Text price;
        public TMP_Text stock;

        public GameObject ItemIconPrefab;
        
        public void Init(ShopOffer offer){
            myOffer = offer;
            ItemStackUI ui = Instantiate(ItemIconPrefab, ItemIconTransform).GetComponent<ItemStackUI>();
            ui.Init(myOffer.item);
            Refresh();
            //ui.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
        }
        
        void Refresh(){
            price.text = "$" + myOffer.price;
            stock.text = "x" + myOffer.stock;
        }
        
        public void Buy(){
            Refresh();

            if(RoundManager.Instance.SpendMoney(myOffer.price) && !Player.Instance.Inventory.isFull()){
                myOffer.stock -= 1;
                
                Debug.Log("Bought " + myOffer.stock);
                stock.text = "x" + myOffer.stock;
                Debug.Log( stock.text);

                var itemStack = new ItemStack(myOffer.item, 1);
                Player.Instance.Inventory.Insert(ref itemStack);
                //myOffer.price += 1;
            }
            
            
            if(myOffer.stock <= 0){
                Destroy(gameObject);
            }
        }
    }
}