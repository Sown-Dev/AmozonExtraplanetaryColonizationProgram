using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Items;
using UI.BlockUI;
using Unity.VisualScripting;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ConveyorBeltBlock : TickingBlock, IContainerBlock{
        public int Capacity;
        public int Speed = 1;
        private List<ConveyorSlot> ConveyorContainer;


        private List<SlotVisualizer> availableSlots;
        public SlotVisualizer prefab;

        private int slots = 5;
        protected override void Awake(){
            base.Awake();
            availableSlots = new List<SlotVisualizer>();
            ConveyorContainer = new List<ConveyorSlot>();
            for (int i = 0; i < slots; i++){
                SlotVisualizer sv = Instantiate(prefab, transform);
                availableSlots.Add(sv);
                
            }
        }
        
        
        //needs to be in start bc init (which sets rotation) happens AFTER awake
        private void Start(){
            
            //the reason the numbers are so stupid is bc 0.333 in the shader is 1 pixel. idk how it happened though prob bc base size for this is 48x48 not 16x16 which it was made for
            mat.SetVector("_AreaSize" , rotation.isVertical()? new Vector4(4,5.333333f,0,0) : new Vector4(5.333333f,4,0,0));
            mat.SetVector("_AreaOffset" , rotation.isVertical()? new Vector4(6,6,0,0) :  new Vector4(5.333333f,6.666666f,0,0));
            mat.SetVector("_Direction", rotation.GetVector() / 3);
            
        }


        public ItemStack Extract(){
            Slot extractionSlot = ConveyorContainer.Last().mySlot;

            if (extractionSlot != null){
                ItemStack ret = extractionSlot.ItemStack;
                extractionSlot.ItemStack = null;
                if (ret.amount == 0)
                    return null;

                //By this point, we 100% are gonna extract item, so remove it from conveyor
                RemoveConveyorSlot(ConveyorContainer.Last());

                return ret;
            }

            return null;
        }

        public bool Insert(ref ItemStack s, bool simulate = false){
            //check for space. need 4 slots for item
            if (availableSlots.Count <= 0){
                return false;
            }

            if (ConveyorContainer.Count > 0 && ConveyorContainer[0].distance < 4){
                return false;
            }

            //creating new slot
            if (!simulate){
                Slot slot = new Slot(null, Capacity);
                ConveyorSlot cSlot = new ConveyorSlot(slot, availableSlots[0]);
                availableSlots.Remove(cSlot.assignedSlot); //remove it after
                ConveyorContainer.Add(cSlot);
                cSlot.assignedSlot.transform.position =
                    transform.position + (-0.5f * rotation.GetOpposite().GetVector3());
                return slot.Insert(ref s);
            }
            else{
                return true;
            }
        }

        public override void Tick(){

            mat.SetFloat("_Speed", Speed * 16.3333333f);
            ConveyorContainer = ConveyorContainer.OrderBy(cs => cs.distance).ToList();
            List<ConveyorSlot> toRemove = new List<ConveyorSlot>();
            for (int i = ConveyorContainer.Count - 1; i >= 0; i--){
                ConveyorSlot cs = ConveyorContainer[i];
                if (cs.distance <= 17){
                    Vector3 direction = rotation.GetOpposite().GetVector();

                    if (i + 1 < ConveyorContainer.Count){
                        if (ConveyorContainer[i + 1].distance >= cs.distance + 4){
                            cs.distance += Speed;
                            cs.assignedSlot.transform.position = transform.position + (-0.5f * direction) +
                                                                 (direction / 16 * (cs.distance-1));
                        }
                    }
                    else{
                        cs.distance += Speed;
                        cs.assignedSlot.transform.position =
                            transform.position + (-0.5f * direction) + (direction / 16 *  (cs.distance-1));
                    }
                }
            }

            foreach (ConveyorSlot cs in ConveyorContainer){
                if (cs.distance >= 17){
                    if (GetDirection(rotation.GetOpposite()) is IContainerBlock icb){
                        //attempt to insert
                        icb.Insert(ref cs.mySlot.ItemStack);
                        //if null, remove slot
                        if (cs.mySlot.ItemStack == null){
                            toRemove.Add(cs);
                        }
                    }
                }
            }


            foreach (ConveyorSlot cs in toRemove){
                RemoveConveyorSlot(cs);
            }
        }

        public void RemoveConveyorSlot(ConveyorSlot cs){
            cs.assignedSlot.SetSlot(null);
            availableSlots.Add(cs.assignedSlot);
            ConveyorContainer.Remove(cs);
        }


        public class ConveyorSlot{
            public Slot mySlot;
            public int distance;
            public SlotVisualizer assignedSlot;
            //public bool marked_for_removal = false;

            public ConveyorSlot(Slot s, SlotVisualizer visual){
                distance = 0;
                mySlot = s;
                assignedSlot = visual;
                assignedSlot.SetSlot(mySlot);
            }
        }
    }
}