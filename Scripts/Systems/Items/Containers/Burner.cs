using Systems.BlockUI;using Systems.Items;

public class Burner: IBlockUI, IContainer{
    public int Priority{ get; set; }
    public bool Hidden{ get; set; }
    
    public Container fuelContainer;
    
    public int fuelTime;
    public int burnRate;
    
    
    public Burner(int size,  int _burnRate, Item[] filter = null){
        fuelContainer = new Container(new ContainerProperties(size));
        burnRate = _burnRate;
    }
    
    public bool Burn(){
        if(fuelTime > 0){
            fuelTime-=burnRate;
            return true;
        }else{
            if (!fuelContainer.isEmpty()){
                if (fuelContainer.GetExtractionSlot().Decrement()){
                    fuelTime= 100;
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