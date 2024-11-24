//attached to ui

using System.Collections.Generic;
using Systems.Block.CustomBlocks;
using UnityEngine;

public class TileIndicatorManager : MonoBehaviour{

    public Transform indicatorList;
    public float indicatorAlpha = 0.5f;

    public GameObject[] prefabs; // ordered by IndicatorType
    
    public void DrawIndicators(IEnumerable<TileIndicator> indicators, Vector2Int origin = default){
        foreach (Transform child in indicatorList){
            Destroy(child.gameObject);
        }
        foreach (TileIndicator indicator in indicators){
            DrawIndicator(indicator, origin);
        }
    }
    
    public void DrawIndicator(TileIndicator indicator, Vector2Int origin = default){
        
        
        
        //indicator logic for changing color, etc
        foreach (Vector2Int pos in indicator.pos){
            Vector2Int myPos = new Vector2Int(pos.x+origin.x, pos.y+origin.y);
            
            GameObject go = Instantiate(prefabs[(int)indicator.type], indicatorList);
            go.transform.position = (Vector2)myPos;
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();


            switch (indicator.type){
                case IndicatorType.Mining:
                    //check if ore
                    sr.color = TerrainManager.Instance.GetOre(myPos) != null ? Color.green : Color.red;
                    break;
                case IndicatorType.Extracting:
                    //check if resourceblock
                    bool isResource = TerrainManager.Instance.GetBlock(myPos) is ResourceBlock;
                    sr.color = isResource ?  new Color(0.2f, 0.8f,0.4f): new Color(0.4f, 0.2f,0.2f);
                    break;
                case IndicatorType.BlockPower:
                    sr.color = TerrainManager.Instance.GetBlock(myPos) is IPowerBlock ? new Color(0.5f, 0.9f,0.1f): new Color(
                        0.9f, 0.7f, 0.3f);
                    break;
                case IndicatorType.PowerConnector:
                    sr.color = TerrainManager.Instance.GetBlock(myPos) is IPowerConnector ? Color.green : new Color(
                        0.9f, 0.7f, 0.3f);
                    break;
                default:

                    break;

            }
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, indicatorAlpha);
        }
    }
}