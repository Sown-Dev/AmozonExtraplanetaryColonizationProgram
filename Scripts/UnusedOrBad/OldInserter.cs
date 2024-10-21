using System.Text;
using Systems.BlockUI;
using Systems.Items;
using UnityEngine;

namespace Systems.Block{
    public class OldInserter : ContainerBlock{
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
            DirSelect.inputOrientation = rotation.GetOpposite();
            DirSelect.outputOrientation = rotation;

            angle = rotation.GetAngle();
            arm.localRotation = Quaternion.Euler(0, 0, angle);
        }
    
        //new vars for remake:
        private int timeElapsed;
        private int movetime = 24;
        
        public override void Tick(){
            slotVisualizer.Refresh();

            float targetAngle = toFace.GetAngle();
    
            // Use Slerp to smoothly rotate the arm towards the target angle
            if (Mathf.Abs(targetAngle - angle) > 0.1f){ 
                angle = Mathf.LerpAngle(angle, targetAngle, timeElapsed / (float)movetime);
                timeElapsed = Mathf.Min(timeElapsed + 1, movetime); // increment timeElapsed up to movetime
            } else {
                timeElapsed = 0; // Reset the time if rotation is done
            }
    
            arm.localRotation = Quaternion.Euler(0, 0, angle);
    
            if (DirSelect.inputOrientation == DirSelect.outputOrientation){
                return;
            }

            if (mySlot.IsEmpty()){
                // Move to input
                toFace = DirSelect.inputOrientation;

                // If arm is close enough to input angle
                if (Mathf.Abs(arm.localRotation.eulerAngles.z - DirSelect.inputOrientation.GetAngle()) < 5){
                    extractTimer++;
                    Block b = TerrainManager.Instance.GetBlock(Vector2Int.RoundToInt((Vector2)transform.position + DirSelect.inputOrientation.GetVector2()));
            
                    if (extractTimer > 6){
                        extractTimer = 0;
                        toExtract = b as IContainerBlock;
                
                        CU.Transfer(toExtract, this);
                    }
                }
            }
            else{
                // Move to output
                toFace = DirSelect.outputOrientation;

                // If arm is close enough to output angle
                if (Mathf.Abs(arm.localRotation.eulerAngles.z - DirSelect.outputOrientation.GetAngle()) < 5){
            
                    GetDirection(DirSelect.outputOrientation)?.Actuate();
                    toInsert = GetDirection(DirSelect.outputOrientation) as IContainerBlock;
            
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


        public override StringBuilder GetDescription(){
            return base.GetDescription().Append(" Holds up to: ").Append(stackSize).Append(" items.");
        }
    }
}