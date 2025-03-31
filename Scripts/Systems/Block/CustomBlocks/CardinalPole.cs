using System;
using System.Collections.Generic;
using Systems.Block;
using Unity.Collections;
using UnityEngine;

public class CardinalPole : BaseConnector{

    public Vector2Int area = new(2, 2); //provided value is the topleft area
    public int poleRange = 5;

    [SerializeField] private LineRenderer lr;  //can override connect to wire to blocks and shit ig

    
    

    //these functions sucks but don't care enough to rewrite it. doable without lists
    public override Vector2Int[] GetConnectorCoverage(){
        List<Vector2Int> coverage = new();

        foreach (Vector2Int dir in new Vector2Int[]
                     { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }){
            for (int i = 1; i < poleRange; i++){
                coverage.Add(data.origin+dir*i);
            }
        }
        return coverage.ToArray();
    }
    public override Vector2Int[] GetBlockCoverage(){
        //get all v2ints in area, ie from -2 to 2, -2 to 2, as list in one line
        List<Vector2Int> coverage = new();
        for (int i = -area.x; i <= area.x; i++){
            for (int j = -area.y; j <= area.y; j++){
                coverage.Add(data.origin +new Vector2Int(i, j));
            }
        }
        return coverage.ToArray();
    }
    

  

}