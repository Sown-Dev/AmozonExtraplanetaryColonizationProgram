using Systems.Items;
using UI;

namespace Systems.Block
{
    public class SmartInserterBlock : InserterBlock
    {
        public Filter filter;

        override public void Init(Orientation orientation){
            base.Init(orientation);
            mySlot.filter = new Filter();
            filter = mySlot.filter;
            filter.Priority = 3;
        }
    }
}