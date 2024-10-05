using Systems.Items;
using Systems.Round;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class UpgradeButton: MonoBehaviour{
        public UpgradeOffer myOffer;
        public TMP_Text priceText;
        public Image UpgradeImage;
        public Button buyButton;
        public GameObject SoldOut;
        
        public void Init(UpgradeOffer upgrade){
            myOffer = upgrade;
            Refresh();
            //ui.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
        }
        
        void Refresh(){
            priceText.text = "$" + myOffer.price;
            UpgradeImage.sprite = myOffer.upgrade.u.icon;
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