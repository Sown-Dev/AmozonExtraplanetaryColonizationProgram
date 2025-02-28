using Systems.Items;
using Systems.Round;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI{
    public class UpgradeButton: MonoBehaviour{
        public UpgradeOffer myOffer;
        public TMP_Text priceText;

        public UpgradeUI UpgradeUI;
        public Button buyButton;
        public GameObject SoldOut;
        
        public void Init(UpgradeOffer offer){
            myOffer = offer;
            Refresh();
            //ui.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            UpgradeUI.Init(offer.upgrade);
        }
        
        void Refresh(){
            priceText.text = "$" + myOffer.price;
            buyButton.interactable = !myOffer.bought;
            SoldOut.SetActive(myOffer.bought);
        }
        
        public void Buy(){

            if(RoundManager.Instance.SpendMoney( myOffer.price) && !myOffer.bought){
                

                Player.Instance.AddUpgrade((Upgrade) myOffer.upgrade.u.Clone());
                myOffer.bought  = true;
                Refresh();

            }
            
        }

      
    }
}