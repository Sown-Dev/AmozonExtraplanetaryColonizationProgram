using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class GridProducerUI : MonoBehaviour{
        
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
    }
}