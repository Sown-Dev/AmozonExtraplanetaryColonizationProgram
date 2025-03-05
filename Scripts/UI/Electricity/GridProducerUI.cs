using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI{
    public class GridProducerUI : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler{
        
        private IPowerProducer myProducer;
            
        public TMP_Text nameText;
        public TMP_Text powerText;
        public Image icon;
        public void Init(IPowerProducer prod){
            myProducer = prod;
            nameText.text = myProducer.myBlock.properties.name;
            powerText.text = myProducer.producing + "W ";
            icon.sprite = myProducer.myBlock.properties.myItem.icon;



        }

        void Update(){
            powerText.text = myProducer.producing + "W ";

        }
        
        public void OnPointerEnter(PointerEventData eventData){
            TooltipManager.Instance.ShowCameraTooltip(transform.position, myProducer.myBlock.transform.position);

        }
        public void OnPointerExit(PointerEventData eventData){
            TooltipManager.Instance.HideCameraTooltip();
        }

    }
}