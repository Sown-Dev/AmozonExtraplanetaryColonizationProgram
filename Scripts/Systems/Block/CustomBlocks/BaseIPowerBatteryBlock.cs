using System.Text;
using Systems.Block;
using Systems.Block.CustomBlocks;
using UnityEngine;

public class BaseIPowerBatteryBlock: BaseIPowerBlock, IPowerBattery{
    
    [SerializeField] private int baseCapacity = 1000;
    [SerializeField] private int baseTransferRate = 20;
    public int capacity{ get; set; } = 1000;
    public float storedPower{ get; set; }
    public int transferRate{ get; set; } = 10;

    public override void Init(Orientation orientation){
        base.Init(orientation);
    }
    protected override void Start(){
        base.Start();
        capacity = baseCapacity;
        transferRate = baseTransferRate;
    }

    public override StringBuilder GetDescription(){
      
        return base.GetDescription().AppendFormat(
            "\nStored: {0:F1}W/{1}W\nTransfer Rate: {2}W/S",
            storedPower,
            baseCapacity,
            baseTransferRate
        );
    }

}