using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI{
    public class GridBatteryUI : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler{

        private IPowerBattery myBat;
        public ItemStackUI itemStackUI;

        //public TMP_Text nameText;
        public TMP_Text storedText;
        public Image icon;
        public void Init(IPowerBattery bat){
            itemStackUI.Init(bat.myBlock.properties.myItem);
            myBat = bat;
            
            //nameText.text = myBat.myBlock.properties.name;
            storedText.text =  myBat.storedPower.ToString("F1") + "Wm";
            //icon.sprite = myBat.myBlock.properties.myItem.icon;

        }
        void Update(){
            storedText.text =  myBat.storedPower.ToString("F1") + "Wm";
        }
        
        public void OnPointerEnter(PointerEventData eventData){
            TooltipManager.Instance.ShowCameraTooltip(transform.position, myBat.myBlock.transform.position);

        }
        public void OnPointerExit(PointerEventData eventData){
            TooltipManager.Instance.HideCameraTooltip();
        }


    }
}