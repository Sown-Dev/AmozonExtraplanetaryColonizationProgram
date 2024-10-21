using System;
using System.Collections.Generic;
using System.Linq;


public enum GStatstype{
    OreUseChance,
    OreYieldMult,
    OreYieldAdd,
    BonusTickRate,
    
    
}

[Serializable]
public class GlobalStats : ICloneable{
    public List<GlobalStatistic> stats;


    public GlobalStats(){
        stats = new List<GlobalStatistic>();
    }

    public GlobalStats(int i){
        stats = new List<GlobalStatistic>();
        foreach (GStatstype t in Enum.GetValues(typeof(GStatstype))){
            stats.Add(new GlobalStatistic(t, i, Stats.StatsOperation.Multiply));
        }
    }


   

    public GlobalStats Combine(GlobalStats toCombine){
        if(toCombine == null){
            return this;
        }
        
        /*foreach (KeyValuePair<Statstype, GlobalStatistic> e  in toCombine.stats){
            if (this.stats[e.Key] != null){
                if (e.Value.operation == StatsOperation.Add){
                    this.stats[e.Key].amount += e.Value.amount;
                }
                if(e.Value.operation == StatsOperation.Multiply){
                    this.stats[e.Key].amount *= e.Value.amount ;
                }
            }else{
                this.stats[e.Key] = e.Value;
            }
        }
        return this;*/
        foreach (var e in toCombine.stats){
            bool found = false;
            foreach (var f in this.stats.Where(f => f.type == e.type)){
                found = true;
                //if both multiply, end result is multiply, if one is add and one mult, end is add, else add
                // ( * * => * ) ; ( * + => + ; + * => + ) ; ( + + => + )
                if (e.operation == Stats.StatsOperation.Multiply && f.operation == Stats.StatsOperation.Multiply){
                    f.amount *= e.amount;
                    f.operation = Stats.StatsOperation.Multiply;
                }
                else if (e.operation == Stats.StatsOperation.Multiply && f.operation == Stats.StatsOperation.Add){
                    f.amount += e.amount;
                    f.operation = Stats.StatsOperation.Add;
                }
                else if (e.operation == Stats.StatsOperation.Add && f.operation == Stats.StatsOperation.Multiply){
                    f.amount *= e.amount;
                    f.operation = Stats.StatsOperation.Add;
                }
                else if (e.operation == Stats.StatsOperation.Add && f.operation == Stats.StatsOperation.Add){
                    f.amount += e.amount;
                    f.operation = Stats.StatsOperation.Add;
                }
                //doesn't break in case you have multiple stats of the same type
            }

            if (!found){
                this.stats.Add(e);
            }
        }

        return this;
    }

    //rewrite this for null case
    public float this[GStatstype stat]{
        get{
            if (this.stats.Find(t => t.type == stat) != null){
                return (float)this.stats.Find(t => t.type == stat).amount;
            }
            else{
                return 0;
            }
        }
        set{
            if (this.stats.Find(t => t.type == stat) != null){
                this.stats.Find(t => t.type == stat).amount = value;
            }
            else{
                this.stats.Add(new GlobalStatistic(){ type = stat, amount = value, operation = Stats.StatsOperation.Multiply });
            }
        }
    }

    public bool BoolStat(GStatstype stat){
        return this[stat] > 0;
    }

    public object Clone(){
        GlobalStats s = new GlobalStats();
        foreach (var e in this.stats){
            s.stats.Add(new GlobalStatistic(){ type = e.type, amount = e.amount, operation = e.operation });
        }

        return s;
    }

    public static GlobalStats operator *(GlobalStats a, float b){
        GlobalStats ret = new GlobalStats();

        foreach (GlobalStatistic s in a.stats){
            ret[s.type] = (float)(s.amount * b);
        }

        return a;
    }
}