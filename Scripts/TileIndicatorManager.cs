//attached to ui

using System.Collections.Generic;
using Systems.Block;
using Systems.Block.CustomBlocks;
using UnityEngine;
using Orientation = Systems.Block.Orientation;

public class TileIndicatorManager : MonoBehaviour{
    
    public static TileIndicatorManager Instance;

    public Transform indicatorList;
    public float indicatorAlpha = 0.5f;

    public GameObject[] prefabs; // ordered by IndicatorType
    
    void Awake(){
        Instance = this;
    }
    
    public void DrawIndicators(IEnumerable<TileIndicator> indicators, Vector2Int origin = default, Orientation rot = Orientation.Up){
        Clear();
        foreach (TileIndicator indicator in indicators){
            
            DrawIndicator(indicator.pos.RotateArray(rot, Vector2Int.zero),indicator.type, origin);
        }
    }

    public void Clear(){
        foreach (Transform child in indicatorList){
            Destroy(child.gameObject);
        } 
    }
    
    public void DrawIndicator(Vector2Int[] posList, IndicatorType type, Vector2Int origin = default ){
        
        
        
        //indicator logic for changing color, etc
        foreach (Vector2Int pos in posList){
            Vector2Int myPos = new Vector2Int(pos.x+origin.x, pos.y+origin.y);
            
            GameObject go = Instantiate(prefabs[(int)type], indicatorList);
            go.transform.position = (Vector2)myPos;
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();


            switch (type){
                case IndicatorType.Mining:
                    //check if ore
                    sr.color = TerrainManager.Instance.GetOre(myPos) != null ? Color.green : new Color(0.6f, 0.2f,0.2f);;
                    break;
                case IndicatorType.Harvesting:
                    //check if resourceblock
                    bool isResource = TerrainManager.Instance.GetBlock(myPos) is ResourceBlock;
                    sr.color = isResource ?  new Color(0.2f, 0.8f,0.4f): new Color(0.6f, 0.2f,0.2f);
                    break;
                case IndicatorType.BlockPower:
                    sr.color = TerrainManager.Instance.GetBlock(myPos) is IPowerBlock ? new Color(0.5f, 0.9f,0.1f):new Color(
                        0.8f, 0.5f, 0.3f);
                    break;
                case IndicatorType.PowerConnector:
                    sr.color = TerrainManager.Instance.GetBlock(myPos) is IPowerConnector ? Color.green : new Color(
                        0.8f, 0.5f, 0.3f);
                    break;
                case IndicatorType.InsertingTo:
                    sr.color = TerrainManager.Instance.GetBlock(myPos) is IContainerBlock ? Color.green :  new Color(0.6f, 0.2f,0.2f);
                    break;
                case IndicatorType.ExtractingFrom:
                    sr.color = TerrainManager.Instance.GetBlock(myPos) is IContainerBlock ? Color.green :  new Color(0.6f, 0.2f,0.2f);
                    break;
                default:

                    break;

            }
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, indicatorAlpha);
        }
    }
}