using System;
using System.Collections.Generic;
using Systems.Items;
using UI.BlockUI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Block{
    public class ActuatedMoverBlock : ContainerBlock{
        public new ActuatedMoverBlockData data => (ActuatedMoverBlockData)base.data;

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


            output.GetSlot(0).Stacksize = StackSize;
        }

        protected override void Start(){
            base.Start();
            ExtractedItemThisTick = false;
            isOutputAnimating = false;


            button = new BlockUIButton(Click, null);

            if (InputSlotVis != null)
            {
                InputSlotVis.SetSlot(output.GetSlot(0));
                slotPos = InputSlotVis.transform.position;
            }

        }

        public override void Tick(){
            base.Tick();
            InputSlotVis?.Refresh();
            OutputSlotVis?.Refresh();

            // Always animate input back to its original position.
            if (InputSlotVis != null)
            {
                InputSlotVis.transform.position =
                    Vector2.Lerp(InputSlotVis.transform.position, slotPos, 0.25f);
            }
            ExtractedItemThisTick = false;

            // Only animate output if an animation has been initiated.
            if (isOutputAnimating && OutputSlotVis != null)
            {
                if (Vector2.Distance(OutputSlotVis.transform.position, ToOutput) < 0.15f)
                {
                    OutputSlotVis.SetItemStack(null);
                    isOutputAnimating = false;
                }
                else
                {
                    OutputSlotVis.transform.position =
                        Vector3.Lerp(OutputSlotVis.transform.position, ToOutput, 0.25f);
                }
            }
        }

        public void Animate(Vector2 inputPos){
            // Animate input slot (mover extraction)
            if (InputSlotVis != null)
                InputSlotVis.transform.position = inputPos;
        }

        // Start the output animation only when called.
        public void AnimateOutput(Vector2 outputPos){
            if (OutputSlotVis != null)
                OutputSlotVis.transform.position = transform.position;
            ToOutput = outputPos;
            isOutputAnimating = true;
        }

        // Transfer items from input to container, then from container to output.
        public void Click(){
            Block prevBlock = TerrainManager.Instance.GetBlock(data.origin + data.rotation.GetOpposite().GetVectorInt());
            Block nextBlock = TerrainManager.Instance.GetBlock(data.origin + data.rotation.GetVectorInt());

            if (nextBlock != null)
            {
                // Activate next block first.
                try
                {
                    nextBlock.Actuate();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            // Try to insert into the next container.
            if (nextBlock is IContainerBlock nextCon)
            {
                ItemStack before = output.GetSlot(0)?.ItemStack?.Clone();
                bool t = CU.Transfer(this, nextCon);

                if (t ||
                    (before?.amount ?? 0) > (output.GetSlot(0).ItemStack?.amount ?? 0))
                {
                    if (nextBlock is ActuatedMoverBlock nextActuatedMoverBlock)
                    {
                        nextActuatedMoverBlock.Animate(data.origin);
                    }
                    else
                    {
                        AnimateOutput(data.rotation.GetVector2() + data.origin);
                        OutputSlotVis?.SetItemStack(before);
                    }
                }
            }

            // Only pull from the previous container if we have room
            if (!output.isFull() && prevBlock is IContainerBlock prevCon)
            {
                ItemStack before = output.GetSlot(0).ItemStack?.Clone();
                bool t = CU.Transfer(prevCon, this);

                if (t ||
                    (before?.amount ?? 0) < (output.GetSlot(0).ItemStack?.amount ?? 0))
                {
                    Animate(data.rotation.GetOpposite().GetVector2() + data.origin);
                    ExtractedItemThisTick = true;
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
            e.Add(new TileIndicator(new Vector2Int[]{ data.rotation.GetVectorInt() + data.origin }, IndicatorType.InsertingTo));
            e.Add(new TileIndicator(new Vector2Int[]{ data.rotation.GetOpposite().GetVectorInt() + data.origin }, IndicatorType.ExtractingFrom));

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