using System;
using Systems;
using UnityEngine;

[Serializable]
public class Upgrade: ICloneable, IToolTippable{
    [field:SerializeField]public string name{get;set;}
    
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

    
    
    [field:SerializeField]public string description{ get; set; }
    [field:SerializeField]public Sprite icon{ get; set; }
}