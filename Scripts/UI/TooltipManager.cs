using System.Linq;
using System.Reflection;
using Systems.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class TooltipManager : MonoBehaviour
    {
        public static TooltipManager Instance;

        
        [Header("Tooltip Fields")]
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private RectTransform iconHolder;
        [SerializeField] private Image icon;

    
        [SerializeField] private Vector2 offset;
    
        [SerializeField] private RectTransform rt; // Assuming this is already correctly assigned in the inspector
       
        
        [SerializeField] private Transform attributeHolder;
        [Header("Attribute Prefabs")] 
        [SerializeField]private GameObject rotatable;
        [SerializeField]private GameObject fuel;
        [SerializeField]private GameObject burner;
        
        
        
        private bool isOpen;

        private void Awake()
        {
            Instance = this;
        }

        private void Update(){
            cg.alpha = isOpen ? 1 : 0;
        
            //cg.alpha = cg.alpha > 0.1f ? cg.alpha : 0;

        }

        private int width = 228;
        //called by both. used for clearing the tooltip and other housekeeping things
        public void OnShow(Vector2 pos, bool useOffset){
            
            // Convert screen position to a position within the canvas
            Vector2 canvasPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.parent, pos, null, out canvasPosition);
    
            if(useOffset)
                canvasPosition += offset;
            
            //Debug.Log($"our original pos is {pos}, end pos is{canvasPosition.x +width} and screen edge thresh is {screenDimensions.x}");

            if(canvasPosition.x+width> screenDimensions.x/2){
                //Debug.Log("out of bounds");
                canvasPosition.x = screenDimensions.x/2 - (width);
            }
            //do the same for y
            if(canvasPosition.y-rt.sizeDelta.y< -screenDimensions.y/2){
                canvasPosition.y = -screenDimensions.y/2 + (rt.sizeDelta.y+64);
                canvasPosition.x += 32;
            }
            
            rt.anchoredPosition = canvasPosition;

            
            foreach (Transform child in attributeHolder){
                Destroy(child.gameObject);
            }
            isOpen = true;

        }
        Vector2 screenDimensions = new Vector2(640,360);
        public void Show(IDescriptable s, Vector2 screenPosition, bool useOffset = true)
        {
            OnShow(screenPosition, useOffset);

            
         
            
            title.text = s.name;
            description.text = s.description;

        }
        
        public void ShowItem(Item item, Vector2 screenPosition, bool useOffset = true)
        {
            OnShow(screenPosition, useOffset);
            
            title.text = item.name;
            icon.sprite = item.icon;
            
            costText.text = "$"+item.value;
            if(item.value<=0){
                costText.transform.parent.gameObject.SetActive(false);
            }
            if (item is BlockItem blockItem){
                //blockitem
                description.text = blockItem.blockPrefab.GetDescription().ToString();
                
                
                //block attributes
                if (blockItem.blockPrefab.properties.rotateable){
                    GameObject go = Instantiate(rotatable, attributeHolder);
                }

                
            }
            else{
                //is regular item
                description.text = item.description;
                
            }
            
            //add attributes
            
            if(item.fuelValue>0){
                GameObject go = Instantiate(fuel, attributeHolder);
                go.GetComponentInChildren<TMP_Text>().text = $"{item.fuelValue/20f}s";
            }
            
        }
        
                

        public void Hide()
        {
            isOpen = false;
        }
    }
}