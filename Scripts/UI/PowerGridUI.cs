using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class PowerGridUI : UIWindow{
        private PowerGrid myGrid;
    
        public GameObject consumerUIPrefab;
        public GameObject producerUIPrefab;

        public VerticalLayoutGroup consumerList;
        public VerticalLayoutGroup producerList;

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
                }
            }
        
        
        }
    }
}
