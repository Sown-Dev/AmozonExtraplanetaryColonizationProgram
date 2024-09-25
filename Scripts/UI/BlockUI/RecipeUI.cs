using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeUI: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    private Recipe myRecipe;

    public Image icon;
    public Button button;
    
    public void SetRecipe(Recipe r){
        myRecipe = r;
        icon.sprite = r.icon;
    }


    public void OnPointerEnter(PointerEventData eventData){
        RecipeToolTip.Instance.Open(myRecipe, transform.position);
        if (myRecipe.results.Count == 1){
            ItemInfoUI.Instance.Select(myRecipe.results[0]);
        }
        
        
    }

    public void OnPointerExit(PointerEventData eventData){
        RecipeToolTip.Instance.Close();
        ItemInfoUI.Instance.Deselect();
    }
}