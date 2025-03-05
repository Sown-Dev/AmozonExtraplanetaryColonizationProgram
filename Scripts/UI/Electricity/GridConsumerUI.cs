using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI{
    public class GridConsumerUI : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler{
        
        private IPowerConsumer myConsumer;
            
        public TMP_Text nameText;
        public TMP_Text powerText;
        public Image icon;
        public void Init(IPowerConsumer consumer){
            myConsumer = consumer;
            nameText.text = myConsumer.myBlock.properties.name;
            powerText.text = myConsumer.providedPower + "W ";
            icon.sprite = myConsumer.myBlock.properties.myItem.icon;

        }

        void Update(){
            powerText.text = myConsumer.providedPower + "W ";
        }

        public void OnPointerEnter(PointerEventData eventData){
            TooltipManager.Instance.ShowCameraTooltip(transform.position, myConsumer.myBlock.transform.position);

        }
        public void OnPointerExit(PointerEventData eventData){
            TooltipManager.Instance.HideCameraTooltip();
        }


    }
}