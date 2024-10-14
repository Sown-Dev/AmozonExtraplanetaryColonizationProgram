using Systems.BlockUI;
using Systems.Items;
using UnityEngine;

namespace Systems.Block{
    public class InserterBlock : ContainerBlock{
        public Slot mySlot;


        public Orientation toFace;

        public DirectionSelect DirSelect;

        public IContainerBlock toExtract;
        public IContainerBlock toInsert;

        public Transform arm;
    
        [SerializeField] private SlotVisualizer slotVisualizer;
        
        public int stackSize = 2;

        protected override void Awake(){
            DirSelect = new DirectionSelect();
            base.Awake();
            //init container
            outputProperties.size = 1;
            output = new Container(outputProperties);
            mySlot = output.GetSlot(0);
            mySlot.Stacksize = stackSize;
        
            slotVisualizer.SetSlot(output.GetSlot(0));
        }


        private float angle;

        private float extractTimer = 0;

        public override void Init( Orientation orientation){
            base.Init( orientation);
            DirSelect.inputOrientation = rotation;
            DirSelect.outputOrientation = rotation.GetOpposite();

            angle = rotation.GetAngle();
            arm.localRotation = Quaternion.Euler(0, 0, angle);
        }

        public override void Tick(){
            slotVisualizer.Refresh();
            if (toFace.GetAngle() != angle){
                angle += toFace.GetAngle() > angle ? 5 : -5;
            }        

            arm.localRotation = Quaternion.Euler(0, 0, angle);
    
        
            if(DirSelect.inputOrientation == DirSelect.outputOrientation){
                return;
            }

            if (mySlot.IsEmpty()){
                //Move to input
                toFace = DirSelect.inputOrientation;
                if (arm.localRotation.eulerAngles.z < DirSelect.inputOrientation.GetAngle() + 5 &&
                    arm.localRotation.eulerAngles.z > DirSelect.inputOrientation.GetAngle() - 5){
                    extractTimer++;
                    Block b = TerrainManager.Instance.GetBlock(Vector2Int.RoundToInt((Vector2)transform.position + DirSelect.inputOrientation.GetVector()));

                    if (extractTimer > 6){
                        extractTimer = 0;
                        toExtract = b as IContainerBlock;
                        
                        CU.Transfer(toExtract, this);
                    }
                }
            }
            else{
                //Move to output
                toFace = DirSelect.outputOrientation;
                if (arm.localRotation.eulerAngles.z < DirSelect.outputOrientation.GetAngle() + 5 &&
                    arm.localRotation.eulerAngles.z > DirSelect.outputOrientation.GetAngle() - 5){
                    
                    GetDirection( DirSelect.outputOrientation)?.Actuate();

                    toInsert = GetDirection( DirSelect.outputOrientation) as IContainerBlock;
                    if (toInsert != null){
                        CU.Transfer(mySlot, toInsert);

                    }

                }
            }
            if (mySlot.dirty){
                slotVisualizer.Refresh();
                mySlot.dirty = false;
            }  
        }

        public override string GetDescription(){
            return properties.description + " Holds up to " + stackSize + " items";
        }
    }
}