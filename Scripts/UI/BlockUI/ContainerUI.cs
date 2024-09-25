using System;
using System.Collections.Generic;
using Systems.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class ContainerUI : MonoBehaviour{
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private TMP_Text nameText;

        [SerializeField]
        private GameObject slotPrefab;
        
        private List<SlotUI> slotUIs;
        
        
        
        [HideInInspector] public Container containerRef;
        
        public void Init(Container c){
            containerRef = c;
            Refresh();
        }
        
        public void Refresh(){
            //Dwai this code
            gridLayoutGroup.constraintCount = containerRef.properties.scaleDownGridIfSmaller? Mathf.Min( containerRef.properties.gridWidth, containerRef.Size)  : containerRef.properties.gridWidth;
            
            nameText.text = containerRef.properties.name;
            slotUIs = new List<SlotUI>();
            foreach (Transform child in gridLayoutGroup.transform){
                Destroy(child.gameObject);
            }
            for (int i = 0; i < containerRef.Size; i++){
                SlotUI slot = Instantiate(slotPrefab, gridLayoutGroup.transform).GetComponent<SlotUI>();
                slotUIs.Add(slot);
                slot.Slot =containerRef.containerList[i];
                slot.Refresh();
            }
        }
    }
}