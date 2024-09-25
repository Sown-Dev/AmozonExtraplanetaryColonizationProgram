using System.Collections.Generic;

public partial class TerrainManager{
    public GlobalStats finalStats;
    
    public GlobalStats baseStats;
    
    public List<WorldUpgrade> upgrades = new ();
    
    
    public void calculateStats(){
        finalStats = (GlobalStats)baseStats.Clone();
        foreach (WorldUpgrade u in upgrades){
            finalStats.Combine(u.stats); 
        }
    }
    
    public void AddUpgrade(WorldUpgrade upgrade){
        upgrade.Init();
        upgrades.Add(upgrade);
        calculateStats();
    }
}