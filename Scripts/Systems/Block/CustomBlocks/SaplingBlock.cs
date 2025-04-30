using System;
using System.Text;
using Systems.Block;
using Random = UnityEngine.Random;

public class SaplingBlock: TickingBlock{
    
    //public new SaplingBlockData data => (SaplingBlockData)myData;
    private int growTime;
    public Block treePrefab;
    public float timeElapsed = 0;

    protected override void Awake(){
        base.Awake();
        growTime = Random.Range(3000, 6000) + Random.Range(-1500, 7000);

    }

    public override void Init(Orientation orientation){
        base.Init(orientation);
        timeElapsed = Random.Range(0, growTime/5);
    }
    
   

    public override void Tick(){
        base.Tick();
        timeElapsed++;
        if (timeElapsed >= growTime){
            timeElapsed = -2;
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
            ("This sapling is {0}s old.\nSaplings grow up after an average of 5 minutes, although it varies a lot.", (int)(timeElapsed/20f *10) /10);
    }

    public override BlockData Save(){
        BlockData d = base.Save();
        d.data.SetFloat("timeElapsed", timeElapsed);
        return d;
    }
    
    public override void Load(BlockData d){
        base.Load(d);
        timeElapsed = d.data.GetFloat("timeElapsed");
    }
}
[Serializable]
public class SaplingBlockData : TickingBlockData{
    public float timeElapsed = 0;
}