using UnityEngine;
using UnityEngine.Tilemaps;

public class Procedural : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase platformTile;
    [SerializeField] private Vector2Int mapSize = new(30, 20);
    [SerializeField] private int borderThickness = 10;
    [SerializeField] private int minPlatformHeight = 2;
    [SerializeField] private int maxPlatformHeight = 4;
    [SerializeField] private int minPlatformWidth = 3;
    [SerializeField] private int maxPlatformWidth = 7;
    [SerializeField] private int[] floorHeights = { 2, 8, 15 };
    [SerializeField] private int[] platformsPerFloor = { 3, 4 };  //number of platforms for upper floors

    private void Start() => GenerateLevel();

    public void GenerateLevel()
    {
        tilemap.ClearAllTiles();

        GenerateBorders();
        GenerateBottomFloor();
        GeneratePlatforms();
    }

    private void GenerateBorders()
    {
        for (int x = -borderThickness; x < mapSize.x + borderThickness; x++)
        {
            for (int y = -borderThickness; y < mapSize.y + borderThickness; y++)
            {
                if (x < 0 || x >= mapSize.x || y < 0 || y >= mapSize.y)
                    tilemap.SetTile(new Vector3Int(x, y, 0), platformTile);
            }
        }
    }

    private void GenerateBottomFloor()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            // Use a gradual slope to smooth out the bottom floor
            int heightVariation = Random.Range(0, 2); // Gentle height variation
            int floorHeight = floorHeights[0] + heightVariation;

            // Avoid single-block-width towers by ensuring a minimum width of 2 tiles
            int tileWidth = Random.Range(2, 4);
            for (int y = 0; y < floorHeight; y++)
            {
                for (int width = 0; width < tileWidth; width++)
                {
                    tilemap.SetTile(new Vector3Int(x + width, y, 0), platformTile);
                }
            }
            x += tileWidth - 1; // Move to the next section after filling
        }
    }

    private void GeneratePlatforms()
    {
        for (int i = 1; i < floorHeights.Length; i++)
        {
            int sectionWidth = mapSize.x / platformsPerFloor[i - 1]; // Divide map into equal sections

            for (int j = 0; j < platformsPerFloor[i - 1]; j++)
            {
                int platformWidth = Random.Range(minPlatformWidth, maxPlatformWidth);
                int platformHeight = Random.Range(minPlatformHeight, maxPlatformHeight);

                // Ensure each platform spawns within its own section
                int sectionStartX = j * sectionWidth;
                int sectionEndX = sectionStartX + sectionWidth - platformWidth;

                // Clamp the xPosition within the section to ensure it doesn't overlap
                int xPosition = Random.Range(sectionStartX, sectionEndX);
                int yPosition = floorHeights[i] + Random.Range(-1, 2);

                for (int x = xPosition; x < xPosition + platformWidth; x++)
                {
                    for (int y = yPosition; y < yPosition + platformHeight; y++)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), platformTile);
                    }
                }
            }
        }
    }
}