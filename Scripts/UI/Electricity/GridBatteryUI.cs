using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class GridBatteryUI : MonoBehaviour{

        private IPowerBattery myBat;
            
        public TMP_Text nameText;
        public TMP_Text storedText;
        public Image icon;
        public void Init(IPowerBattery bat){
            myBat = bat;
            
            nameText.text = myBat.myBlock.properties.name;
            storedText.text = myBat.storedPower + "W ";
            icon.sprite = myBat.myBlock.properties.myItem.icon;

        }
        void Update(){
            storedText.text = myBat.storedPower + "W ";
        }

    }
}