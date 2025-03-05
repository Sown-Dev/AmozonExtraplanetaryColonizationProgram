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

public class Cursor : MonoBehaviour
{
    public static Cursor Instance;

    [SerializeField] public SpriteRenderer buildingPreview;
    [SerializeField] public SpriteRenderer sr;
    [SerializeField] public SpriteRenderer directionArrow;

    [FormerlySerializedAs("OnClick")] public UnityEvent<Vector2Int> OnLeftClick;
    public UnityEvent<Vector2Int> OnRightClick;
    public UnityEvent<Vector2Int> OnCTRLClick;

    private Vector2Int lastPos;
    public Vector2Int currentPos;

    public Orientation cursorRotation;

    public Block lookingBlock;
    public Ore lookingOre;

    public Terrain lookingTerrain;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(Time.timeScale<=0) return;
        
        // Update current position based on mouse position
        currentPos = Vector2Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        // Refresh cursor state if position has changed
        if (currentPos != lastPos)
        {
            Refresh();
        }

        // Check if the pointer is over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Handle mouse clicks
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                OnCTRLClick.Invoke(currentPos);
            }
            else
            {
                OnLeftClick.Invoke(currentPos);
            }
            Refresh();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnRightClick.Invoke(currentPos);
            Refresh();
        }

        // Rotate cursor with the R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            cursorRotation = cursorRotation.next();
            Debug.Log(cursorRotation.GetAngle());
        }

        // Smoothly rotate the direction arrow
        Quaternion currentRotation = directionArrow.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, cursorRotation.GetAngle());
        directionArrow.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * 24f);
    }

    void Refresh()
    {
        transform.position = (Vector2)currentPos;
        lastPos = currentPos;

        lookingBlock?.Deselect();
        lookingBlock = TerrainManager.Instance.GetBlock(currentPos);
        lookingBlock?.Select();

        lookingOre = TerrainManager.Instance.GetOre(currentPos);
        if (lookingOre != null)
        {
            OreInfoUI.Instance.Select(lookingOre);
        }
        else
        {
            OreInfoUI.Instance.Deselect();
        }

        lookingTerrain = TerrainManager.Instance.GetTerrain(currentPos);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, cursorRotation.ToString());
    }
#endif
}
