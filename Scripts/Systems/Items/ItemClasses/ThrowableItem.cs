using UnityEngine;

namespace Systems.Items.ItemClasses{
    [CreateAssetMenu(fileName = "CartItem", menuName = "ScriptableObjects/Items/DynaItem", order = 0)]
    public class ThrowableItem:Item{
        public Throwable throwPrefab;
        public int velocity=30;
        public override void Use(Vector2Int pos, Unit user, Slot slot){
            Throwable throwObject = Instantiate(throwPrefab.gameObject, (Vector2)user.transform.position, Quaternion.identity).GetComponent<Throwable>();
            throwObject.Throw((Vector2)pos, velocity, velocity);
            slot.ItemStack.amount--;
            if (slot.ItemStack.amount <= 0){
                slot.ItemStack = null;
            }
        }
    }
}