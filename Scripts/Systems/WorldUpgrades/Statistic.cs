using System;

[Serializable]
public class GlobalStatistic{
    public GlobalStatistic(){
        type = default;
        amount = 0;
        operation = Stats.StatsOperation.Add;
    }

    public GlobalStatistic(GStatstype _type, double amt, Stats.StatsOperation op){
        type = _type;
        amount = amt;
        operation = op;
    }
    
    public GStatstype type;
    public double amount;
    public Stats.StatsOperation operation;
}