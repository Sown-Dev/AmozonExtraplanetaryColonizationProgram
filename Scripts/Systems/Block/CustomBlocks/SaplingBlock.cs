using System.Text;
using Systems.Block;
using UnityEngine;

public class SaplingBlock: TickingBlock{
    private int growTime;
    private int timeElapsed = 0;
    public Block treePrefab;

    public override void Init(Orientation orientation){
        base.Init(orientation);
        growTime = Random.Range(3000, 6000) + Random.Range(-1500, 7000);
        timeElapsed = Random.Range(0, growTime/4);
    }

    public override void Tick(){
        base.Tick();
        timeElapsed++;
        if (timeElapsed >= growTime){
            timeElapsed = -2;
            //grow tree
            TerrainManager.Instance.RemoveBlock(origin, false);
            TerrainManager.Instance.PlaceBlock( treePrefab, origin,rotation);

        }
        
    }

    public override bool BlockDestroy(bool dropLoot = true){
        //remove from loot table where item is equal to my properties blockitem
        lootTable.RemoveAll(itemStack => itemStack.item == properties.myItem);
        return base.BlockDestroy(dropLoot);
    }

    public override StringBuilder GetDescription(){
        return base.GetDescription().AppendFormat("This sapling is {0}s old.\nSaplings grow up after an average of 5 minutes, although it varies a lot.", (int)(timeElapsed/20f *10) /10);
    }
}