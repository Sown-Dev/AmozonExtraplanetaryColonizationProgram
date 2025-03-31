using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldButton: MonoBehaviour{
    private World myWorld;
    public TMP_Text worldNameText;
    public Button deleteButton;
    //public TitleScreen titleScreen;
    public void Init(World w){
        myWorld = w;
        worldNameText.text = w.name;
        
        deleteButton.onClick.AddListener(() => {
            GameManager.Instance.DeleteWorld(w);
            Destroy(gameObject);
        });
    }
    public void OnClick(){
        GameManager.Instance.LoadWorld(myWorld);
    }
}