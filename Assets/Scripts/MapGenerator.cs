using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

public enum TileType { Grass, Water, Dirt, Bridge };

[Serializable]
public struct TerrainTileMapping
{
    public TileType type;
    public TileBase tileBase;
}

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    public Vector2Int mapSize;

    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private List<TerrainTileMapping> _tileMappings;

    private List<CurvedLinePath> _rivers;
    private List<CurvedLinePath> _pathes;
    private Vector3 _offset;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _offset = new Vector3(-mapSize.x * 0.5f + 0.5f, 0.0f, 0.0f);
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
        //TestFillPolygon();
    }

    private void Update()
    {
        DebugDrawElements();
    }

    private void DebugDrawElements()
    {
        DebugDrawRiver();
    }

    private void TestFillPolygon()
    {
        List<Vector2Int> vertices = new List<Vector2Int>()
        {
            new Vector2Int(0, 9),
            new Vector2Int(2, 10),
            new Vector2Int(4, 11),
            new Vector2Int(6, 11),
            new Vector2Int(9, 13),
            new Vector2Int(11, 14),
            new Vector2Int(14, 14),
            new Vector2Int(16, 15),
            new Vector2Int(19, 16),
            new Vector2Int(19, 12),
            new Vector2Int(16, 11),
            new Vector2Int(14, 10),
            new Vector2Int(11, 10),
            new Vector2Int(9, 9),
            new Vector2Int(6, 7),
            new Vector2Int(4, 7),
            new Vector2Int(2, 6),
            new Vector2Int(0, 5)
        };
        Polygon poly = new Polygon(vertices);
        poly.GenerateFillingPoints();
        PutTiles(poly.Points, TileType.Water);
    }

    private void DebugDrawRiver()
    {
        if (_rivers != null)
        {
            _rivers.Where(r => r.polygonPointsGenerated).ToList().ForEach(r => DebugDraw(r.PolygonPoints, Color.cyan));
        }
    }

    private void DebugDraw(List<Vector2Int> points, Color color)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 begin = new Vector3(points[i].x, points[i].y, 0) + _offset;
            Vector3 end = new Vector3(points[i + 1].x, points[i + 1].y, 0) + _offset;
            Debug.DrawLine(begin, end, color);
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

        List<Vector2Int> riverControlPoints = new List<Vector2Int>()
        {
            new Vector2Int(0, 49),
            new Vector2Int(49, 54),
            new Vector2Int(74, 69),
            new Vector2Int(99, 74),
        };
        GenerateRiver(riverControlPoints);

        List<Vector2Int> riverControlPoints2 = new List<Vector2Int>()
        {
            new Vector2Int(0, 85),
            new Vector2Int(49, 75),
            new Vector2Int(74, 69)
        };
        GenerateRiver(riverControlPoints2);

        List<Vector2Int> pathControlPoints = new List<Vector2Int>()
        {
            new Vector2Int(49, 0),
            new Vector2Int(49, 49),
            new Vector2Int(20, 100)
        };
        GeneratePath(pathControlPoints);

        List<Vector2Int> pathControlPoints2 = new List<Vector2Int>()
        {
            new Vector2Int(49, 49),
            new Vector2Int(80, 100)
        };
        GeneratePath(pathControlPoints2);
    }

    private void GenerateRiver(List<Vector2Int> riverControlPoints)
    {
        if(_rivers == null)
        {
            _rivers = new List<CurvedLinePath>();
        }
        CurvedLinePath river = new CurvedLinePath(riverControlPoints, 4);
        river.GeneratePoints(0, 5);
        PutTiles(river.AllPoints, TileType.Water);
        _rivers.Add(river);
    }

    private void GeneratePath(List<Vector2Int> pathControlPoints)
    {
        if(_pathes == null)
        {
            _pathes = new List<CurvedLinePath>();
        }
        CurvedLinePath path = new CurvedLinePath(pathControlPoints, 5);
        path.GeneratePoints(0, 8);
        List<Vector2Int> riverPathCrossingPositions = path.AllPoints.Where(p => _rivers.Any(r => r.AllPoints.Contains(p))).ToList();
        List<Vector2Int> pathPositions = path.AllPoints.Where(p => !riverPathCrossingPositions.Contains(p)).ToList();
        PutTiles(pathPositions, TileType.Dirt);
        PutTiles(riverPathCrossingPositions, TileType.Bridge);
        _pathes.Add(path);
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
        TileBase templateTile = _tileMappings.Find(t => t.type == type).tileBase;
        if(templateTile != null)
        {
            TileBase newTile = Instantiate(templateTile);
            Vector3Int pos3D = new Vector3Int(position.x, position.y, 0);
            _tilemap.SetTile(pos3D, newTile);
        }
    }
}
