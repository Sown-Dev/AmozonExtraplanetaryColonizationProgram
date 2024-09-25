using UnityEngine;

public class WorldUpgrade{
    public Sprite icon;
    
    public GlobalStats stats;

   
    
    public virtual void Init(){
    }
    
    public virtual void Remove(){
    }

    public object Clone(){
        WorldUpgrade clone = MemberwiseClone() as  WorldUpgrade;
        clone.stats = (GlobalStats) stats.Clone();
        return clone;
    }
}