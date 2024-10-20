using System;
using Systems.Items;
using UI.BlockUI;
using Unity.VisualScripting;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ActuatedMoverBlock : ContainerBlock{
        public BlockUIButton button;

        [SerializeField] private SlotVisualizer SlotVisualizer;

        public int StackSize = 8;
        private Vector2 slotPos;
        bool ExtractedItemThisTick = false; 

        protected override void Awake(){
            base.Awake();
            SlotVisualizer.SetSlot(output.GetSlot(0));
            output.GetSlot(0).Stacksize = StackSize;
            button = new BlockUIButton(Click, null);
            slotPos = SlotVisualizer.transform.position;
        }

        public override void Tick(){
            base.Tick();
            SlotVisualizer.Refresh();
            SlotVisualizer.transform.position = Vector2.Lerp(SlotVisualizer.transform.position, slotPos, 0.2f);
            ExtractedItemThisTick = false;
        }


        public void Animate(Vector2 inputPos){
            //if(ExtractedItemThisTick)
            SlotVisualizer.transform.position =inputPos;
            SlotVisualizer.transform.position = Vector2.Lerp(SlotVisualizer.transform.position, slotPos, 0.2f);
        }


        //transfer items from input to container, then from container to output
        public void Click(){
            Block prevBlock = TerrainManager.Instance.GetBlock(origin + rotation.GetOpposite().GetVectorInt());
            Block nextBlock = TerrainManager.Instance.GetBlock(origin + rotation.GetVectorInt());


            if (nextBlock != null){
                //activate next first
                try{
                    nextBlock.Actuate();
                }
                catch (Exception e){
                    Debug.LogError(e);
                }


                //insert second

                if (nextBlock is IContainerBlock nextCon)
                    if (CU.Transfer(this, nextCon)){
                        if (nextBlock is ActuatedMoverBlock nextActuatedMoverBlock){
                            nextActuatedMoverBlock.Animate(origin);
                        }
                    }
            }

            if (prevBlock is not ActuatedMoverBlock){
                if (prevBlock is IContainerBlock prevCon){
                    bool t = CU.Transfer(prevCon, this);
                    if (prevBlock is not ActuatedMoverBlock actuatedMoverBlock && t){
                        Animate(rotation.GetVector2() + origin);
                    }
                }
            }

            //activate prev last
            try{
                prevBlock?.Actuate();
            }
            catch (Exception e){
                Debug.LogError(e);
            }
        }
    }
}