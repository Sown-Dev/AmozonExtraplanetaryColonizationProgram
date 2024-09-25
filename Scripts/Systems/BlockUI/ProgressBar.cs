using Systems.BlockUI;

public class ProgressBar : IBlockUI{
    public int progress;
    public int maxProgress=100;

    public int Priority{ get; set; }
    public bool Hidden{ get; set; }

    public ProgressBar(){
        
    }
    
    public ProgressBar(int priority){
        this.Priority = priority;
    }
    
    public bool ResetIfFull(){
        if(progress >= maxProgress){
            progress = 0;
            return true;
        }
        return false;
    }
}