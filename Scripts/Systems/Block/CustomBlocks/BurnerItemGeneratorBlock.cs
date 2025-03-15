using Systems.Block;
using Systems.Block.CustomBlocks;
using Systems.Items;

public class BurnerItemGeneratorBlock: BurnerProgressBarBlock{
    public ItemStack generatedItem;

    public int time;
    
    void Awake(){
        base.Awake();
        progressBar.maxProgress = time;
    }
    
    
    public override void CompleteCycle(){
        base.CompleteCycle();
        output.Insert(generatedItem.Clone());
    }
}