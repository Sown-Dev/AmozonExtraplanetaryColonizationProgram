using Systems.Block;
using Systems.Block.CustomBlocks;
using Systems.BlockUI;
using Systems.Items;
using Systems.Round;
using Unity.VisualScripting;
using UnityEngine;

public class SellerBlock: ProgressMachineContainerBlock{
    

    public float rate = 1f;
    
    [HideInInspector]public Label label;
    
    protected override void Awake(){
        base.Awake();
    }

    protected override void Start(){
        base.Start();
        label = new Label();
        Color goodColor = new Color(0.8f, 1f, 0.7f);
        Color badColor = new Color(1f, 0.6f, 0.6f);

        label.textColor = Color.Lerp(badColor, goodColor, rate);        
        label.text = Mathf.RoundToInt(rate * 100) + "%";
        label.background = true;
        label.Priority = 21;
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
            if(CanSell( s.ItemStack)){
                RoundManager.Instance.Sell(s.ItemStack, rate);
                s.ItemStack = null;
                
                return;
            }
        }   
    }

    public virtual bool CanSell(ItemStack itemStack){
        return itemStack.item.value > 0;
    }

    
    
}