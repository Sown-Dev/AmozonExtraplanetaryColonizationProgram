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

        
        
        public GameObject charOptionPrefab;


        void Awake(){
            foreach (Character c in GameManager.Instance.allCharacters){
                GameObject go = Instantiate(charOptionPrefab, transform);
                go.GetComponent<CharacterOption>().Init(c, this);
            }
        }


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