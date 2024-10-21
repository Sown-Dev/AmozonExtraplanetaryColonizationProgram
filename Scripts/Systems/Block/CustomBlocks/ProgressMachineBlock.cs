using UnityEngine;

namespace Systems.Block.CustomBlocks{
    public class ProgressMachineBlock : TickingBlock, IMachineBlock{
        public ProgressBar progressBar;

        public int speed{ get; set; } = 1;
        [field: SerializeField]public int baseSpeed{ get; set; } = 1;

        protected override void Awake(){
            base.Awake();

            progressBar = new ProgressBar(21);
            progressBar.maxProgress = 100;
            progressBar.progress = 0;
        }

        public override void Tick(){
            base.Tick();
            if (!CanProgress()){
                //progressBar.progress=0; TODO idk if this is necessary, but well see
                return;
            }
            
            Progress();
        }

        public virtual void Progress(){
            speed = baseSpeed;
            progressBar.progress+= speed;
            if (progressBar.ResetIfFull()){
               CompleteCycle();
            }
        }

        public virtual bool CanProgress(){
            return true;
        }

        //what it does when progress complete
        public virtual void CompleteCycle(){ }
    }
}

public interface IMachineBlock{
    public int baseSpeed{ get; set; }
    public int speed{ get; set; }
}