using System;
using System.Text;
using Systems.Block;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CropBlock: TickingBlock{
    
    //public new SaplingBlockData data => (SaplingBlockData)myData;
    public int growTime;
    public Block treePrefab;
    [FormerlySerializedAs("timeElapsed")] public int ticksElapsed = 0;
    public bool randomStartTime = false;
    public int growTimeRange;
    public Sprite[] stages;
    protected override void Awake(){
        base.Awake();
        
    }

    public override void Init(Orientation orientation){
        base.Init(orientation);
        ticksElapsed = 0;
        growTime += Random.Range(-growTimeRange, growTimeRange);
        if(randomStartTime)
            ticksElapsed = Random.Range(0, growTime/5);
    }
    
   

    public override void Tick(){
        base.Tick();
        ticksElapsed++;
        // set spriteto aceetain stage based on growth. should be split evenly,and make sure it is atthe start, so we dont go to end stateright before growth
        
        int stage = Mathf.FloorToInt(ticksElapsed / (growTime / stages.Length));
        //only usestage ifstagesisnt  empty 
        if (stages.Length > 0 && stage < stages.Length){
            sr.sprite= stages[stage];
        }
        
        if (ticksElapsed >= growTime){
            ticksElapsed = -2;
            //grow tree
            TerrainManager.Instance.RemoveBlock(data.origin, false);
            TerrainManager.Instance.PlaceBlock( treePrefab, data.origin,data.rotation);

        }
        
    }

    public override bool BlockDestroy(bool dropLoot = true){
        //remove from loot table where item is equal to my properties blockitem
        data?.lootTable?.RemoveAll(itemStack => itemStack.item == properties.myItem);
        return base.BlockDestroy(dropLoot);
    }

    public override StringBuilder GetDescription(){
        return base.GetDescription().AppendFormat
            ("This plant is {0}s old.\nAvg Growth time: {1}s", (int)(ticksElapsed/20f *10) /10, (int)(growTime/20f *10) /10);
    }

    public override BlockData Save(){
        BlockData d = base.Save();
        d.data.SetInt("timeElapsed", ticksElapsed);
        return d;
    }
    
    public override void Load(BlockData d){
        base.Load(d);
        ticksElapsed = d.data.GetInt("timeElapsed");
    }
}
[Serializable]
public class SaplingBlockData : TickingBlockData{
    public float timeElapsed = 0;
}