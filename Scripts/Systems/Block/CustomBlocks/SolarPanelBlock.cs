using Systems.Block;
using Systems.Block.CustomBlocks;
using UnityEngine;

public class SolarPanelBlock : BaseIPowerProducerBlock{
    
    public override void Tick(){
        base.Tick();
        producing = (int)(baseRate * (10f-(TerrainManager.totalTicksElapsed %800)/80f)/10f);
    }
}