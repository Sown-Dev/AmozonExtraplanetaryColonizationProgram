using System.Text;
using Systems.Block;
using Systems.Block.CustomBlocks;
using UnityEngine;

public class BaseIPowerBatteryBlock: BaseIPowerBlock, IPowerBattery{
    
    [SerializeField] private int baseCapacity = 1000;
    
    public int capacity{ get; set; }
    public int storedPower{ get; set; }

    public override void Init(Orientation orientation){
        base.Init(orientation);
        capacity = baseCapacity;
    }

    public override StringBuilder GetDescription(){
        if (myGrid != null){
            return base.GetDescription().AppendFormat("\nStored: {0}W/{1}W", myGrid.storedPower/myGrid.capacity * capacity, capacity);
        }
        return base.GetDescription().AppendFormat("\nStored: {0}W/{1}W", storedPower, capacity);

    }

}