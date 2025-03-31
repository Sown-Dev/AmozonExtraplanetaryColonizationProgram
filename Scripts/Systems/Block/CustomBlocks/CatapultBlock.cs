using System.Collections.Generic;
using Systems.Items;
using UnityEngine;
using Systems.Block;
using UI.BlockUI;

namespace Systems.Block.CustomBlocks
{
    public class CatapultBlock : ContainerBlock
    {
        
        
        public new CatapultBlockData data => (CatapultBlockData)base.data;
        
        
        [SerializeField] private SlotVisualizer slotVisualizer;

        public int stackSize = 2;
        public Slot mySlot;

        private Vector3 goToPoint;

        protected override void Awake()
        {
            base.Awake();
            outputProperties.size = 1;
            data.output = new Container(outputProperties);
            mySlot = data.output.GetSlot(0);
            mySlot.Stacksize = stackSize;

            slotVisualizer.SetSlot(data.output.GetSlot(0));

            data.selector = new NumberSelector(null, 2, 12);
            data.selector.Priority = 21;
        }

        public override void Tick()
        {
            base.Tick();
            slotVisualizer.Refresh();

            switch (data.state)
            {
                case CatapultState.Idle:
                    Vector2Int sourcePos = data.origin + data.rotation.GetOpposite().GetVectorInt() * 2;
                    Block sourceBlock = TerrainManager.Instance.GetBlock(sourcePos);
                    if (sourceBlock is IContainerBlock sourceContainer)
                    {
                        bool transferred = CU.Transfer(sourceContainer, this);
                        if (transferred)
                        {
                            slotVisualizer.transform.position = (Vector3)(Vector2)sourcePos;
                        }
                    }
                    break;

                case CatapultState.Loading:
                    data. timeElapsed++;
                    goToPoint = transform.position + (Vector3)(data.rotation.GetOpposite().GetVector2() * 1f);

                    slotVisualizer.transform.position = Vector3.Lerp(
                        slotVisualizer.transform.position,
                        goToPoint,
                        (float)data.timeElapsed / 18);

                    if (data.timeElapsed > 22)
                    {
                        data. state = CatapultState.Firing;
                        data.timeElapsed = 0;
                    }
                    break;

                case CatapultState.Firing:
                    data.timeElapsed++;
                    Vector2Int targetGrid = data.origin + data.rotation.GetVectorInt() * data.selector.value;
                    Vector2 targetWorld = (Vector2)targetGrid + new Vector2(0.5f, 0.5f); // Center of the target tile

                    float progress = (float)data.timeElapsed / 24;
                    Vector3 horizontalPos = Vector3.Lerp(goToPoint, targetWorld, progress);
                    float verticalOffset = Mathf.Sin(progress * Mathf.PI) * 2;
                    slotVisualizer.transform.position = horizontalPos + new Vector3(0, verticalOffset, 0);

                    if (data.timeElapsed == 24)
                    {
                        Land(targetGrid);
                        data.state = CatapultState.Idle;
                        data.timeElapsed = 0;
                    }
                    break;
            }
        }

        public override List<TileIndicator> GetIndicators()
        {
            var indicators = base.GetIndicators();

            // Source indicator (2 tiles behind)
            Vector2Int sourcePos = data.origin + data.rotation.GetOpposite().GetVectorInt() * 2;
            indicators.Add(new TileIndicator(new[] { sourcePos }, IndicatorType.InsertingTo));

            // Target indicator (selector.value tiles ahead)
            Vector2Int targetPos = data.origin + data.rotation.GetVectorInt() * data.selector.value;
            indicators.Add(new TileIndicator(new[] { targetPos }, IndicatorType.ExtractingFrom));

            return indicators;
        }

        public void ChangeState(CatapultState newState)
        {
            data.state = newState;
            data.timeElapsed = 0;
        }

        public bool Land(Vector2Int pos)
        {
            if (TerrainManager.Instance.GetBlock(pos) is IContainerBlock icb)
            {
                return icb.Insert(ref mySlot.ItemStack);
            }
            else
            {
                Utils.Instance.CreateItemDrop(mySlot.ItemStack, (Vector2)pos + new Vector2(0.5f, 0.5f));
                mySlot.ItemStack = null;
                return true;
            }
        }

        public override bool Insert(ref ItemStack itemStack, bool simulate = false)
        {
            if (!mySlot.IsEmpty())
                return false;

            if (!simulate)
            {
                mySlot.ItemStack = itemStack.Clone();
                itemStack.amount = 0;
                ChangeState(CatapultState.Loading);
            }
            return true;
        }
    }

    public enum CatapultState
    {
        Idle,
        Loading,
        Firing
    }
    public class CatapultBlockData : ContainerBlockData
    {
        public NumberSelector selector;
        public int timeElapsed = 0;
        public CatapultState state = CatapultState.Idle;


    }
}