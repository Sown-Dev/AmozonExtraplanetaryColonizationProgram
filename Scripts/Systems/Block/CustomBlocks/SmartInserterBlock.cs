using System.Text.Json;
using System.Text.Json.Serialization;
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
            d.data.SetString( "filter", JsonSerializer.Serialize( filter, GameManager.JSONoptions ) );
            return d;
        }
        public override void Load(BlockData d){
            base.Load(d);
            filter = JsonSerializer.Deserialize<Filter>( d.data.GetString( "filter" ), GameManager.JSONoptions );
            mySlot.filter = filter;
        }
    }
}