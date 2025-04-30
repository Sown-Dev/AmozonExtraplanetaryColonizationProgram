using Systems.BlockUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BlockUI{
    public class LabelUI: MonoBehaviour{
        
        public Label label;
        
        public TMP_Text labelText;
        public Image backgroundImage;

        public void Init(Label l){
            label = l;
            Update();
        }

        void Update(){
            if (label == null) return;
            labelText.text = label.text;
            labelText.color = label.textColor;
            if (label.background)
            {
                backgroundImage.enabled = true;
            }
            else
            {
                backgroundImage.enabled = false;
            }
        }
    }
}