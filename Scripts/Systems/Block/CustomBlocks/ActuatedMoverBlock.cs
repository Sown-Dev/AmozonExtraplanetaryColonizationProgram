using System;
using System.Collections.Generic;
using Systems.Items;
using UI.BlockUI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Block{
    public class ActuatedMoverBlock : ContainerBlock{
        public BlockUIButton button;

        [FormerlySerializedAs("SlotVisualizer")] [SerializeField]
        private SlotVisualizer InputSlotVis;

        [SerializeField] private ItemStackVisualizer OutputSlotVis;

        public Vector2 ToOutput;


        public int StackSize = 8;
        private Vector2 slotPos;
        bool ExtractedItemThisTick = false;

        protected override void Awake(){
            base.Awake();
            InputSlotVis.SetSlot(output.GetSlot(0));
            output.GetSlot(0).Stacksize = StackSize;
            button = new BlockUIButton(Click, null);
            slotPos = InputSlotVis.transform.position;
        }

        public override void Tick(){
            base.Tick();
            InputSlotVis.Refresh();
            OutputSlotVis.Refresh();

            InputSlotVis.transform.position = Vector2.Lerp(InputSlotVis.transform.position, slotPos, 0.25f);
            ExtractedItemThisTick = false;

            if (Vector2.Distance(OutputSlotVis.transform.position, ToOutput) < 0.15f){
                OutputSlotVis.SetItemStack(null);
            }
            else{
                OutputSlotVis.transform.position = Vector3.Lerp(OutputSlotVis.transform.position, ToOutput, 0.25f);
            }
        }


        public void Animate(Vector2 inputPos){
            //if(ExtractedItemThisTick)
            //Debug.Log("Animating from relative pos: " + (inputPos-(Vector2)transform.position).ToString());
            InputSlotVis.transform.position = inputPos;
        }


        public void AnimateOutput(Vector2 outputPos){
            OutputSlotVis.transform.position = transform.position;
            ToOutput = outputPos;
        }


        //transfer items from input to container, then from container to output
        public void Click(){
            Block prevBlock = TerrainManager.Instance.GetBlock(origin + rotation.GetOpposite().GetVectorInt());
            Block nextBlock = TerrainManager.Instance.GetBlock(origin + rotation.GetVectorInt());


            if (nextBlock != null){
                //activate next first
                try{
                    // if (nextBlock is not ActuatedMoverBlock)
                    nextBlock.Actuate();
                }
                catch (Exception e){
                    Debug.LogError(e);
                }
            }
            //insert second

            if (nextBlock is IContainerBlock nextCon){
                ItemStack s = null;

                s = output.GetSlot(0)?.ItemStack?.Clone() ?? null;

                int amt = output.GetSlot(0).ItemStack?.amount ?? 0;
                bool t = CU.Transfer(this, nextCon);


                if (t || amt < (output.GetSlot(0).ItemStack?.amount ?? 0)){
                    Debug.Log(nextBlock.name + " inserted:" + nextBlock.GetType());

                    if (nextBlock is ActuatedMoverBlock nextActuatedMoverBlock){
                        nextActuatedMoverBlock.Animate(origin);
                    }
                    else{
                        AnimateOutput(rotation.GetVector2() + origin);

                        OutputSlotVis.SetItemStack(s);
                    }
                }
            }

            if (prevBlock is not ActuatedMoverBlock){
                if (prevBlock is IContainerBlock prevCon){
                    int amt = output.GetSlot(0).ItemStack?.amount ?? 0;
                    bool t = CU.Transfer(prevCon, this) || amt < (output.GetSlot(0).ItemStack?.amount ?? 0);
                    //Debug.Log("Transfered: " + t);
                    if (t){
                        Animate(rotation.GetOpposite().GetVector2() + origin);
                        ExtractedItemThisTick = true;
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

        public override List<TileIndicator> GetIndicators(){
            var e = base.GetIndicators();
            //add input and output indicators
            e.Add(new TileIndicator(new Vector2Int[]{ Orientation.Down.GetVectorInt() }, IndicatorType.InsertingTo));
            e.Add(new TileIndicator(new Vector2Int[]{ Orientation.Up.GetVectorInt() }, IndicatorType.ExtractingFrom));


            return e;
        }
    }
}