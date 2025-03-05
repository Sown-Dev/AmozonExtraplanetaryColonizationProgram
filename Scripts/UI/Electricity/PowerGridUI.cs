using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class PowerGridUI : UIWindow{
        private PowerGrid myGrid;
    
        public GameObject consumerUIPrefab;
        public GameObject producerUIPrefab;
        public GameObject batteryUIPrefab;

        public VerticalLayoutGroup consumerList;
        public VerticalLayoutGroup producerList;
        public VerticalLayoutGroup batteryList;

        public TMP_Text producing;
        public TMP_Text consuming;
        public TMP_Text stored;
        
        public SlicedFilledImage producingBar;
        public SlicedFilledImage storedBar;
        public SlicedFilledImage consumingBar;

        public void Init(PowerGrid g){
            myGrid = g;
        
            foreach (var block in myGrid.blocks){
                if (block is IPowerConsumer consumer){
                    GameObject consumerUI = Instantiate(consumerUIPrefab, consumerList.transform);
                    consumerUI.GetComponent<GridConsumerUI>().Init(consumer);
                }
                else if (block is IPowerProducer producer){
                    GameObject producerUI = Instantiate(producerUIPrefab, producerList.transform);
                    producerUI.GetComponent<GridProducerUI>().Init(producer);
                }else if(block is IPowerBattery battery){
                    GameObject batteryUI = Instantiate(batteryUIPrefab, batteryList.transform);
                    batteryUI.GetComponent<GridBatteryUI>().Init(battery);
                }
                else{
                    Debug.LogError("Block is not a consumer or producer");
                }
            }
        
        
        }

        private void Update(){
            
            producing.text = myGrid.producing + "W/" + myGrid.productionCapacity + "W";
            consuming.text = myGrid.consuming + "W/" + myGrid.powerNeeded + "W";
            stored.text = myGrid.storedPower + "W/" + myGrid.capacity + "W";
            
            //make bars empty if the value doesn't apply.
            producingBar.fillAmount = myGrid.producing>0?(float)myGrid.producing / myGrid.productionCapacity:0;
            storedBar.fillAmount = (myGrid.storedPower>0 && myGrid.capacity>0) ?myGrid.storedPower / myGrid.capacity : 0;
            consumingBar.fillAmount =myGrid.consuming>0 ? (float)myGrid.consuming / myGrid.powerNeeded : 0;


        }
    }
}
