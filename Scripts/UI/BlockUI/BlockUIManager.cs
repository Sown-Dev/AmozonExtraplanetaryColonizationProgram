using Systems.Block;
using Systems.Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.BlockUI{
    public class BlockUIManager : MonoBehaviour{
        public static BlockUIManager Instance;

        [FormerlySerializedAs("blockUIPrefab")] [SerializeField]
        private GeneratedBlockUI generatedBlockUIPrefab;

        [HideInInspector] public BlockUI currentBlockUI;

        private void Awake(){
            Instance = this;
        }

        private void Update(){
            if (Input.GetKeyDown(KeyCode.Escape)){
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

            if (b.properties.customUI){
                currentBlockUI = WindowManager.Instance.CreateWindow(b.properties.customUI, lastPos).GetComponent<BlockUI>();
            }
            else{
                currentBlockUI = WindowManager.Instance.CreateWindow(generatedBlockUIPrefab, lastPos).GetComponent<GeneratedBlockUI>();
            }

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