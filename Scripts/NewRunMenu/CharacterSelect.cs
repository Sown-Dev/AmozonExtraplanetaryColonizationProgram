using System.Collections.Generic;
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

        public Toggle DevModeToggle;

        public GameObject charOptionPrefab;
        public Transform charList;

        public List<CharacterOption> charOptions = new List<CharacterOption>();
        void Start(){
            //selectedChar = GameManager.Instance.selectedChar;
            foreach (Transform child in charList){
                Destroy(child.gameObject);
            }
            charOptions.Clear();
            
            foreach (Character c in GameManager.Instance.allCharacters){
                GameObject go = Instantiate(charOptionPrefab, charList);
                CharacterOption co = go.GetComponent<CharacterOption>();
                co.Init(c, this);
                charOptions.Add(co);
                
            }
            charOptions[0].Select();
            

            startButton.onClick.AddListener(() => StartGame());
            startButton.interactable = selectedChar != null;

            //devmode   
            DevModeToggle.gameObject.SetActive(false);
#if ALLITEMS1
            DevModeToggle.isOn = GameManager.Instance.settings.DevMode;
            DevModeToggle.gameObject.SetActive(true);
#endif
        }

        public void SetDevMode(bool b){
            GameManager.Instance.settings.DevMode = b;
            Debug.Log("DevMode is now " + GameManager.Instance.settings.DevMode);
        }


        public void SelectChar(Character c){
            if (c == selectedChar) return;
            
            foreach (CharacterOption co in charOptions){
                co.button.interactable = true;
            }
            
            GameManager.Instance.selectedChar = c;
            
            barCode.Rotate(new Vector3(0, 0, 180));
            selectedChar = c;
            charName.text = c.legalName.Replace(" ", "\n");
            ;
            charTitle.text = c.name;
            charDescription.text = c.description;

            portrait.sprite = c.portrait;

            idCard.color = c.shirtColor * 0.3f + Color.white * 0.7f;

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
            GameManager.Instance.currentWorld.playerCharacter = selectedChar.name;
            GameManager.Instance.StartNewRun();
        }

        public void Back(){
            GameManager.Instance.ExitToMain();
        }
    }
}