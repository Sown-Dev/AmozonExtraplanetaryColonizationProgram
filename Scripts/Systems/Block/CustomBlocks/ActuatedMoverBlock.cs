using System;
using System.Collections.Generic;
using Systems.Items;
using UI.BlockUI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Block{
    public class ActuatedMoverBlock : ContainerBlock{
        //public new ActuatedMoverBlockData data => (ActuatedMoverBlockData)base.data;

        public BlockUIButton button;

        [FormerlySerializedAs("SlotVisualizer")] [SerializeField]
        private SlotVisualizer InputSlotVis;

        [SerializeField] private ItemStackVisualizer OutputSlotVis;


        public Vector2 ToOutput;
        public int StackSize = 8;
        private Vector2 slotPos;
        
        public bool ExtractedItemThisTick = false;
        public bool isOutputAnimating = false;


        public override void Init(Orientation orientation){
            base.Init(orientation);
            
            ExtractedItemThisTick = false;
            isOutputAnimating = false;
            InputSlotVis.SetSlot(output.GetSlot(0));
            output.GetSlot(0).Stacksize = StackSize;
            
            
            button = new BlockUIButton(Click, null);
            slotPos = InputSlotVis.transform.position;
        }

        public override void Tick(){
            base.Tick();
            InputSlotVis.Refresh();
            OutputSlotVis.Refresh();

            // Always animate input back to its original position.
            InputSlotVis.transform.position = Vector2.Lerp(InputSlotVis.transform.position, slotPos, 0.25f);
            ExtractedItemThisTick = false;

            // Only animate output if an animation has been initiated.
            if (isOutputAnimating){
                if (Vector2.Distance(OutputSlotVis.transform.position, ToOutput) < 0.15f){
                    OutputSlotVis.SetItemStack(null);
                    isOutputAnimating = false;
                }
                else{
                    OutputSlotVis.transform.position = Vector3.Lerp(OutputSlotVis.transform.position, ToOutput, 0.25f);
                }
            }
        }

        public void Animate(Vector2 inputPos){
            // Animate input slot (mover extraction)
            InputSlotVis.transform.position = inputPos;
        }

        // Start the output animation only when called.
        public void AnimateOutput(Vector2 outputPos){
            OutputSlotVis.transform.position = transform.position;
            ToOutput = outputPos;
            isOutputAnimating = true;
        }

        // Transfer items from input to container, then from container to output.
        public void Click(){
            Block prevBlock = TerrainManager.Instance.GetBlock(data.origin + data.rotation.GetOpposite().GetVectorInt());
            Block nextBlock = TerrainManager.Instance.GetBlock(data.origin + data.rotation.GetVectorInt());

            if (nextBlock != null){
                // Activate next block first.
                try{
                    nextBlock.Actuate();
                }
                catch (Exception e){
                    Debug.LogError(e);
                }
            }

            // Try to insert into the next container.
            if (nextBlock is IContainerBlock nextCon){
                ItemStack s = output.GetSlot(0)?.ItemStack?.Clone();
                int amt = output.GetSlot(0).ItemStack?.amount ?? 0;
                bool t = CU.Transfer(this, nextCon);

                if (t || amt < (output.GetSlot(0).ItemStack?.amount ?? 0)){
                    Debug.Log(nextBlock.name + " inserted:" + nextBlock.GetType());

                    if (nextBlock is ActuatedMoverBlock nextActuatedMoverBlock){
                        nextActuatedMoverBlock.Animate(data.origin);
                    }
                    else{
                        AnimateOutput(data.rotation.GetVector2() + data.origin);
                        OutputSlotVis.SetItemStack(s);
                    }
                }
            }

            if (prevBlock is not ActuatedMoverBlock){
                if (prevBlock is IContainerBlock prevCon){
                    int amt = output.GetSlot(0).ItemStack?.amount ?? 0;
                    bool t = CU.Transfer(prevCon, this) || amt < (output.GetSlot(0).ItemStack?.amount ?? 0);
                    if (t){
                        Animate(data.rotation.GetOpposite().GetVector2() + data.origin);
                        
                        ExtractedItemThisTick = true;
                    }
                }
            }

            // Activate previous block last.
            try{
                prevBlock?.Actuate();
            }
            catch (Exception e){
                Debug.LogError(e);
            }
        }

        public override List<TileIndicator> GetIndicators(){
            var e = base.GetIndicators();
            // Add input and output indicators.
            e.Add(new TileIndicator(new Vector2Int[]{ Orientation.Down.GetVectorInt() }, IndicatorType.InsertingTo));
            e.Add(new TileIndicator(new Vector2Int[]{ Orientation.Up.GetVectorInt() }, IndicatorType.ExtractingFrom));

            return e;
        }
    }
    [Serializable]
    public class ActuatedMoverBlockData : ContainerBlockData{
        public bool ExtractedItemThisTick = false;

        // Added flag to control output animation
        public bool isOutputAnimating = false;
    }
}