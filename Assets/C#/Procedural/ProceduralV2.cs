using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralV2 : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase tile;
    public TileBase borderTile;
    public TileBase platformTile;

    public int width = 100;
    public int height = 100;
    public float scale = 20f;

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        tilemap.ClearAllTiles();
        GenerateTerrain();
        AddBorders();
        CreatePlatforms();
    }

    void GenerateTerrain()
    {
        float offsetX = Random.Range(0f, 10000f);
        float offsetY = Random.Range(0f, 10000f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float perlinValue = Mathf.PerlinNoise((x + offsetX) / scale, (y + offsetY) / scale);
                if (perlinValue > 0.5f) // Adjust threshold for terrain shape
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
    }

    void AddBorders()
    {
        for (int thickness = 0; thickness < 10; thickness++)
        {
            for (int x = thickness; x < width - thickness; x++)
            {
                tilemap.SetTile(new Vector3Int(x, thickness, 0), borderTile);         // Bottom border
                tilemap.SetTile(new Vector3Int(x, height - 1 - thickness, 0), borderTile); // Top border
            }

            for (int y = thickness; y < height - thickness; y++)
            {
                tilemap.SetTile(new Vector3Int(thickness, y, 0), borderTile);         // Left border
                tilemap.SetTile(new Vector3Int(width - 1 - thickness, y, 0), borderTile); // Right border
            }
        }
    }

    void CreatePlatforms()
    {
        int middleY = height / 2;
        int topY = (height * 3) / 4;
        int platformWidth = 10;

        // Bottom floor platforms
        for (int x = 0; x < width; x += platformWidth * 2)
        {
            for (int y = 0; y < 2; y++) // Bottom area height adjusted to 1-2 blocks high
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), platformTile);
            }
        }

        // Middle floor platforms
        for (int x = 0; x < width; x += platformWidth * 2)
        {
            for (int y = middleY - 1; y < middleY + 1; y++) // Narrower middle area
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), platformTile);
            }
        }

        // Top floor platforms
        for (int x = 0; x < width; x += platformWidth * 2)
        {
            for (int y = topY - 1; y < topY + 1; y++) // Narrower top area
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), platformTile);
            }
        }
    }
}

//try to make path from one corner to the other 3, if path can't be created, generate map again.