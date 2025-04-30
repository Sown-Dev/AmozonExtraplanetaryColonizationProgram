using Systems.Items;
using Systems.Round;

namespace Systems.Block{
    public class ContractSellerBlock: SellerBlock{
    
    
    
        public override bool CanSell(ItemStack itemStack){
            return base.CanSell(itemStack) && RoundManager.Instance.CanSell(itemStack);
        }
    
    }
}