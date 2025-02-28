using System.Linq;
using System.Reflection;
using Systems.Items;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI{
    public class TooltipManager : MonoBehaviour{
        public static TooltipManager Instance;


        [FormerlySerializedAs("cg")] [Header("Tooltip Fields")] [SerializeField]
        private CanvasGroup tooltipCG;

        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private RectTransform iconHolder;
        [SerializeField] private Image icon;


        [SerializeField] private Vector2 offset;

        [SerializeField] private RectTransform rt; // Assuming this is already correctly assigned in the inspector


        [SerializeField] private Transform attributeHolder;

        [Header("Attribute Prefabs")] [SerializeField]
        private GameObject rotatable;

        [SerializeField] private GameObject fuel;
        [SerializeField] private GameObject burner;
        [SerializeField] private GameObject electric;
        [SerializeField] private GameObject container;
        [SerializeField] private GameObject actuatable;
        [SerializeField] private GameObject logistics;

        private GameObject tooltipCaller; //the object that called the tooltip. we keep track of it to hide the tooltip when it is destroyed or hidden
        
        // Camera Tooltip (shows a certain place in the world)
        [Header("Camera Tooltip")] 
        [SerializeField]private Camera ttCam;
        [SerializeField] private CanvasGroup camCG;
        [SerializeField] private RectTransform camRT;
        


        private bool isOpen;
        private int width = 228;
        private Vector2 toPos;


        private void Awake(){
            tooltipCG.alpha = 0;
            Instance = this;
            
            HideCameraTooltip();
        }

        private void Update(){
            // cg.alpha = isOpen ? 1 : 0;
            tooltipCG.alpha = Mathf.Lerp(tooltipCG.alpha, isOpen ? 1 : 0, Time.unscaledDeltaTime * 24);

            tooltipCG.alpha = tooltipCG.alpha < 0.1f && !isOpen ? 0 : tooltipCG.alpha;

            if (isOpen){
                rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, toPos, Time.unscaledDeltaTime * 24);
                
                if (tooltipCaller == null || !tooltipCaller.activeSelf){
                    Hide();
                }
            }
            
            
        }


        //called by both. used for clearing the tooltip and other housekeeping things
        public void OnShow(Vector2 pos, bool useOffset){
            // Convert screen position to a position within the canvas
            Vector2 canvasPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.parent, pos, null,
                out canvasPosition);

            if (useOffset)
                canvasPosition += offset;

            Debug.Log($"our original pos is {pos}, end pos is{canvasPosition.x +width} and screen edge thresh is {screenDimensions.x}");

            if (canvasPosition.x + width > screenDimensions.x / 2){
                //Debug.Log("out of bounds");
                canvasPosition.x = screenDimensions.x / 2 - (width);
            }

            //do the same for y
            if (canvasPosition.y - rt.sizeDelta.y -32 < -screenDimensions.y / 2){
                canvasPosition.y = -screenDimensions.y / 2 + (rt.sizeDelta.y + 64);
                canvasPosition.x += 32;
            }

            toPos= canvasPosition;


            foreach (Transform child in attributeHolder){
                Destroy(child.gameObject);
            }

            isOpen = true;
        }

        Vector2 screenDimensions = new Vector2(640, 360);

        public void Show(IToolTippable desc, Vector2 screenPosition,GameObject caller, bool useOffset = true){
            OnShow(screenPosition, useOffset);
            
            if (caller){
                tooltipCaller = caller;
            }

            title.text = desc.name;
            description.text = desc.description;
            icon.sprite = desc.icon;
            costText.transform.parent.gameObject.SetActive(false);
        }

        public void ShowItem(Item item, Vector2 screenPosition,GameObject caller, bool useOffset = true){
            OnShow(screenPosition, useOffset);

            title.text = item.name;
            icon.sprite = item.icon;

            costText.text = "$" + item.value;
            costText.transform.parent.gameObject.SetActive(item.value > 0);

            if (caller){
                tooltipCaller = caller;
            }

            if (item is BlockItem blockItem){
                //blockitem
                description.text = blockItem.blockPrefab.GetDescription().ToString();


                //block attributes
                if (blockItem.blockPrefab.properties.rotatable){
                    Instantiate(rotatable, attributeHolder);
                }

                if (blockItem.blockPrefab.properties.ttFlags.HasFlag(TooltipFlags.isBurner)){
                    Instantiate(burner, attributeHolder);
                }

               
                
                if (blockItem.blockPrefab.properties.ttFlags.HasFlag(TooltipFlags.isActuatable)){
                    Instantiate(actuatable, attributeHolder);
                }

                //attributes based off blockitem category
                if (blockItem.blockCategory.HasFlag(BlockCategory.Electrical)){
                    Instantiate(electric, attributeHolder);
                }
                if (blockItem.blockCategory.HasFlag(BlockCategory.Storage)){
                    Instantiate(container, attributeHolder);
                }
                if (blockItem.blockCategory.HasFlag(BlockCategory.Logistics)){
                    Instantiate(logistics, attributeHolder);
                }
                
            }
            else{
                //is regular item
                description.text = item.description;
            }


            //add shared attributes

            if (item.fuelValue > 0){
                GameObject go = Instantiate(fuel, attributeHolder);
                go.GetComponentInChildren<TMP_Text>().text = $"{item.fuelValue / 20f}s";
            }
        }


        public void Hide(){
            tooltipCaller = null;
            isOpen = TTmouseOver && isOpen; //only close if we are not hovering over the tooltip
        }

        private bool TTmouseOver=false;
        public void OnTTEnter(){
            TTmouseOver = true;
        }
        public void OnTTExit(){
            TTmouseOver = false;
            Hide();
        }
        
        public void ShowCameraTooltip(Vector2 screenPosition, Vector2 camPosition){
            Debug.Log($"showing camera tooltip at {screenPosition} and cam pos {camPosition}");
            int w = 100;
            Vector2 canvasPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.parent, screenPosition, null,
                out canvasPosition);

            
            if (canvasPosition.x + w > screenDimensions.x / 2){
                //Debug.Log("out of bounds");
                canvasPosition.x = screenDimensions.x / 2 - (w);
            }

            //do the same for y
            if (canvasPosition.y - rt.sizeDelta.y -32 < -screenDimensions.y / 2){
                canvasPosition.y = -screenDimensions.y / 2 + (rt.sizeDelta.y + 64);
                canvasPosition.x += 32;
            }
            
            
            camCG.alpha = 1;
            camRT.anchoredPosition = canvasPosition;
            ttCam.transform.position = new Vector3( camPosition.x, camPosition.y, -10);
        }
        public void HideCameraTooltip(){
            camCG.alpha = 0;
        }
        
    }
}