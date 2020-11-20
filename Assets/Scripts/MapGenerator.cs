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

    private River _generatedRiver;

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
        TestFillPolygon();
    }

    private void Update()
    {
        //DebugDrawElements();
    }

    private void DebugDrawElements()
    {
        DebugDrawRiver();
    }

    private void TestFillPolygon()
    {
        List<Vector2Int> vertices = new List<Vector2Int>()
        {
            new Vector2Int(12,12),
            new Vector2Int(12, 18),
            new Vector2Int(18, 22),
            new Vector2Int(30, 12),
            new Vector2Int(30, 18),
            new Vector2Int(24, 12)
        };
        List<Vector2Int> fillingPoints = Polygon.GenerateFillingPoints(vertices);
        PutTiles(fillingPoints, TileType.Water);
    }

    private void DebugDrawRiver()
    {
        if (_generatedRiver != null && _generatedRiver.mainPointsGenerated)
        {
            Vector2Int[] points = _generatedRiver.mainPoints.ToArray();
            Vector3 offset = new Vector3(-mapSize.x * 0.5f + 0.5f, 0.0f, 0.0f);
            for (int i = 0; i < _generatedRiver.mainPoints.Count - 1; i++)
            {
                Vector3 begin = new Vector3(points[i].x, points[i].y, 0) + offset;
                Vector3 end = new Vector3(points[i + 1].x, points[i + 1].y, 0) + offset;
                Debug.DrawLine(begin, end, Color.cyan);
            }
        }
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

        //GenerateRiver();
    }

    private void GenerateRiver()
    {
        MapLimit beginLimit = MapLimit.Left;
        MapLimit endLimit = MapLimit.Right;
        _generatedRiver = new River();
        _generatedRiver.beginLimit = beginLimit;
        _generatedRiver.endLimit = endLimit;
        _generatedRiver.GenerateMainPoints();
        _generatedRiver.MoveMainPointsRandomly(0, 3);
        _generatedRiver.GenerateInterpolationPoints();

        PutTiles(_generatedRiver.allPoints, TileType.Water);
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
