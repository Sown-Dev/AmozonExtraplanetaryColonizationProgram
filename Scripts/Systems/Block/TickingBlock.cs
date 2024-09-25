using UI.BlockUI;
using UnityEngine;

namespace Systems.Block{
    public class TickingBlock : Block{
        public virtual void Tick(){
            if (TerrainManager.ticks % 2 == 0){
                actuatedThisTick = false;
                if (properties.actuatable)
                    mat.SetColor("_AddColor", new Color(0, 0, 0, 0));

                currentState.SetNextSprite();
                UpdateSprite();
            }
        }

        bool actuatedThisTick = false;

        public override void Actuate(){
            if (actuatedThisTick) return;
            actuatedThisTick = true;
            base.Actuate();
            if (properties.actuatable)
                mat.SetColor("_AddColor", new Color(0.25f, 0.1f, 0.05f, 0));
        }

        public override void Use(Unit user){
            base.Use(user);
            BlockUIManager.Instance.GenerateBlockUI(this);
        }
    }
}