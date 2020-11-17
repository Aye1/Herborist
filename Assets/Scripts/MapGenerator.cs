using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { Grass, Water };

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    public Vector2Int mapSize;

    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private TileBase _grassTileBase;
    [SerializeField] private TileBase _waterTileBase;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CleanMap();
        GenerateMap();
    }

    public void CleanMap()
    {
        _tilemap.ClearAllTiles();
    }

    public void GenerateMap()
    {
        for(int i=0; i<mapSize.x; i++)
        {
            for(int j=0; j<mapSize.y; j++)
            {
                PutTile(new Vector2Int(i, j), TileType.Grass);
            }
        }
        _grid.transform.position = new Vector3(-mapSize.x * 0.5f, -0.5f, _grid.transform.position.z);

        GenerateRiver();
    }

    private void GenerateRiver()
    {
        Vector2Int begin = MapGeneratorHelper.GeneratePointOnLimit(MapLimit.Left, 5, 18, mapSize);
        Vector2Int end = MapGeneratorHelper.GeneratePointOnLimit(MapLimit.Down, 2, 14, mapSize);
        List<Vector2Int> fullLine = new List<Vector2Int>() { begin, end };
        MapGeneratorHelper.SubdivideLine(fullLine, begin, end, 3);
        MapGeneratorHelper.FillLine(fullLine);
        PutTiles(fullLine, TileType.Water);
    }

    private void PutTiles(List<Vector2Int> positions, TileType type)
    {
        foreach(Vector2Int pos in positions)
        {
            PutTile(pos, type);
        } 
    }

    private void PutTile(Vector2Int position, TileType type)
    {
        TileBase templateTile = null;
        switch(type)
        {
            case TileType.Grass:
                templateTile = _grassTileBase;
                break;
            case TileType.Water:
                templateTile = _waterTileBase;
                break;
        }
        TileBase newTile = Instantiate(templateTile);
        Vector3Int pos3D = new Vector3Int(position.x, position.y, 0);
        _tilemap.SetTile(pos3D, newTile);
    }
}
