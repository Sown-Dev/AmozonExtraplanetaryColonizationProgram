using System;

[Serializable]
public class Statistic{
    public Statistic(){
        type = default;
        amount = 0;
        operation = GlobalStats.StatsOperation.Add;
    }

    public Statistic(Statstype _type, double amt, GlobalStats.StatsOperation op){
        type = _type;
        amount = amt;
        operation = op;
    }
    
    public Statstype type;
    public double amount;
    public GlobalStats.StatsOperation operation;
}