    using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Systems.Block;
using Systems.BlockUI;
using Systems.Items;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.BlockUI{
    public class BlockUI : UIWindow{
        [HideInInspector] public Block block;

        [SerializeField] private GameObject containerUIPrefab;
        [SerializeField] private GameObject progressBarUIPrefab;
        [SerializeField] private GameObject DirectionalSelectUIPrefab;
        [SerializeField] private GameObject RecipeSelectUIPrefab;
        [SerializeField] private GameObject buttonUIPrefab;
        [SerializeField] private GameObject burnerUIPrefab;
        [SerializeField] private GameObject numberSelectorUIPrefab;
        [SerializeField] private GameObject PowerProducerUIPrefab;
        [SerializeField] private GameObject PowerConsumerUIPrefab;
        [SerializeField] private GameObject PowerBatteryUIPrefab;
        [SerializeField] private GameObject BuildingProgressUIPrefab;
        [SerializeField] private GameObject FilterSelectUIPrefab;
        
        
        [SerializeField] private GameObject EmptySpacePrefab;
        
        
        [SerializeField] private GameObject horizListPrefab;

        [SerializeField] private VerticalLayoutGroup windowList;
        
        [SerializeField] private TMP_Text nameText;

        [SerializeField] private Sprite baseButton;

        private void Start(){
            block.UpdateUI += RegenerateUI;
            block.DestroyAction += Close;
            GenerateUIForBlock(block);
        }

        private void Update(){
            if (Vector2.Distance(block.transform.position, Player.Instance.transform.position) > 18)
                Close();
        }

        void OnDestroy(){
            block.UpdateUI -= RegenerateUI;
        }

        void RegenerateUI(){
            GenerateUIForBlock(block);
        }

        public void GenerateUIForBlock(Block block){
            foreach (Transform child in windowList.transform){
                Destroy(child.gameObject);
            }
            nameText.text = block.properties.name;


            // Retrieve all fields from the block that are of a type implementing IBlockComponent
            var componentFields = block.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => typeof(IBlockUI).IsAssignableFrom(field.FieldType))
                .Select(field => field.GetValue(block) as IBlockUI)
                .Where(component => component != null)
                .Where(component => component.Hidden == false)
                .OrderByDescending(component => -component.Priority)
                .ToList();

            // Now componentFields contains all components sorted by Priority
            //convert to regular for loop
            for (int i = 0; i < componentFields.Count; i++){
                var component = componentFields[i];
                if (i < componentFields.Count - 1 && (int)(componentFields[i + 1].Priority/10) == component.Priority/10){
                    Transform list = Instantiate(horizListPrefab, windowList.transform).transform;
                    AddComponent(component, list);
                    while (i < componentFields.Count-1 && (int)(componentFields[i + 1].Priority/10) == component.Priority/10){
                        AddComponent(componentFields[i+1],list );
                        i++;
                        //Debug.Log("SIZE IS " + list.childCount);
                    }
                }
                else{
                    AddComponent(component, windowList.transform);
                }
            }
            if (block is IPowerProducer powerProducer){
                PowerProducerUI gridProducerUI = Instantiate(PowerProducerUIPrefab,  windowList.transform)
                    .GetComponent<PowerProducerUI>();
                gridProducerUI.Init(powerProducer);
                Instantiate(Utils.Instance.EmptyUI, windowList.transform);
                
                /*if( componentFields.Count == 0)
                    Instantiate(Utils.Instance.EmptyUI, windowList.transform);*/
    
                //windowList.padding.bottom = 12;

            }
            else if (block is IPowerConsumer powerConsumer){
               PowerConsumerUI gridConsumerUI = Instantiate(PowerConsumerUIPrefab,   windowList.transform)
                    .GetComponent<PowerConsumerUI>();   
                gridConsumerUI.Init(powerConsumer);
                Instantiate(Utils.Instance.EmptyUI, windowList.transform);
                
               /* if( componentFields.Count == 0)
                    Instantiate(Utils.Instance.EmptyUI, windowList.transform);*/

                //windowList.padding.bottom = 12;


            }else if (block is IPowerBattery powerBattery){
                /* TODO: all this shit
                 gridBatteryUI = Instantiate(PowerBatteryUIPrefab, windowList.transform)
                    .GetComponent<PowerBatteryUI>();
                gridBatteryUI.Init(powerBattery);
                Instantiate(Utils.Instance.EmptyUI, windowList.transform);

                if (componentFields.Count == 0)
                    Instantiate(Utils.Instance.EmptyUI, windowList.transform);

                //windowList.padding.bottom = 12;*/
            }

            
            //refresh
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
        }

        public void AddComponent(IBlockUI component, Transform parent){
            if (component is Container c){
                ContainerUI container = Instantiate(containerUIPrefab, parent).GetComponent<ContainerUI>();
                container.Init(c);
            }
            else if (component is ProgressBar p){
                ProgressBarUI progressBar = Instantiate(progressBarUIPrefab,  parent).GetComponent<ProgressBarUI>();
                progressBar.bar = p;
            }
            else if (component is DirectionSelect d){
                DirectionSelectUI directionSelectUI = Instantiate(DirectionalSelectUIPrefab,  parent)
                    .GetComponent<DirectionSelectUI>();
                directionSelectUI.directionSelect = d;
            }
            else if (component is RecipeSelector r){
                RecipeSelectUI recipeSelectUI =
                    Instantiate(RecipeSelectUIPrefab,  parent).GetComponent<RecipeSelectUI>();
                recipeSelectUI.recipeSelector = r;
            }
            else if (component is BlockUIButton button){
                Button buttonUI = Instantiate(buttonUIPrefab,  parent).GetComponent<Button>();
                buttonUI.image.sprite = button.icon ?? baseButton;
                buttonUI.GetComponent<RectTransform>().sizeDelta =
                    new Vector2(buttonUI.image.sprite.texture.width, buttonUI.image.sprite.texture.height);
                buttonUI.onClick.AddListener(new UnityAction(button.OnClick));
            }
            else if (component is StringBlockUI textBlock){
                TMP_Text textUI = Instantiate(nameText,  parent);
                textUI.text = textBlock.text;
            }
            else if (component is Burner burner){
                BurnerUI burnerUI = Instantiate(burnerUIPrefab,  parent).GetComponent<BurnerUI>();
                burnerUI.Init(burner);
            }
            else if (component is NumberSelector numberSelector){
                NumberSelectorUI numberSelectorUI = Instantiate(numberSelectorUIPrefab,  parent)
                    .GetComponent<NumberSelectorUI>();
                numberSelectorUI.Init(numberSelector);
            }else if (component is BuildingProgress buildingProgress){
                BuildingProgressUI buildingProgressUI = Instantiate(BuildingProgressUIPrefab,  parent)
                    .GetComponent<BuildingProgressUI>();
                buildingProgressUI.Init(buildingProgress);
            }else if(component is Filter filter){
                FilterUI filterSelectUI = Instantiate(FilterSelectUIPrefab,  parent)
                    .GetComponent<FilterUI>();
                filterSelectUI.Init(filter);
            }
            
        }

      

        public override void Close(){
            OnClose?.Invoke();
            BlockUIManager.Instance.CloseBlockUI();
        }
    }
}