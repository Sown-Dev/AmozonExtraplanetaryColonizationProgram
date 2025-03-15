using System.Collections.Generic;
using Systems.Block;
using Systems.Block.CustomBlocks;
using UnityEngine;

public class BurnerMoverBlock: BurnerProgressBarBlock{
    
    
    public override List<TileIndicator> GetIndicators(){
        var e =base.GetIndicators();
        e.Add( new TileIndicator(new []{Orientation.Down.GetVectorInt()}, IndicatorType.InsertingTo));
        e.Add( new TileIndicator(new []{Orientation.Up.GetVectorInt()*2}, IndicatorType.ExtractingFrom));

        return e;
    }
}