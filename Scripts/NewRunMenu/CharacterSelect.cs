using Systems.Items;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NewRunMenu{
    public class CharacterSelect : MonoBehaviour{

        public Character selectedChar;
        
        
        public TMP_Text charName;
        public TMP_Text charTitle;
        public TMP_Text charDescription;
        
        public Transform itemsList;
        public GameObject itemPrefab;
        public Transform upgradesList;
        public GameObject upgradePrefab; 
        public Image portrait;
        public Image idCard;
        public RectTransform barCode;
        public Button startButton;

        
        
        public GameObject charOptionPrefab;
        public Transform charList;
        
        

        void Start(){
            foreach (Transform child in charList){
                Destroy(child.gameObject);
            }
            foreach (Character c in GameManager.Instance.allCharacters){
                GameObject go = Instantiate(charOptionPrefab, charList);
                go.GetComponent<CharacterOption>().Init(c, this);
                if (selectedChar == null){
                    SelectChar(c);
                }
            }
            
            startButton.onClick.AddListener(() => StartGame());
            startButton.interactable = selectedChar != null;
        }


        public void SelectChar(Character c){
            if(c == selectedChar) return;
            barCode.Rotate(new Vector3(0, 0, 180));
            selectedChar = c;
            charName.text = c.legalName.Replace(" ", "\n");;
            charTitle.text = c.name;
            charDescription.text = c.description;
            
            portrait.sprite = c.portrait;

            idCard.color = c.shirtColor * 0.5f + Color.white * 0.5f;
            
            foreach (Transform child in itemsList){
                Destroy(child.gameObject);
            }

            foreach (ItemStack i in c.startingItems){
                GameObject go = Instantiate(itemPrefab, itemsList);
                go.GetComponent<ItemStackUI>().Init(i);
            }
            foreach (Transform child in upgradesList){
                Destroy(child.gameObject);
            }
            foreach (UpgradeSO u in c.startingUpgrades){
                GameObject go = Instantiate(upgradePrefab, upgradesList);
                go.GetComponent<UpgradeUI>().Init(u);
            }
            
            startButton.interactable = true;
        }
        
        public void StartGame(){
            GameManager.Instance.selectedChar = selectedChar;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    }
}