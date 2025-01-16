using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase tile;
    public Vector2Int mapSize;

    [SerializeField] private GameObject[] largeDecorations;
    [SerializeField] private GameObject[] smallDecorations;
    [SerializeField] private GameObject[] grassDecorations;

    private readonly List<GameObject> spawnedDecorations = new();
    GameObject decorationsParent;

    private void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        GenerateMap();
    }

    public void GenerateMap()
    {
        ClearGeneratedTiles();
        ClearGeneratedDecorations();
        GenerateBottomFloor();
        GeneratePlatformFloor();
        GenerateTopFloor();
        SpawnDecorations();
    }

    void ClearGeneratedTiles()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
                tilemap.SetTile(new Vector3Int(x, y, 0), null);
        }
    }

    private void ClearGeneratedDecorations()
    {
        foreach (var obj in spawnedDecorations)
            if (obj != null) Destroy(obj);
        spawnedDecorations.Clear();
    }

    private void GenerateBottomFloor()
    {
        int baseY = 1;
        int smoothness = 4;
        int maxHeightVariation = 3;

        for (int x = 0; x < mapSize.x; x++)
        {
            int floorHeight = baseY + Mathf.FloorToInt(Mathf.PerlinNoise(x / (float)smoothness, 0) * maxHeightVariation);

            if (x > 0 && x < mapSize.x - 1)
            {
                int prevHeight = baseY + Mathf.FloorToInt(Mathf.PerlinNoise((x - 1) / (float)smoothness, 0) * maxHeightVariation);
                int nextHeight = baseY + Mathf.FloorToInt(Mathf.PerlinNoise((x + 1) / (float)smoothness, 0) * maxHeightVariation);
                floorHeight = (floorHeight + prevHeight + nextHeight) / 3;
            }

            for (int y = 0; y <= floorHeight; y++)
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        }
    }

    private void GeneratePlatformFloor()
    {
        int centerY = mapSize.y / 2;
        int maxPlatformWidth = 8;
        int minPlatformWidth = 5;
        int platformHeightVariation = 3;

        for (int x = 0; x < mapSize.x;)
        {
            int platformWidth = Random.Range(minPlatformWidth, maxPlatformWidth + 1);
            int platformY = centerY + Random.Range(-platformHeightVariation, platformHeightVariation + 1);

            for (int w = 0; w < platformWidth && x + w < mapSize.x; w++)
            {
                for (int h = 0; h < Random.Range(1, 3); h++)
                {
                    tilemap.SetTile(new Vector3Int(x + w, platformY + h, 0), tile);
                }
            }

            int gapWidth = Random.Range(1, 3);
            if (gapWidth > 1 && Random.value > 0.4f)
            {
                int extraPlatformY = centerY + Random.Range(-platformHeightVariation, platformHeightVariation + 1);
                int extraPlatformWidth = Random.Range(2, 4);
                for (int extraW = 0; extraW < extraPlatformWidth && x + extraW < mapSize.x; extraW++)
                {
                    tilemap.SetTile(new Vector3Int(x + extraW, extraPlatformY, 0), tile);
                }
            }

            x += platformWidth + gapWidth;
        }
    }

    private void GenerateTopFloor()
    {
        int topFloorHeight = mapSize.y - 5;
        int maxPlatformWidth = 8;
        int minPlatformWidth = 5;
        int heightVariation = 1;

        for (int x = 0; x < mapSize.x;)
        {
            int platformWidth = Random.Range(minPlatformWidth, maxPlatformWidth + 1);
            int platformY = topFloorHeight + Random.Range(-heightVariation, heightVariation + 1);

            for (int w = 0; w < platformWidth && x + w < mapSize.x; w++)
            {
                for (int h = 0; h < Random.Range(1, 3); h++)
                {
                    tilemap.SetTile(new Vector3Int(x + w, platformY + h, 0), tile);
                }
            }

            int gapWidth = Random.Range(1, 3);
            if (gapWidth > 1 && Random.value > 0.4f)
            {
                int extraPlatformY = topFloorHeight + Random.Range(-heightVariation, heightVariation + 1);
                int extraPlatformWidth = Random.Range(2, 4);
                for (int extraW = 0; extraW < extraPlatformWidth && x + extraW < mapSize.x; extraW++)
                {
                    tilemap.SetTile(new Vector3Int(x + extraW, extraPlatformY, 0), tile);
                }
            }

            x += platformWidth + gapWidth;
        }
    }

    private void SpawnDecorations()
    {
        if(decorationsParent == null)
            decorationsParent = new("Decorations");

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                // Check if there's a tile and if there's no tile above it (empty space)
                if (tilemap.HasTile(new Vector3Int(x, y, 0)) && !tilemap.HasTile(new Vector3Int(x, y + 1, 0)))
                {
                    // Get the world position of the tile
                    Vector3 tilePosition = tilemap.CellToWorld(new Vector3Int(x, y + 1, 0));
                    // Center the decoration only on the X-axis (adjust X to center, keep Y same)
                    tilePosition.x += tilemap.cellSize.x / 2;

                    // Spawn a large decoration with a low probability (e.g., 5%)
                    if (Random.Range(0f, 1f) < 0.05f && largeDecorations.Length > 0)
                    {
                        GameObject largeDecorationPrefab = largeDecorations[Random.Range(0, largeDecorations.Length)];
                        GameObject obj = Instantiate(largeDecorationPrefab, tilePosition, Quaternion.identity);
                        obj.transform.SetParent(decorationsParent.transform); // Set the parent to the new empty GameObject
                        spawnedDecorations.Add(obj);
                    }
                    // Spawn a small decoration with a medium probability (e.g., 15%)
                    else if (Random.Range(0f, 1f) < 0.15f && smallDecorations.Length > 0)
                    {
                        GameObject smallDecorationPrefab = smallDecorations[Random.Range(0, smallDecorations.Length)];
                        GameObject obj = Instantiate(smallDecorationPrefab, tilePosition, Quaternion.identity);
                        obj.transform.SetParent(decorationsParent.transform); // Set the parent to the new empty GameObject
                        spawnedDecorations.Add(obj);
                    }
                    // Spawn grass with a higher probability (e.g., 50%)
                    else if (Random.Range(0f, 1f) < 0.50f && grassDecorations.Length > 0)
                    {
                        GameObject grassDecorationPrefab = grassDecorations[Random.Range(0, grassDecorations.Length)];
                        GameObject obj = Instantiate(grassDecorationPrefab, tilePosition, Quaternion.identity);
                        obj.transform.SetParent(decorationsParent.transform); // Set the parent to the new empty GameObject
                        spawnedDecorations.Add(obj);
                    }
                }
            }
        }
    }

}

