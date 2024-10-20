using Systems.Items;
using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class CatapultBlock : ContainerBlock{
        [SerializeField] private SlotVisualizer slotVisualizer;

        public int stackSize = 2;


        public Slot mySlot;
        public NumberSelector selector;

        private Vector3 goToPoint;

        private CatapultState state = CatapultState.Idle;

        protected override void Awake(){
            base.Awake();
            //init container
            outputProperties.size = 1;
            output = new Container(outputProperties);
            mySlot = output.GetSlot(0);
            mySlot.Stacksize = stackSize;

            slotVisualizer.SetSlot(output.GetSlot(0));

            selector = new NumberSelector(null, 2, 12);
            selector.Priority = 21;

        }

        int timeElapsed = 0;

        public override void Tick(){
            slotVisualizer.Refresh();

            //slotVisualizer.transform.position =Vector3.Lerp(slotVisualizer.transform.position, goToPoint, 0.2f); //10/24
            switch (state){
                case CatapultState.Idle:
                    //extract item from inventory behind
                    if(!mySlot.IsEmpty()){
                        state = CatapultState.Loading;
                        slotVisualizer.transform.position =
                            transform.position + (Vector3)(rotation.GetOpposite().GetVector2() * 1.5f);
                    }
                    
                    
                    break;
                case CatapultState.Loading:
                    timeElapsed++;

                    goToPoint = transform.position + (Vector3)(rotation.GetOpposite().GetVector2() * 1f);

                    slotVisualizer.transform.position = Vector3.Lerp(
                        transform.position + (Vector3)(rotation.GetOpposite().GetVector2() * 1.6f), goToPoint,
                        (float)timeElapsed / 18);

                    if (timeElapsed>22){
                        state = CatapultState.Firing;
                        timeElapsed = 0;
                    }

                    break;
                case CatapultState.Firing:
                    timeElapsed++; //not outside since we dont want to increase in idle, could cause overflow

                    Vector2 target = (origin + (rotation.GetVector2() * selector.value));
                    
                    //give parabolic trajectory, with 24 ticks to reach target, inclduing height
                    slotVisualizer.transform.position = Vector3.Lerp( transform.position + (Vector3)(rotation.GetOpposite().GetVector2() * 1f), target, (float)timeElapsed / 24)
                                                        + new Vector3(0, Mathf.Sin((float)timeElapsed / 24 * Mathf.PI) * 2, 0);
                    
                    if (timeElapsed == 24){
                        Land(Vector2Int.RoundToInt(target));
                        state = CatapultState.Idle;
                    }

                    break;

                default:
                    break;
            }
        }
        public void ChangeState(CatapultState newState){
            state = newState;
            timeElapsed = 0;
        }

        public bool Land(Vector2Int pos){
            //attempt to insert if block at pos it Icontainer. otherwise, drop item. (maybe prevent launching if not launching at valid target?)
            if (TerrainManager.Instance.GetBlock(pos) is IContainerBlock icb){
                return icb.Insert(ref mySlot.ItemStack);
            }
            else{
                Utils.Instance.CreateItemDrop(mySlot.ItemStack, (Vector2)pos);
            }

            return true;
        }


        public override bool Insert(ref ItemStack mySlot, bool simulate = false){
            if(!this.mySlot.IsEmpty()){
                //even if item matches, we reject new items until we're done launching
                return false;
            }
            
            if (!simulate){                 
                ChangeState(CatapultState.Loading);
            }

            return base.Insert(ref mySlot, simulate);
        }
    }

    public enum CatapultState{
        Idle,
        Loading,
        Firing
    }
}