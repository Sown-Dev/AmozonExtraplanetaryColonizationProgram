using Systems.Round;
using UnityEngine;

namespace Systems.Items.ItemClasses{
    [CreateAssetMenu(fileName = "Dice", menuName = "ScriptableObjects/Items/DiceItem", order = 0)]

    public class DiceItem : ThrowableItem{
        public override void Use(Vector2Int pos, Unit user, Slot slot){
            base.Use(pos, user, slot);
            RoundManager.Instance.RegenerateRoundShop();

        }
    }
}