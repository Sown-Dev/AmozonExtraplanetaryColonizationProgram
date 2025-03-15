using Systems.Block;
using Systems.Block.CustomBlocks;
using UnityEngine;

public class SolarPanelBlock : BaseIPowerProducerBlock{
    
    public override void Tick(){
        base.Tick();
        producing = (int)(baseRate * TerrainManager.Instance.GetSolarIntensity());
    }
}