using Systems.Block;
using Systems.Block.CustomBlocks;
using Systems.Items;
using Systems.Round;

public class SellerBlock: ProgressMachineContainerBlock{
    
    
    protected override void Awake(){
        base.Awake();
        progressBar.maxProgress = 40;
    }

    public override bool CanProgress(){
        return base.CanProgress() && !output.isEmpty();
    }

    public override void CompleteCycle(){
        base.CompleteCycle();
        if(output.isEmpty()) return;
        //sell all items in the output container
        foreach(Slot s in output.containerList){
            if(s.ItemStack == null) continue;
            if(RoundManager.Instance.CanSell(s.ItemStack)){
                RoundManager.Instance.Sell(s.ItemStack);
                s.ItemStack = null;
            }
        }   
    }

    
    
}