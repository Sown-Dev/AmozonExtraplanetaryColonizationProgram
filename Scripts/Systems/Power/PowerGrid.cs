using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerGrid{
    public List<IPowerBlock> blocks;
    public List<IPowerConnector> connectors;

    public int size{
        get{ return blocks.Count + connectors.Count; }
    }


    private int id = 0;

    public PowerGrid(){
        blocks = new List<IPowerBlock>();
        connectors = new List<IPowerConnector>();
        TerrainManager.Instance.powerGrids.Add(this);
        id = Random.Range(0, 6000);
    }
    

    public int totalPower;
    public int availablePower;

    public void GridTick(){
        totalPower = 0;
        int powerNeeded = 0;
        
        foreach (IPowerConsumer block in blocks.OfType<IPowerConsumer>()){
            powerNeeded += block.needed;
        }
        
        foreach (IPowerProducer generator in blocks.OfType<IPowerProducer>().OrderBy(x => x.Priority)){
            generator.neededOn = totalPower < powerNeeded;
            //idea: could maybe put a function in between here that lets us get our power production based on whether we need it or not
            totalPower += generator.producing;

        }

        availablePower = totalPower;
        foreach (IPowerConsumer block in blocks.OfType<IPowerConsumer>()){
            if (availablePower >= block.needed){
                block.providedPower = block.needed;
                availablePower -= block.needed;
            }
            else{
                block.providedPower = availablePower;
                availablePower = 0;
            }
        }
    }

    public bool HasBlock(IPowerBlock block){
        return blocks.Contains(block);
    }

    public void AddBlock(IPowerBlock block){
        block.myGrid?.RemoveBlock(block); //remove from old grid if any
        blocks.Add(block);
        block.myGrid = this;
    }

    public void AddConnector(IPowerConnector connector){
        connector.myGrid?.RemoveConnector(connector); //remove from old grid if any

        connectors.Add(connector);
        connector.myGrid = this;
    }

    public void RemoveBlock(IPowerBlock block){
        blocks.Remove(block);
        block.myGrid = null;
    }

    public void RemoveConnector(IPowerConnector connector){
        connectors.Remove(connector);
        connector.myGrid = null;
    }


    public void MergeGrid(PowerGrid other){
        if (other == null) return;
        //other grid is destroyed
        foreach (IPowerBlock block in other.blocks.ToList()){
            other.RemoveBlock(block);
            AddBlock(block);
        }

        foreach (IPowerConnector connector in other.connectors.ToList()){
            other.RemoveConnector(connector);
            AddConnector(connector);
        }


        other.KillGrid();
    }

    public void KillGrid(){
        foreach (var block in blocks.ToList()){
            RemoveBlock(block);
        }

        foreach (var connector in connectors.ToList()){
            RemoveConnector(connector);
        }

        TerrainManager.Instance.powerGrids.Remove(this);
    }
}