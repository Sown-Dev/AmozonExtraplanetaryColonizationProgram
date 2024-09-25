using System;
using System.Collections.Generic;
using Systems.Block;
using UnityEngine;

public class CardinalPole : Block, IPowerBlock{
    public int Priority{ get; set; }
    public bool Hidden{ get; set; }

    public Vector2Int area = new(2, 2); //provided value is the topleft area
    public int poleRange = 5;

    public PowerGrid myGrid{ get; set; }
    
    
    public Block myBlock{
        get{ return this; }
    }

    List<IPowerBlock> connectedBlocks = new();
    [SerializeField] private LineRenderer lr;

    private void Start(){
        //get adjascent poles
        Vector2Int pos = Vector2Int.RoundToInt(transform.position);

        foreach (Vector2Int dir in new Vector2Int[]
                     { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }){
            for (int i = 1; i < poleRange; i++){
                if (TerrainManager.Instance.GetBlock(pos+dir*i) is CardinalPole pole && pole != this){
                    pole.myGrid.AddBlock(this);
                    //might want to redo add logic
                    pole.connectedBlocks.Add(this);
                    connectedBlocks.Add(pole);
                    break;
                }
            }
        }

        if (myGrid == null){
            myGrid = new PowerGrid();
            myGrid.AddBlock(this);
        }

        //get all blocks in area
        for(int i = -area.x; i <= area.x; i++){
            for(int j = -area.y; j <= area.y; j++){
                if (TerrainManager.Instance.GetBlock(pos + new Vector2Int(i, j)) is IPowerBlock block){
                    if (block.myGrid == null){
                        myGrid.AddBlock(block);
                        connectedBlocks.Add(block);
                    }
                }
            }
        }
        
        
        //draw line
        lr.positionCount = connectedBlocks.Count*2;
        int indx = 0;
        foreach (IPowerBlock block in connectedBlocks){
            lr.SetPosition(indx, transform.position + Vector3.up);
            indx++;
            lr.SetPosition(indx, block.myBlock.transform.position + Vector3.up);
            indx++;

        }
        
    }

    public override bool BlockDestroy(bool dropLoot){
        foreach (IPowerBlock block in connectedBlocks){
            myGrid.RemoveBlock(block);
        }
        myGrid.RemoveBlock(this);
        return base.BlockDestroy();
    }
}