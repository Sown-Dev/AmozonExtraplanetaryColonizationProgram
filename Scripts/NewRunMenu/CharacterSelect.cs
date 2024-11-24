using Systems.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewRunMenu{
    public class CharacterSelect : MonoBehaviour{

        public Character selectedChar;
        
        
        public TMP_Text charName;
        public TMP_Text charDescription;
        public Transform itemsList;
        public GameObject itemPrefab;
        /*public Transform upgradesList;
        public GameObject upgradePrefab; */
        public Image portrait;



        public void SelectChar(Character c){
            selectedChar = c;
            charName.text = c.name;
            charDescription.text = c.description;
            portrait.sprite = c.portrait;
            
            foreach (Transform child in itemsList){
                Destroy(child.gameObject);
            }

            foreach (ItemStack i in c.startingItems){
                GameObject go = Instantiate(itemPrefab, itemsList);
                go.GetComponent<ItemStackUI>().Init(i);
            }
        }
    }
}