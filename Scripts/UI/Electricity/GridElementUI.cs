using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI{
    public class GridElementUI : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler{
        
        private IPowerBlock myBlock;
            
        public TMP_Text nameText;
        public TMP_Text powerText;
        public Image icon;
        public void Init(IPowerBlock block){
            myBlock = block;
            icon.sprite = myBlock.myBlock.properties.myItem.icon;



        }

        void Update(){
           // powerText.text = myProducer.producing + "W ";

        }
        
        public void OnPointerEnter(PointerEventData eventData){
            TooltipManager.Instance.ShowCameraTooltip(transform.position, myBlock.myBlock.transform.position);

        }
        public void OnPointerExit(PointerEventData eventData){
            TooltipManager.Instance.HideCameraTooltip();
        }

    }
}