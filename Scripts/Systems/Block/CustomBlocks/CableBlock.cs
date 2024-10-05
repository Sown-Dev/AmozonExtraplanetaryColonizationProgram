using System;
using System.Collections.Generic;
using Systems.Block;
using UnityEngine;

public class CableBlock : Block, IPowerConnector{
    public Block myBlock{
        get{ return this; }
    }

    public PowerGrid myGrid{ get; set; }
    public bool Visited{ get; set; }
    public List<IPowerBlock> connectedBlocks{ get; set; }
    public List<IPowerConnector> connectors{ get; set; }
    public void CheckConnections(){ }

    public void SetVisitedRecursive(bool visited = false){
        if (Visited == visited){
            return;
        }

        Visited = visited;
        foreach (var connector in connectors){
            connector.SetVisitedRecursive(visited);
        }
    }

    public void SetGridRecursive(PowerGrid grid){
        if (Visited){
            return;
        }
        else{
            Visited = true;
            if (myGrid == grid){
                return;
            }
            else{
                if (myGrid == null){
                    myGrid = grid;
                    foreach (var connector in connectors){
                        connector.SetGridRecursive(grid);
                    }
                }
                else{
                    grid.MergeGrid(myGrid);
                }
            }

            foreach (var connector in connectors){
                connector.SetGridRecursive(grid);
            }
        }
    }


    [SerializeField] private LineRenderer lr;

    private void Start(){
        GetConnected();
        //then look at connected 
    }

    public void GetConnected(){
        List<Block> adjacentBlocks =
            TerrainManager.Instance.GetAdjacentBlocks(origin, properties.size.x, properties.size.y);
        connectedBlocks = adjacentBlocks.FindAll(block => block is IPowerBlock powerBlock)
            .ConvertAll(block => (IPowerBlock)block);
        connectors = adjacentBlocks.FindAll(block => block is IPowerConnector powerConnector)
            .ConvertAll(block => (IPowerConnector)block);
        foreach (var connector in connectors){
            connectors.Add(connector);
            if (myGrid == null){
                connector.myGrid.AddConnector(this);
            }
            else{
                if (connector.myGrid != myGrid){
                    myGrid.MergeGrid(connector.myGrid);
                }
            }
        }

        foreach (var block in connectedBlocks){
            if (!myGrid.HasBlock(block)){
                myGrid.AddBlock(block);
                connectedBlocks.Add(block);
            }
        }
    }

    public override bool BlockDestroy(bool dropLoot){
        foreach (IPowerBlock block in connectedBlocks){
            myGrid.RemoveBlock(block);
            //kill grid
        }

        foreach (IPowerConnector connector in connectors){
            connector.connectors.Remove(this);
            myGrid.RemoveConnector(connector);
            connector.GetConnected();
        }

        myGrid.RemoveConnector(this);
        return base.BlockDestroy();
    }

    public void OnDrawGizmos(){
        //draw yellow to all blocks, red to all connectors
        Gizmos.color = Color.yellow;
        foreach (var block in connectedBlocks){
            Gizmos.DrawLine((Vector2)origin, (Vector2)block.myBlock.origin);
        }

        Gizmos.color = Color.red;
        foreach (var connector in connectors){
            Gizmos.DrawLine((Vector2)origin, (Vector2)connector.myBlock.origin);
        }
    }
    
   
}