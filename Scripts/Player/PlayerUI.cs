using System;
using Systems.Items;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerUI: MonoBehaviour{
    public static PlayerUI Instance;
    
    [SerializeField] private Player player;


    [SerializeField] public Transform OnTop;
    [Header("UI")] [SerializeField] private UIWindow inventoryUIWindow;
    [SerializeField] private ContainerUI inventoryUI;
    
    [SerializeField] private BlockInfoUI blockInfoUI;
    [SerializeField] private SlicedFilledImage destroyBar;
    [SerializeField] private CanvasGroup destroyBarCG;
    
    private bool inventoryOpen;

    public int toolbarSize = 8;
    public Slot[] toolbar;
    
    void Awake(){
        Instance = this;
        toolbar = new Slot[toolbarSize];
        
        for(int i = 0; i < toolbarSize; i++){
            toolbar[i] = player.Inventory.GetSlot(i);
        }
    }

    private void Start(){
        inventoryUI.Init(player.Inventory);
        inventoryUIWindow.Hide();
        player.SelectSlot(toolbar[0]);
    }

    
    public void Update(){
        destroyBarCG.alpha = player.destroyTimer > 0 ? 1 : 0;
        if(Math.Abs(destroyBar.fillAmount - player.destroyTimer / player.destroyDuration) > 0.01f)
            destroyBar.fillAmount = player.destroyTimer / player.destroyDuration;

        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab)){
            inventoryUIWindow.Toggle();
            inventoryUI.Refresh();
        }

        for (int i = 0; i < toolbarSize; i++){
            
            if (Input.GetKeyDown((i+1).ToString())){
                player.SelectSlot(toolbar[i]);
            }
        }
        
        

        blockInfoUI.block = player.myCursor.lookingBlock;
        
    }
}