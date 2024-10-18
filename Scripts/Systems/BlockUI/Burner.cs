using System.Linq;
using Systems.BlockUI;using Systems.Items;
using UnityEngine;

public class Burner: IBlockUI, IContainer{
    public int Priority{ get; set; } = 100;
    public bool Hidden{ get; set; }
    
    public Container fuelContainer;
    
    [HideInInspector]public int fuelTime;
    public int maxFuelTime = 96;
    public int burnRate;
    
    
    public Burner(int size,  int _burnRate, Item[] filter = null){
        fuelContainer = new Container(new ContainerProperties(size));
        fuelContainer.filterList = filter.ToList();
        fuelContainer.blackList = false;
        burnRate = _burnRate;
    }
    
    public bool Burn(){
        if(fuelTime > 0){
            fuelTime-=burnRate;
            return true;
        }else{
            if (!fuelContainer.isEmpty()){
                if (fuelContainer.GetExtractionSlot().Decrement()){
                    fuelTime= maxFuelTime;
                }
                return false;
            }
        }

        return false;
    }
    
    
    

    public bool Insert(ref ItemStack s, bool simulate = false){
       return fuelContainer.Insert(ref s, simulate);
    }

    public ItemStack Extract(){
        return fuelContainer.Extract();        
    }
}