﻿using System.Collections.Generic;
using Systems.Block.CustomBlocks;
using UnityEngine;



public struct TileIndicator{
    public Vector2Int[] pos;
    public IndicatorType type;
    
    public TileIndicator(Vector2Int[] pos, IndicatorType type){
        this.pos = pos;
        this.type = type;
    }
}


public enum IndicatorType{
    Mining,
    Harvesting, //referring to extracting resources from a resource block
    BlockPower,
    PowerConnector,
    InsertingTo,
    ExtractingFrom,
}
