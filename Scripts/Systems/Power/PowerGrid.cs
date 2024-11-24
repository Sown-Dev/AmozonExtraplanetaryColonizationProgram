using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerGrid{
    public List<IPowerBlock> blocks;
    public List<IPowerConnector> connectors;

    public int size{
        get{ return blocks.Count + connectors.Count; }
    }
    
    
    //local vars
    
    //stored power and max capacity
    public int capacity;
    public float storedPower;
    
    //production and max production
    public int producing;
    public int productionCapacity;
    
    //consumption and max consumption (if we don't have enough power we have less consumption than max)
    public int consuming;
    public int powerNeeded;
    
    private int id = 0; // an unnecessary id. it is only used for debugging as it allows serialized representations to be unique
    public PowerGrid(){
        blocks = new List<IPowerBlock>();
        connectors = new List<IPowerConnector>();
        TerrainManager.Instance.powerGrids.Add(this);
        id = Random.Range(0, 6000);
        capacity = 0;
        storedPower = 0;
    }

    public void GridTick(){
        producing = 0; //how much power we are producing
        powerNeeded = 0; //how much power we need
        consuming = 0; //keeps track of how much power we are consuming. can be seperate from needs if we are overconsuming
        
        foreach (IPowerConsumer block in blocks.OfType<IPowerConsumer>()){
            powerNeeded += block.needed;
        }
        
        foreach (IPowerProducer generator in blocks.OfType<IPowerProducer>().OrderBy(x => x.Priority)){
            generator.neededOn = producing < powerNeeded;
            //idea: could maybe put a function in between here that lets us get our power production based on whether we need it or not
            producing += generator.producing;
        }
        int remainingPower = producing; //we use this to keep track of how much power we have left to give to consumers, but still need to keep track of how much power we produce


        if (producing > powerNeeded){
            //add difference to stored power
            StorePower(producing - powerNeeded, Time.deltaTime);
            remainingPower-= producing - powerNeeded;
        }
        if(producing < powerNeeded){
            //use stored power
            UsePower(powerNeeded - producing, Time.deltaTime);
            remainingPower+= powerNeeded - producing;
        }
        

        foreach (IPowerConsumer block in blocks.OfType<IPowerConsumer>()){
            if (remainingPower>= block.needed){
                block.providedPower = block.needed;
                remainingPower-= block.needed;
                consuming += block.needed;
            }
            else{
                block.providedPower = remainingPower;
                consuming += remainingPower;
                remainingPower = 0;
            }
        }
        
        
    }
    
    public void StorePower(int power, float deltaTime){
        storedPower += power*deltaTime;
        if (storedPower > capacity){
            storedPower = capacity;
        }
    }
    
    public int UsePower(int power, float deltaTime){
        if (storedPower >= power){
            storedPower -= power*deltaTime;
            return power;
        }
        else{
            int usedPower = (int)storedPower;
            storedPower = 0;
            return usedPower;
        }
    }

    public bool HasBlock(IPowerBlock block){
        return blocks.Contains(block);
    }

    public void AddBlock(IPowerBlock block){
        block.myGrid?.RemoveBlock(block); //remove from old grid if any
        blocks.Add(block);
        block.myGrid = this;
        if(block is IPowerBattery battery){
            capacity += battery.capacity;
            storedPower += battery.storedPower;
        }
    }

    public void AddConnector(IPowerConnector connector){
        connector.myGrid?.RemoveConnector(connector); //remove from old grid if any

        connectors.Add(connector);
        connector.myGrid = this;
    }

    public void RemoveBlock(IPowerBlock block){
        blocks.Remove(block);
        block.myGrid = null;
        if(block is IPowerBattery battery){
            capacity -= battery.capacity;
            int powerTransfer = UsePower((int)  storedPower/capacity*battery.capacity, Time.deltaTime);
            battery.storedPower = powerTransfer;
        }
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