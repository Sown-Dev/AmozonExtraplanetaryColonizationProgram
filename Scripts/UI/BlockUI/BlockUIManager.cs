using Systems.Block;
using Systems.Items;
using UnityEngine;

namespace UI.BlockUI{
    public class BlockUIManager: MonoBehaviour{
        
        public static BlockUIManager Instance;
        
        [SerializeField] private BlockUI blockUIPrefab;
        
        [HideInInspector]public BlockUI currentBlockUI;
        
        private void Awake(){
            Instance = this;
        }

        private void Update(){
            if(Input.GetKeyDown(KeyCode.Escape)){
                if (currentBlockUI != null){
                    Destroy(currentBlockUI.gameObject);
                    currentBlockUI = null;
                }
            }
        }
        Vector3 lastPos = Vector3.zero; 
        public void GenerateBlockUI(Block b){

            if (b == currentBlockUI?.block){
                CloseBlockUI();
                return;
            }
            
            CloseBlockUI();
            
            currentBlockUI = WindowManager.Instance.CreateWindow(blockUIPrefab, lastPos).GetComponent<BlockUI>();
            
            
            
            currentBlockUI.block = b;

        }
        public void GenerateContainerUI(Container container){
            CloseBlockUI();
            //todo
        }
        
        public void CloseBlockUI(){
            
            
            if (currentBlockUI != null){
                lastPos = currentBlockUI.transform.localPosition;
                currentBlockUI.block.OnUIClose();
                Destroy(currentBlockUI.gameObject); 
                currentBlockUI = null;
            }
        }
        
        
    }
}