using Systems.BlockUI;
using Systems.Items;
using UnityEngine;

namespace Systems.Block
{
    public class InserterBlock : ContainerBlock
    {
        public Slot mySlot;
        public Orientation toFace;
        public DirectionSelect DirSelect;
        public IContainerBlock toExtract;
        public IContainerBlock toInsert;
        public Transform arm;
        
        [SerializeField] private SlotVisualizer slotVisualizer;

        protected override void Awake()
        {
            DirSelect = new DirectionSelect();
            base.Awake();

            outputProperties.size = 1;
            output = new Container(outputProperties);
            mySlot = output.GetSlot(0);
            mySlot.Stacksize = 2;

            slotVisualizer.SetSlot(mySlot);
        }

        private float extractTimer = 0;

        public override void Init(Orientation orientation)
        {
            base.Init(orientation);
            DirSelect.inputOrientation = rotation;
            DirSelect.outputOrientation = rotation.GetOpposite();

            toFace = DirSelect.inputOrientation;
            SetArmRotation(toFace);
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
    }
}

