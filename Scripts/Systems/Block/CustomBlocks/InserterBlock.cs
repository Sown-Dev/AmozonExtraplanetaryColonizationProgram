using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Systems.BlockUI;
using Systems.Items;
using UnityEngine;

namespace Systems.Block
{
    public class InserterBlock : ContainerBlock
    {
        public Slot mySlot;
        public Orientation toFace;
        public DirectionSelect DirSelect = new DirectionSelect();
        public IContainerBlock toExtract;
        public IContainerBlock toInsert;
        public Transform arm;
        
        public bool hiddenDirSelect = false;
        
        [SerializeField] private SlotVisualizer slotVisualizer;

        
        private float extractTimer = 0;

        public override void Init(Orientation orientation)
        {
            base.Init(orientation);
            
            outputProperties.size = 1;
            output = new Container(outputProperties);
            output.Priority = 2;

            
            DirSelect = new DirectionSelect();
            DirSelect.inputOrientation = data.rotation.GetOpposite();
            DirSelect.outputOrientation = data.rotation;
            
            

            
        }

        protected override void Start(){
            base.Start();
            toFace = DirSelect.inputOrientation;
            SetArmRotation(toFace);
            mySlot = output.GetSlot(0);
            mySlot.Stacksize = 2;
            
            slotVisualizer.SetSlot(mySlot);

        }

        public override void Tick()
        {
            slotVisualizer.Refresh();

            // Smooth, linear rotation towards target angle
            float targetAngle = toFace.GetAngle();
            arm.localRotation = Quaternion.Euler(0, 0, Mathf.MoveTowardsAngle(arm.localRotation.eulerAngles.z, targetAngle, 5f));

            if (DirSelect.inputOrientation == DirSelect.outputOrientation) return;

            if (mySlot.IsEmpty())
            {
                // Move to input
                toFace = DirSelect.inputOrientation;

                if (IsArmAtTarget(toFace))
                {
                    extractTimer++;
                    if (extractTimer > 6)
                    {
                        extractTimer = 0;
                        Block b = TerrainManager.Instance.GetBlock(Vector2Int.RoundToInt((Vector2)transform.position + DirSelect.inputOrientation.GetVector2()));
                        toExtract = b as IContainerBlock;

                        // Transfer only if the extracted item matches the filter
                        if (toExtract != null)
                        {
                            CU.Transfer(toExtract, this);
                        }
                    }
                }
            }
            else
            {
                // Move to output
                toFace = DirSelect.outputOrientation;

                if (IsArmAtTarget(toFace))
                {
                    GetDirection(DirSelect.outputOrientation)?.Actuate();
                    toInsert = GetDirection(DirSelect.outputOrientation) as IContainerBlock;

                    if (toInsert != null && SlotMatchesFilter(mySlot))
                    {
                        CU.Transfer(mySlot, toInsert);
                    }
                }
            }

            if (mySlot.dirty)
            {
                slotVisualizer.Refresh();
                mySlot.dirty = false;
            }
        }

        private bool IsArmAtTarget(Orientation orientation)
        {
            float targetAngle = orientation.GetAngle();
            return Mathf.Abs(Mathf.DeltaAngle(arm.localRotation.eulerAngles.z, targetAngle)) < 5f;
        }

        private void SetArmRotation(Orientation orientation)
        {
            arm.localRotation = Quaternion.Euler(0, 0, orientation.GetAngle());
        }

        // Check if the slot's item matches the filter
        protected virtual bool SlotMatchesFilter(Slot slot)
        {
            return true; // No filter in base InserterBlock, accepts all items
        }

        public override ItemStack Extract(){
            //can't extract
            return null;
        }
        
        public override List<TileIndicator> GetIndicators(){
            var e = base.GetIndicators();
            //add input and output indicators
            e.Add( new TileIndicator(new Vector2Int[]{DirSelect.inputOrientation.GetVectorInt()}, IndicatorType.InsertingTo));
            e.Add( new TileIndicator(new Vector2Int[]{DirSelect.outputOrientation.GetVectorInt()}, IndicatorType.ExtractingFrom));
            
            
            return e;
        }

        public override BlockData Save(){
            BlockData save = base.Save();
            save.data.SetString( "DirSelect", JsonConvert.SerializeObject(DirSelect,GameManager.JSONsettings));
            save.data.SetInt( "toFace", (int)toFace);
            return save;
        }
        
        public override void Load(BlockData save){
            base.Load(save);
            DirSelect = JsonConvert.DeserializeObject<DirectionSelect>(save.data.GetString("DirSelect"),GameManager.JSONsettings);
            toFace = (Orientation)save.data.GetInt("toFace");
        }
    }
}

