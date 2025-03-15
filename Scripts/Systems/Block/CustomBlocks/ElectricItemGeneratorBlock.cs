using Systems.Block;
using Systems.Block.CustomBlocks;
using Systems.Items;
using UnityEngine.Serialization;

public class ElectricItemGeneratorBlock: ElectricProgressBlock{
    public ItemStack generatedItem;

    [FormerlySerializedAs("time")] public int baseTime;
    
    void Awake(){
        base.Awake();
        progressBar.maxProgress = baseTime;
        progressBar.Priority = 0;
    }
    
    
    public override void CompleteCycle(){
        base.CompleteCycle();
        output.Insert(generatedItem.Clone());
    }
}