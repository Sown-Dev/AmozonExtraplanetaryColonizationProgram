using Systems.Items;
using Systems.Round;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI{
    public class ShopButton: MonoBehaviour,IPointerEnterHandler, IPointerExitHandler{
        public ShopOffer myOffer;
        

        public Transform ItemIconTransform;
        public GameObject ItemIconPrefab;
        public Transform[] BlockParent;

        public TMP_Text price;
        public TMP_Text stock;

        public Image BlockImg;
        
        public Button buyButton;
        
        public void Init(ShopOffer offer){
            myOffer = offer;
            if (myOffer.item is BlockItem b){
                BlockImg.sprite =b.blockPrefab.sr.sprite;
                
                BlockImg.SetNativeSize();
                if(BlockImg.rectTransform.sizeDelta.x > 64 || BlockImg.rectTransform.sizeDelta.y > 64){
                    foreach (var t1 in BlockParent){
                        t1.localScale = new Vector3(0.5f, 0.5f, 1);
                    }
                }else{
                    foreach (var t1 in BlockParent){
                        t1.localScale = new Vector3(1, 1, 1);
                    }
                }
            }
            else{
                BlockImg.sprite = Utils.Instance.blankIcon;
                ItemStackUI ui = Instantiate(ItemIconPrefab, ItemIconTransform).GetComponent<ItemStackUI>();
                ui.Init(myOffer.item);
            }

            
            Refresh();
        }
        
        void Refresh(){
            price.text = "$" + myOffer.price;
            stock.text = "x" + myOffer.stock;
            buyButton.interactable = myOffer.stock > 0;
        }
        
        public void Buy(){
            Refresh();

            if(RoundManager.Instance.SpendMoney(myOffer.price) && !Player.Instance.Inventory.isFull() && myOffer.stock > 0){
                myOffer.stock -= 1;
                
                Debug.Log("Bought " + myOffer.stock);
                stock.text = "x" + myOffer.stock;
                Debug.Log( stock.text);

                var itemStack = new ItemStack(myOffer.item, 1);
                Player.Instance.Inventory.Insert(ref itemStack);
                //myOffer.price += 1;
            }
            
            
            
        }

        public void OnPointerEnter(PointerEventData eventData){
            TooltipManager.Instance.ShowItem(myOffer.item, transform.position+new Vector3(0, -32, 0),gameObject);
        }

        public void OnPointerExit(PointerEventData eventData){
            TooltipManager.Instance.Hide();
        }
    }
}