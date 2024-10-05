using System;
using Systems;
using UnityEngine;

[Serializable]
public class Upgrade: ICloneable{
    
    public Sprite icon;
    
    public Stats stats;

    
    public virtual void Init(Unit u){
    }
    
    public virtual void Remove(){
    }

    public object Clone(){
        Upgrade clone = MemberwiseClone() as Upgrade;
        clone.stats = (Stats) stats.Clone();
        return clone;
    }
}