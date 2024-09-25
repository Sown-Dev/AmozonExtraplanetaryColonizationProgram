using System;
using System.Linq;
using System.Reflection;
using Systems.Block;
using Systems.BlockUI;
using Systems.Items;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.BlockUI{
    public class BlockUI : UIWindow{
        [HideInInspector]public Block block;

        [SerializeField] private GameObject containerUIPrefab;
        [SerializeField] private GameObject progressBarUIPrefab;
        [SerializeField] private GameObject DirectionalSelectUIPrefab;
        [SerializeField] private GameObject RecipeSelectUIPrefab;
[SerializeField] private GameObject buttonUIPrefab;
        
        [SerializeField] private Transform windowTransform;
        [SerializeField] private TMP_Text nameText;

        [SerializeField] private Sprite baseButton;

        private void Start(){
            block.UpdateUI += RegenerateUI;
            block.DestroyAction += Close;
            GenerateUIForBlock(block);
        }

        private void Update(){
            if(Vector2.Distance(block.transform.position, Player.Instance.transform.position) > 18)
                Close();
        }

        void OnDestroy(){
            block.UpdateUI -= RegenerateUI;
        }

        void RegenerateUI(){
            GenerateUIForBlock(block);
        }
        
        public void GenerateUIForBlock(Block block){
            
            foreach (Transform child in windowTransform){
                Destroy(child.gameObject);
            }
            
            // Retrieve all fields from the block that are of a type implementing IBlockComponent
            var componentFields = block.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => typeof(IBlockUI).IsAssignableFrom(field.FieldType))
                .Select(field => field.GetValue(block) as IBlockUI)
                .Where(component => component != null)
                .OrderByDescending(component => -component.Priority)
                .ToList();

            // Now componentFields contains all components sorted by Priority
            foreach (var component in componentFields){
                AddComponent(component);
            }
            nameText.text = block.properties.name;
             
        }
        public void AddComponent(IBlockUI component){
            if (component is Container container){
                GenerateContainerUI(container);
            }
            else if (component is ProgressBar progressBar){
                GenerateProgressBarUI(progressBar);
            }
            else if (component is DirectionSelect directionalSelect){
                GenerateDirectionSelectUI(directionalSelect);
            }
            else if (component is RecipeSelector recipeSelector){
                GenerateRecipeSelectorUI(recipeSelector);
            }
            else if (component is BlockUIButton button){
                AddButton(button);
            }
            else if (component is StringBlockUI textBlock){
                AddText(textBlock.text);
            }
        }
        public void AddText(string text){
            TMP_Text textUI = Instantiate(nameText, windowTransform);
            textUI.text = text;
        }
        public void AddButton(BlockUIButton button){
            Button buttonUI = Instantiate(buttonUIPrefab, windowTransform).GetComponent<Button>();
            buttonUI.image.sprite = button.icon ?? baseButton;
            buttonUI.GetComponent<RectTransform>().sizeDelta =
                new Vector2(buttonUI.image.sprite.texture.width, buttonUI.image.sprite.texture.height);
            buttonUI.onClick.AddListener(new UnityAction(button.OnClick));
        }

        private void GenerateContainerUI(Container c){
            ContainerUI container= Instantiate(containerUIPrefab, windowTransform).GetComponent<ContainerUI>();
            container.Init(c);
        }
    
        private void GenerateProgressBarUI(ProgressBar p){
            ProgressBarUI progressBar= Instantiate(progressBarUIPrefab, windowTransform).GetComponent<ProgressBarUI>();
            progressBar.bar = p;
        }
        
        private void GenerateDirectionSelectUI(DirectionSelect d){
            DirectionSelectUI directionSelectUI= Instantiate(DirectionalSelectUIPrefab, windowTransform).GetComponent<DirectionSelectUI>();
            directionSelectUI.directionSelect = d;
        }
        
        private void GenerateRecipeSelectorUI(RecipeSelector r){
            RecipeSelectUI recipeSelectUI= Instantiate(RecipeSelectUIPrefab, windowTransform).GetComponent<RecipeSelectUI>();
            recipeSelectUI.recipeSelector = r;
        }

        public override void Close(){
            OnClose?.Invoke();
            BlockUIManager.Instance.CloseBlockUI();
        }
    }
}