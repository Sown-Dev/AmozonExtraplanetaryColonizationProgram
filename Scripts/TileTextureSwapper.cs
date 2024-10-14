using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBorderPixelSwapper : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap
    public Texture2D tileTexture; // The texture of the tile

    // How many pixels from the edge should be swapped (border size)
    public int borderSize = 1;

    // Call this method to apply pixel swapping to all borders
    public void SwapTileBorderPixels()
    {
        // Loop through all the positions in the Tilemap
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                TileBase currentTile = tilemap.GetTile(tilePosition);

                // Check if there is a tile at the current position
                if (currentTile != null)
                {
                    // Check the neighboring tiles (left, right, top, bottom)
                    TileBase tileLeft = tilemap.GetTile(tilePosition + Vector3Int.left);
                    TileBase tileRight = tilemap.GetTile(tilePosition + Vector3Int.right);
                    TileBase tileUp = tilemap.GetTile(tilePosition + Vector3Int.up);
                    TileBase tileDown = tilemap.GetTile(tilePosition + Vector3Int.down);

                    // If a neighboring tile is different, swap the pixels on that border
                    if (tileLeft != currentTile)
                        SwapPixels(tilePosition, Vector3Int.left);
                    if (tileRight != currentTile)
                        SwapPixels(tilePosition, Vector3Int.right);
                    if (tileUp != currentTile)
                        SwapPixels(tilePosition, Vector3Int.up);
                    if (tileDown != currentTile)
                        SwapPixels(tilePosition, Vector3Int.down);
                }
            }
        }
    }

    // Method to swap pixels on the border of two tiles
    private void SwapPixels(Vector3Int tilePosition, Vector3Int direction)
    {
        // Calculate the border area to swap pixels between tiles
        int tileSize = tileTexture.width; // Assuming square tiles for simplicity
        Color[] currentTilePixels = tileTexture.GetPixels(0, 0, tileSize, tileSize);

        // Swap pixels at the border in the specified direction
        for (int i = 0; i < tileSize; i++)
        {
            for (int j = 0; j < borderSize; j++)
            {
                int borderPixelIndex;
                int adjacentPixelIndex;

                // Determine which pixels to swap based on the direction
                if (direction == Vector3Int.left)
                {
                    borderPixelIndex = i * tileSize + j; // Left border pixels
                    adjacentPixelIndex = i * tileSize + (tileSize - 1 - j); // Right border of neighbor
                }
                else if (direction == Vector3Int.right)
                {
                    borderPixelIndex = i * tileSize + (tileSize - 1 - j); // Right border pixels
                    adjacentPixelIndex = i * tileSize + j; // Left border of neighbor
                }
                else if (direction == Vector3Int.up)
                {
                    borderPixelIndex = (tileSize - 1 - j) * tileSize + i; // Top border pixels
                    adjacentPixelIndex = j * tileSize + i; // Bottom border of neighbor
                }
                else // Down
                {
                    borderPixelIndex = j * tileSize + i; // Bottom border pixels
                    adjacentPixelIndex = (tileSize - 1 - j) * tileSize + i; // Top border of neighbor
                }

                // Swap pixels at the border
                Color temp = currentTilePixels[borderPixelIndex];
                currentTilePixels[borderPixelIndex] = currentTilePixels[adjacentPixelIndex];
                currentTilePixels[adjacentPixelIndex] = temp;
            }
        }

        // Apply the swapped pixels back to the texture (important to update the texture)
        tileTexture.SetPixels(0, 0, tileSize, tileSize, currentTilePixels);
        tileTexture.Apply(); // Apply the changes to the texture
    }
}
