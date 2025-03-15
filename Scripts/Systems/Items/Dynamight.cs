using UnityEngine;

public class Dynamight : Throwable
{
    public float expRadius;

    public override void Collide()
    {
        base.Collide();

        Vector2 explosionCenter = transform.position;
        for (int x = (int)-expRadius; x <= expRadius; x++)
        {
            for (int y = (int)-expRadius-1; y <= expRadius+1; y++)
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