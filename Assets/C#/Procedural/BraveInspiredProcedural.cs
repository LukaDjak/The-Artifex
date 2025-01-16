using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BraveInspiredProcedural : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase terrainTile;
    public TileBase platformTile;
    public TileBase borderTile;

    public int width = 100;
    public int height = 100;
    public int minPlatformWidth = 4;
    public int maxPlatformWidth = 8;
    public int minPlatformHeight = 1;
    public int maxPlatformHeight = 3;
    public int verticalSpacing = 4;     // Vertical spacing between platform layers
    public int maxJumpHeight = 3;       // Max jump height for player
    public int borderThickness = 10;    // Thickness of the borders

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        tilemap.ClearAllTiles();
        AddBorders();
        GeneratePlatforms();
    }

    void AddBorders()
    {
        for (int thickness = 0; thickness < borderThickness; thickness++)
        {
            for (int x = -thickness; x <= width + thickness; x++)
            {
                tilemap.SetTile(new Vector3Int(x, -thickness, 0), borderTile);                  // Bottom border
                tilemap.SetTile(new Vector3Int(x, height + thickness - 1, 0), borderTile);      // Top border
            }
            for (int y = -thickness; y < height + thickness; y++)
            {
                tilemap.SetTile(new Vector3Int(-thickness, y, 0), borderTile);                  // Left border
                tilemap.SetTile(new Vector3Int(width + thickness - 1, y, 0), borderTile);       // Right border
            }
        }
    }

    void GeneratePlatforms()
    {
        int currentY = verticalSpacing;

        while (currentY < height - verticalSpacing)
        {
            int x = 0;
            List<int> gapPositions = new List<int>();

            // Generate platforms with structured spacing
            while (x < width)
            {
                // Calculate platform width and height
                int platformWidth = Mathf.Max(Random.Range(minPlatformWidth, maxPlatformWidth), minPlatformWidth);
                int platformHeight = Mathf.Max(Random.Range(minPlatformHeight, maxPlatformHeight), minPlatformHeight);

                // Check bounds for platform width
                if (x + platformWidth > width) platformWidth = width - x;

                int layerOffset = Random.Range(-1, 2); // Slight height variation per platform

                // Place the platform tiles
                for (int px = x; px < x + platformWidth; px++)
                {
                    for (int py = currentY + layerOffset; py < currentY + layerOffset + platformHeight; py++)
                    {
                        if (py < height) // Ensure we're not going out of bounds
                        {
                            tilemap.SetTile(new Vector3Int(px, py, 0), platformTile);
                        }
                    }
                }

                // Record the end of this platform for possible gaps
                gapPositions.Add(x + platformWidth);
                x += platformWidth; // Move to the next position after the platform
            }

            // Determine where to create gaps
            int gapCount = Random.Range(2, 4); // 2 to 3 gaps
            for (int i = 0; i < gapCount; i++)
            {
                // Select a random gap position from gapPositions ensuring unique gaps
                if (gapPositions.Count > 0)
                {
                    int gapStart = Random.Range(0, gapPositions.Count - 1); // Randomly select a position
                    int gapWidth = Mathf.Max(Random.Range(minPlatformWidth / 2, maxPlatformWidth), 2); // Minimum gap width

                    // Ensure gap does not exceed width
                    if (gapPositions[gapStart] + gapWidth > width)
                        gapWidth = width - gapPositions[gapStart];

                    // Clear the gap in the tilemap
                    for (int px = gapPositions[gapStart]; px < gapPositions[gapStart] + gapWidth; px++)
                    {
                        for (int py = currentY; py < currentY + maxPlatformHeight; py++)
                        {
                            tilemap.SetTile(new Vector3Int(px, py, 0), null); // Clear tile for gap
                        }
                    }

                    // Remove the gap position to avoid creating a gap there again
                    gapPositions.RemoveAt(gapStart);
                }
            }

            // Move up for the next layer
            currentY += Mathf.Max((int)(maxJumpHeight * 0.75f) + verticalSpacing, minPlatformHeight + verticalSpacing);
        }
    }
}