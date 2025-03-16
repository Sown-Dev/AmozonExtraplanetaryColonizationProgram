using UI.BlockUI;
using UnityEngine;

namespace Systems.Block
{
    public class TickingBlock : Block
    {
        public new TickingBlockData data => (TickingBlockData)base.data;

        public virtual void Tick() { }

        public void ResetActuated()
        {
            data.actuatedThisTick = false;
            if (properties.actuatable)
                mat.SetColor("_AddColor", new Color(0, 0, 0, 0));

            currentState.SetNextSprite();
            UpdateSprite();
        }

        public override void Actuate()
        {
            if (data.actuatedThisTick)
                return;
            data.actuatedThisTick = true;
            base.Actuate();
            if (properties.actuatable)
                mat.SetColor("_AddColor", new Color(0.25f, 0.1f, 0.05f, 0));
        }

        public override void Use(Unit user)
        {
            base.Use(user);
        }
    }

    public class TickingBlockData : BlockData
    {
        public bool actuatedThisTick = false;
    }
}
