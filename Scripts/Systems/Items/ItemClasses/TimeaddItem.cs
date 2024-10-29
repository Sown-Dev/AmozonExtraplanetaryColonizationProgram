using Systems.Round;
using UnityEngine;

namespace Systems.Items.ItemClasses{
    [CreateAssetMenu(fileName = "Hourglass", menuName = "ScriptableObjects/Items/TimeAddItem", order = 0)]

    public class TimeaddItem :Item{

        public int timeAdd = 30;
        public override void Use(Vector2Int pos, Unit user, Slot slot){
            base.Use(pos, user, slot);
            RoundManager.Instance.AddTime(timeAdd);

        }
    }
}