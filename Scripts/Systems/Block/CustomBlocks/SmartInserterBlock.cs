using MemoryPack;
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
        public override void Tick(){
            base.Tick();
            mySlot.filter = filter;
        }
        

        public override BlockData Save(){
            BlockData d = base.Save();
            d.data.Set("filter", filter);
            return d;
        }
        public override void Load(BlockData d){
            base.Load(d);
            filter = d.data.Get<Filter>( "filter" );
            mySlot.filter = filter;
        }
    }
}