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
    }

    public override void Tick(){
        base.Tick();
        timeElapsed++;
        if (timeElapsed >= growTime){
            timeElapsed = 0;
            //grow tree
            BlockDestroy(false);
            TerrainManager.Instance.PlaceBlock( treePrefab, origin,rotation);
        }
    }

    public override StringBuilder GetDescription(){
        return base.GetDescription().AppendFormat("This tree is {0}s old. Trees grow up after an average of 225 seconds, although it varies.", timeElapsed/20f%00.0);
    }
}