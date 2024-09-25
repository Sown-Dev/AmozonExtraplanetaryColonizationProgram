using System.Collections.Generic;
using System.Linq;

public class PowerGrid{
    public List<IPowerBlock> blocks;
    
    public PowerGrid(){
        blocks = new List<IPowerBlock>();
        TerrainManager.Instance.powerGrids.Add(this);
    }
    
    public int totalPower;
    public int availablePower;
    
    public void GridTick(){
        totalPower = 0;
        
        
        
        foreach (IPowerProducer generator in blocks.OfType<IPowerProducer>()){
            totalPower += generator.producing;
        }
        
         availablePower = totalPower;    
        foreach (IPowerConsumer block in blocks.OfType<IPowerConsumer>()){
            if (availablePower >= block.consuming){
                block.providedPower = block.consuming;
                availablePower -= block.consuming;
            }
            else{
                block.providedPower = availablePower;
                availablePower = 0;
            }
        }

    }
    
    public void AddBlock(IPowerBlock block){
        block.myGrid?.RemoveBlock(block);
        blocks.Add(block);
        block.myGrid = this;
    }
    
    public void RemoveBlock(IPowerBlock block){
        blocks.Remove(block);
        block.myGrid = null;
    }
    
    public void MergeGrid(PowerGrid other){
        //other grid is destroyed
        foreach (IPowerBlock block in other.blocks){
            other.RemoveBlock(block);
            AddBlock(block);
        }
    }
}