/*---------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*---------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*---------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

//using UnityEngine;
//using UnityEngine.Tilemaps;

//public class MapGenerator : MonoBehaviour
//{
//    public Tilemap tilemap;
//    public TileBase tile;
//    public Vector2Int mapSize;

//    private void Start()
//    {
//        GenerateMap();
//    }

//    public void GenerateMap()
//    {
//        ClearGeneratedTiles();
//        GenerateBottomFloor();
//        GeneratePlatformFloor();
//        GenerateTopFloor();
//    }

//    void ClearGeneratedTiles()
//    {
//        for (int x = 0; x < mapSize.x; x++)
//        {
//            for (int y = 0; y < mapSize.y; y++)
//                tilemap.SetTile(new Vector3Int(x, y, 0), null);
//        }
//    }

//    private void GenerateBottomFloor()
//    {
//        int baseY = 1; // Closest to the bottom
//        int smoothness = 4; // Wider sections for smoother look

//        for (int x = 0; x < mapSize.x; x++)
//        {
//            // Smooth variation using Perlin noise and randomness
//            int floorHeight = baseY + Mathf.FloorToInt(Mathf.PerlinNoise(x / (float)smoothness, 0) * 2) + Random.Range(0, 2);
//            for (int y = 0; y <= floorHeight; y++)
//            {
//                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
//            }
//        }
//    }

//    private void GeneratePlatformFloor()
//    {
//        int centerY = mapSize.y / 2; // Center of the map
//        int maxPlatformWidth = 8;
//        int minPlatformWidth = 5;
//        int platformHeightVariation = 3;

//        for (int x = 0; x < mapSize.x;)
//        {
//            int platformWidth = Random.Range(minPlatformWidth, maxPlatformWidth + 1);
//            int platformY = centerY + Random.Range(-platformHeightVariation, platformHeightVariation + 1);

//            for (int w = 0; w < platformWidth && x + w < mapSize.x; w++)
//            {
//                for (int h = 0; h < Random.Range(1, 3); h++) // Height variation for platforms
//                {
//                    tilemap.SetTile(new Vector3Int(x + w, platformY + h, 0), tile);
//                }
//            }

//            // Move to the next platform start, ensuring 1-2 block gaps
//            x += platformWidth + Random.Range(1, 3);
//        }
//    }

//    private void GenerateTopFloor()
//    {
//        int topFloorHeight = mapSize.y - 5; // Lowered to be 4-5 tiles below the top border
//        int maxPlatformWidth = 8;
//        int minPlatformWidth = 5;

//        for (int x = 0; x < mapSize.x;)
//        {
//            int platformWidth = Random.Range(minPlatformWidth, maxPlatformWidth + 1);
//            for (int w = 0; w < platformWidth && x + w < mapSize.x; w++)
//            {
//                for (int h = 0; h < Random.Range(1, 3); h++) // Variation in height for visual interest
//                {
//                    tilemap.SetTile(new Vector3Int(x + w, topFloorHeight + h, 0), tile);
//                }
//            }
//            // Shift to the next section, ensuring gaps of 1-2 between top floor platforms
//            x += platformWidth + Random.Range(1, 3);
//        }
//    }
//}

