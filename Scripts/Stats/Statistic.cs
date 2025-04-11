using System;

[Serializable]
public class Statistic{
     public Statstype type;
    public double amount;
    public Stats.StatsOperation operation;
    public Statistic(){
        type = Statstype.InventorySlots;
        amount = 0;
        operation = Stats.StatsOperation.Add;
    }

    public Statistic(Statstype _type, double amt, Stats.StatsOperation op){
        type = _type;
        amount = amt;
        operation = op;
    }
    
   
}