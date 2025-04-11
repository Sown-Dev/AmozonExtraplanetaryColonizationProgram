using System.Collections.Generic;
using Systems.Items;
using UnityEngine;

namespace NewRunMenu{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "ScriptableObjects/character", order = 0)]
    public class Character : ScriptableObject{
    
        
        [TextArea(3, 10)]
        public string description;
        public string legalName;
        //public int startingMoney; //think i'll remove this
        
        public List<ItemStack> startingItems;
        public List<UpgradeSO> startingUpgrades;
        //stats, but later

        public Sprite portrait;
        public Sprite icon;
        
        //later on, colors for shirt, pants and skin for unique character sprites. Probably just use a replacecolor shader
        public Color shirtColor;
        public Color pantsColor;
        public Color skinColor;

        public Sprite HatSprite;
        
        public Stats stats;
        
    }
}