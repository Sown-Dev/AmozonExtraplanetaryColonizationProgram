using Newtonsoft.Json;
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
            d.data.SetString( "filter", JsonConvert.SerializeObject( filter, GameManager.JSONsettings ) );
            return d;
        }
        public override void Load(BlockData d){
            base.Load(d);
            filter = JsonConvert.DeserializeObject<Filter>( d.data.GetString( "filter" ), GameManager.JSONsettings );
            mySlot.filter = filter;
        }
    }
}