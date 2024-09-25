using UnityEngine;

public class Dynamight : Throwable
{
    public int expRadius;

    public override void Collide()
    {
        base.Collide();

        Vector2 explosionCenter = transform.position;
        for (int x = -expRadius; x <= expRadius; x++)
        {
            for (int y = -expRadius; y <= expRadius; y++)
            {
                Vector2 checkPos = explosionCenter + new Vector2(x, y);
                if (Vector2.Distance(explosionCenter, checkPos) <= expRadius)
                {
                   
                        TerrainManager.Instance.DestroyWall((Vector3Int.RoundToInt(checkPos)));
                    
                }
            }
        }
    }
}