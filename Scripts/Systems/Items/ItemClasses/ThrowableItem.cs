using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Items.ItemClasses{
    [CreateAssetMenu(fileName = "CartItem", menuName = "ScriptableObjects/Items/DynaItem", order = 0)]
    public class ThrowableItem:Item{
        public Throwable throwPrefab;
        [FormerlySerializedAs("velocity")] public int xVelocity=30;
        public int yVelocity = 10;
        public override void Use(Vector2Int pos, Unit user, Slot slot){
            Throwable throwObject = Instantiate(throwPrefab.gameObject, (Vector2)user.transform.position, Quaternion.identity).GetComponent<Throwable>();
            throwObject.Throw((Vector2)pos, xVelocity, yVelocity);
            Destroy(throwObject.gameObject, 5);
            slot.ItemStack.amount--;
            if (slot.ItemStack.amount <= 0){
                slot.ItemStack = null;
            }
        }
    }
}