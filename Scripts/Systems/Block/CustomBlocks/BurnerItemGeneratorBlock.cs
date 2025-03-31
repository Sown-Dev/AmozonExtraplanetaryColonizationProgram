using Systems.Block;
using Systems.Block.CustomBlocks;
using Systems.Items;

public class BurnerItemGeneratorBlock: BurnerProgressBarBlock{
    public ItemStack generatedItem;

    public int time;
    

    public override void Init(Orientation orientation){
        base.Init(orientation);
        progressBar.maxProgress = time;
    }

    public override void CompleteCycle(){
        base.CompleteCycle();
        output.Insert(generatedItem.Clone());
    }
}