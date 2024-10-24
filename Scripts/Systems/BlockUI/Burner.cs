using System;
using System.Linq;
using Systems.BlockUI;
using Systems.Items;
using UnityEngine;

[Serializable]
public class Burner : IBlockUI, IContainer{
    public int Priority{ get; set; } = 100;
    public bool Hidden{ get; set; }

    public Container fuelContainer;

    [HideInInspector] public int fuelTime = 0;
    [HideInInspector] public int burnTimeTotal = 100;
     public int burnRate=1;
    public ContainerProperties properties;
    
    
    public Burner(){
        properties = new ContainerProperties( 1);
        Init();
    }
    
    [Obsolete("Not currently used. we rely on setting the burner properties in the block itself")]
    public Burner( int _burnRate, Item[] filter = null){
        fuelContainer = new Container(properties);
        //if (filter != null){
        //new filter code: for now, just use burnables. maybe later add some custom option that prevents you from inserting certain items.
            fuelContainer.filterList = ItemManager.Instance.burnables.ToList();
            fuelContainer.blackList = false;
        //}

        //burnRate = _burnRate;
        
    }
    
    public void Init(){
        fuelContainer = new Container(properties);
        fuelContainer.filterList = ItemManager.Instance.burnables.ToList();
        fuelContainer.blackList = false;
        burnTimeTotal = 20;
        fuelTime = 0;

        Priority = 100; 
    }
    

    public bool Burn(){
        burnRate = 1;

        if (fuelTime > 0){
            fuelTime -= burnRate;
            return true;
        }
        else{
            if (!fuelContainer.isEmpty()){
                Slot s= fuelContainer.GetExtractionSlot();
                if (s !=null  && s.ItemStack?.item.fuelValue > 0){
                    if (s.Decrement()){
                        fuelTime = s.ItemStack.item.fuelValue;
                        burnTimeTotal = fuelTime;
                        return true;
                    }
                }

            }
        }

        return false;
    }


    //must have fuel value to be inserted, can maybe change but would rather just not allow it. (ie if we want custom burners that use other types of fuel)
    public bool Insert(ref ItemStack s, bool simulate = false){
        if (s.item.fuelValue > 0){
            return fuelContainer.Insert(ref s, simulate);
        }

        return false;
    }

    //not sure if extracting should be allowed, but technically no reason it shouldn't be, since it should work.
    public ItemStack Extract(){
        return fuelContainer.Extract();
    }
}