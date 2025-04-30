using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void addStatusEffect(StatusEffect se);
public delegate void removeStatusEffect(StatusEffect se);


[Serializable]
public class Debuffs{

    public event addStatusEffect OnAddStatusEffect;
    public event removeStatusEffect OnRemoveStatusEffect;

    public float size
    {
        get {
            return debuffs.Count;
        }
    }
    
    public Transform parent;
    public enum DebuffTypes{
        Poison = 1,
        Mark = 2,
        Stun = 4,
        Regen = 5,
        Stats = 6,
    }

    public List<StatusEffect> debuffs;

    public Debuffs(Transform p){
        debuffs = new List<StatusEffect>();
        parent = p;
    }
    
    public void Tick( IStatusEffectable st, float buffMult=1, float debuffMult=1){
        foreach (StatusEffect se in debuffs.ToList()){
            se.duration -= Time.deltaTime;
            se.Tick( st, buffMult, debuffMult);
            if(se.duration<=0){
                se.Remove();
                debuffs.Remove(se);
            }
        }
    }

    public Stats applyStats(Stats stats){
        foreach (StatusEffect se in debuffs){
            if (se.isStats){
                stats.Combine(se.stats);
            }
        }

        return stats;
    }

    public void AddDebuff(StatusEffect input){
        var se = input.Clone();
        
            bool found = false;
            foreach (StatusEffect v in debuffs){
                if (v.type == se.type && v.strength == se.strength ){
                    if (se.isStats){
                        if (v.stats == se.stats){
                            AddEffects(v, se);
                            found = true;
                            break;
                        }
                    }
                    else{
                        AddEffects(v, se);
                        found = true;
                        break;
                    }
                }

            }
            if (!found){
                debuffs.Add(se);
                se.Init(parent);
            }
        
        
    }
    public bool this[DebuffTypes type] => debuffs.Find(t => t.type == type) != null;

    public StatusEffect Get(DebuffTypes type){
        return debuffs.Find(t => t.type == type);
    }

    public StatusEffect AddEffects(StatusEffect a, StatusEffect b){
        if (a.addStrength){
            a.strength += b.strength;
        }
        else{
            a.duration += b.duration;
        }

        return a;
    }
    
    public void RemoveAll(){
        foreach (StatusEffect se in debuffs){
            se.Remove();
        }
        debuffs.Clear();
    }
    
}