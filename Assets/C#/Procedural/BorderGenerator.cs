using UnityEngine;
using UnityEngine.Tilemaps;

public class BorderGenerator : MonoBehaviour
{
    public Tilemap borderTilemap;
    public TileBase borderTile;
    public Vector2Int mapSize;
    private readonly int borderThickness = 10;

    private void Start() => GenerateBorder();

    private void GenerateBorder()
    {
        for (int x = -borderThickness; x < mapSize.x + borderThickness; x++)
        {
            for (int y = -borderThickness; y < 0; y++)
                borderTilemap.SetTile(new Vector3Int(x, y, 0), borderTile);
            for (int y = mapSize.y; y < mapSize.y + borderThickness; y++)
                borderTilemap.SetTile(new Vector3Int(x, y, 0), borderTile);
        }

        for (int y = -borderThickness; y < mapSize.y + borderThickness; y++)
        {
            for (int x = -borderThickness; x < 0; x++)
                borderTilemap.SetTile(new Vector3Int(x, y, 0), borderTile);
            for (int x = mapSize.x; x < mapSize.x + borderThickness; x++)
                borderTilemap.SetTile(new Vector3Int(x, y, 0), borderTile);
        }
    }
}
