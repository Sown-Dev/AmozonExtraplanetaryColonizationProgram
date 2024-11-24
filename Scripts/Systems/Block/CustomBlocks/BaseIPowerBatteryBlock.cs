using System.Text;
using Systems.Block.CustomBlocks;

public class BaseIPowerBatteryBlock: BaseIPowerBlock, IPowerBattery{
    
    
    public int capacity{ get; set; }
    public int storedPower{ get; set; }

    public override StringBuilder GetDescription(){
        if (myGrid != null){
            return base.GetDescription().AppendFormat("\nStored: {0}W/{1}W", myGrid.storedPower/myGrid.capacity * capacity, capacity);
        }
        return base.GetDescription().AppendFormat("\nStored: {0}W/{1}W", storedPower, capacity);

    }

}