using Systems.BlockUI;using Systems.Items;

public class Burner: IBlockUI, IContainer{
    public int Priority{ get; set; }
    public bool Hidden{ get; set; }
    
    public Container fuelContainer;
    
    public int fuelTime;

    
    public Burner(int size, Item filter = null){
        fuelContainer = new Container(new ContainerProperties(size));
    }
    
    

    public bool Insert(ref ItemStack s, bool simulate = false){
       return fuelContainer.Insert(ref s, simulate);
    }

    public ItemStack Extract(){
        return fuelContainer.Extract();        
    }
}