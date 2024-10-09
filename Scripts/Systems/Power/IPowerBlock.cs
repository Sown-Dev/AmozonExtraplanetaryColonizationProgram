using System.Collections.Generic;
using Systems.Block;
using Systems.BlockUI;
using UnityEngine;

public interface IPowerBlock{
    public int Priority{ get; } //priority of consumption/production, higher consumption is more needed, ie logistics over refine, higher production is less important to be OFF ie solar
    public PowerGrid myGrid{ get; set; } //the grid that the block is in. If null, then no grid and no power
    public Block myBlock{ get; } //stupid, but we need a reference to the block
    public IPowerConnector myConnector{ get; set; } //the connector that the block is connected to
}

public interface IPowerProducer: IPowerBlock{
    public int producing{get; set; }
    public bool neededOn{ get; set; }
}

public interface IPowerConsumer: IPowerBlock{
    public int needed{ get; set; }
    public int providedPower{ get; set; }//power that the block has. is set outside but only used internally
}
public interface IPowerConnector{
    public Block myBlock{ get; }
    public PowerGrid myGrid{ get; set; }
    public bool Visited{ get; set; }
    public List<IPowerBlock> connectedBlocks{ get; set; }
    public List<IPowerConnector> connectors{ get; set; }
    public Vector2Int[] GetBlockCoverage();
    public Vector2Int[] GetConnectorCoverage();
    public void GetConnected();
    public void SetVisitedRecursive(bool visited=false);
    public void SetGridRecursive(PowerGrid grid);
    public void Connect(IPowerBlock block);
    public void Disconnect(IPowerBlock block);
}
