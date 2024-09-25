using UnityEngine;

namespace Systems.Items.ItemClasses{
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items/CartItem", order = 0)]
    public class CartItem:Item{
        public GameObject cartPrefab;
        
        public override void Use(Vector2Int pos, Unit user, Slot slot){
            if (Cart.CreateCart(pos, cartPrefab)){
                slot.ItemStack.amount--;
                if (slot.ItemStack.amount <= 0){
                    slot.ItemStack = null;
                }

            };
        }
    }
}