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
        if(grid == null){
            grid = new PowerGrid();
        }
        
        if (Visited){
            return;
        }
        else{
            Visited = true;
            if (myGrid == grid){
                //do nothing, still recur on children
            }
            else{
                if (myGrid == null){
                    myGrid = grid;
                }
                else{
                    grid.MergeGrid(myGrid);
                }
            }

            foreach (var connector in connectors){
                connector.SetGridRecursive(grid);
            }
        }
        
        foreach (var block in connectedBlocks){
            if (!myGrid.HasBlock(block)){
                myGrid.AddBlock(block);
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
            if (myGrid == null){
                connector.myGrid.AddConnector(this);
            }
            else{
                if (connector.myGrid != myGrid){

                    myGrid.MergeGrid(connector.myGrid);
                    connector.myGrid = myGrid;
                }
            }
        }
        if(myGrid==null){
            myGrid = new PowerGrid();
        }

        foreach (var block in connectedBlocks){
            if (!myGrid.HasBlock(block)){
                myGrid.AddBlock(block);
            }
        }
    }

    public override bool BlockDestroy(bool dropLoot){
        if (!base.BlockDestroy(dropLoot)){
            return false;
        }
        myGrid?.KillGrid();
        

        foreach (IPowerConnector connector in connectors){
            connector.GetConnected();

            //connector.connectors.Remove(this);
            
            connector.SetVisitedRecursive(false);
            PowerGrid grid = new PowerGrid();
            connector.myGrid = grid;
            connector.SetGridRecursive(grid);
            
        }

        
        return true;
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
            Gizmos.DrawLine((Vector2)origin, (Vector2)connector.myBlock.origin+Vector2.up);

        }
        
        if(myGrid!=null){
            Gizmos.color = Utils.GenerateUniqueColor(myGrid);
            Gizmos.DrawSphere( (Vector2)origin, 0.25f);
        }
    }
    
   
}