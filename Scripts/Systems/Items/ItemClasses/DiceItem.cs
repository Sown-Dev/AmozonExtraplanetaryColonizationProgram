using Systems.Round;
using UnityEngine;

namespace Systems.Items.ItemClasses{
    [CreateAssetMenu(fileName = "Dice", menuName = "ScriptableObjects/Items/DiceItem", order = 0)]

    public class DiceItem : ThrowableItem{
        public override void Use(Vector2Int pos, Unit user, Slot slot){
            RoundManager.Instance.RegenerateRoundShop();

            base.Use(pos, user, slot);

        }
    }
}