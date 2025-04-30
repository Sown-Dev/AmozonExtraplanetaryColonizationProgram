using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Custom Rule Tile")]
public class CustomRuleTile : RuleTile
{
    public bool hasCollider = true;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        tileData.colliderType = hasCollider ? Tile.ColliderType.Sprite : Tile.ColliderType.None;
    }
}