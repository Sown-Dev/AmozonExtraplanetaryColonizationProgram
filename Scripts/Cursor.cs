
using System;
using Systems.Block;
using Systems.Items;
using Systems.Terrain;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Terrain = Systems.Terrain.Terrain;

public class Cursor : MonoBehaviour{
    public static Cursor Instance;
    
   [SerializeField] public SpriteRenderer buildingPreview;

[SerializeField]public SpriteRenderer sr;
[SerializeField] public SpriteRenderer directionArrow;
    
    
    public UnityEvent<Vector2Int> OnClick;
    public UnityEvent<Vector2Int> OnRightClick;

    private Vector2Int lastPos;
    public Vector2Int currentPos;

    public Orientation cursorRotation;
    
    public Block lookingBlock;
    public Ore lookingOre;

    public Terrain lookingTerrain;

    //public Transform player;

    private void Awake(){
        Instance = this;
    }

    void Update()
    {
        if(CursorManager.Instance.uiDepth > 0) return;
        
        currentPos=Vector2Int.RoundToInt( Camera.main.ScreenToWorldPoint(Input.mousePosition));

       /* int range = 8;
        //set pos in range of player pos +- range
        Vector2Int playerPos = Vector2Int.RoundToInt(player.position); // player is a transform field
        currentPos.x = Mathf.Clamp(currentPos.x, playerPos.x - range, playerPos.x + range);
        currentPos.y = Mathf.Clamp(currentPos.y, playerPos.y - range, playerPos.y + range);*/
        
        
        
        if (currentPos!=lastPos){
            Refresh();
        }
        
        //TODO: fix placing while dragging (maybe make it only work if the original click was not on ui, swtich to onmousedown, and use my own bool)
        if(EventSystem.current.IsPointerOverGameObject()|| EventSystem.current.IsInvoking()) return;
        
        if (Input.GetMouseButtonDown(0)){
            OnClick.Invoke( Vector2Int.RoundToInt(currentPos));
            Refresh();
        }

        if (Input.GetMouseButtonDown(1)){
            OnRightClick.Invoke( Vector2Int.RoundToInt(currentPos));
            Refresh();
        }
        if(Input.GetKeyDown(KeyCode.R)){
            cursorRotation = cursorRotation.next();
            Debug.Log(cursorRotation.GetAngle());
        }
       

        //rotate in 2d
        // Get the current rotation (as a quaternion) and the target rotation angle
        Quaternion currentRotation = directionArrow.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, cursorRotation.GetAngle());

// Slerp between the current rotation and the target rotation
        directionArrow.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * 24f);

    }

    void Refresh(){
        transform.position = (Vector2)currentPos;
        lastPos = currentPos;

        lookingBlock?.Deselect();
        lookingBlock = TerrainManager.Instance.GetBlock(currentPos);
        lookingBlock?.Select();

        lookingOre = TerrainManager.Instance.GetOre(currentPos);
        if (lookingOre != null)
            OreInfoUI.Instance.Select(lookingOre);
        else{
            OreInfoUI.Instance.Deselect();
        }






        lookingTerrain = TerrainManager.Instance.GetTerrain(currentPos);
    }

    private void OnDrawGizmos(){
        Handles.Label(transform.position, cursorRotation.ToString());
    }
}
