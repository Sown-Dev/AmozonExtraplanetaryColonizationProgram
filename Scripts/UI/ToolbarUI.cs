using System;
using UnityEngine;

namespace UI{
    public class ToolbarUI:MonoBehaviour{
        public PlayerUI playerUI;
        
        public SlotUI[] slots;
        public Transform list;

        private void Start(){
            Populate();
        }

        public void Populate(){
            slots = new SlotUI[playerUI.toolbarSize];

            foreach (Transform child in list){
                Destroy(child.gameObject);
            }
            for(int i = 0; i < playerUI.toolbarSize; i++){
                SlotUI slotUI = Instantiate(Utils.Instance.SlotUIPrefab, list).GetComponent<SlotUI>();
                slotUI.Slot = playerUI.toolbar[i];
                slots[i] = slotUI;
                slotUI.Refresh();
            }
            
        }
    }
}