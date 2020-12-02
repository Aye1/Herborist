using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;
using Sirenix.OdinInspector;
using CreativeSpore.SuperTilemapEditor;

public enum TileType { Grass, Water, Dirt, Bridge };

[Serializable]
public struct TerrainTileMapping
{
    public TileType type;
    public int tileId;
    public int brushId;
}

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    public Vector2Int mapSize;

    [Required]
    public STETilemap steTilemap;
    [Required]
    public List<TerrainTileMapping> tileMappings;

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
        DebugDrawPath();
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

    private void DebugDrawPath()
    {
        if(_pathes != null)
        {
            _pathes.Where(p => p.polygonPointsGenerated).ToList().ForEach(p => DebugDraw(p.PolygonPoints, Color.black));
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

    public void GenerateMap()
    {
        for(int i=0; i<mapSize.x; i++)
        {
            for(int j=0; j<mapSize.y; j++)
            {
                PutTile(new Vector2Int(i, j), TileType.Grass);
            }
        }
        OffsetMapPosition();

        List<Vector2Int> riverControlPoints = new List<Vector2Int>()
        {
            MapGeneratorHelper.GenerateRandomPointOnLimit(MapLimit.Left, 0.20f, 0.60f),
            new Vector2Int(69, 49),
            MapGeneratorHelper.GenerateRandomPointOnLimit(MapLimit.Right, 0.50f, 0.80f)
        };
        GenerateRiver(riverControlPoints);

        List<Vector2Int> riverControlPoints2 = new List<Vector2Int>()
        {
            MapGeneratorHelper.GenerateRandomPointOnLimit(MapLimit.Left, 0.65f, 0.90f),
            new Vector2Int(69, 49)
        };
        GenerateRiver(riverControlPoints2);

        List<Vector2Int> pathControlPoints = new List<Vector2Int>()
        {
            new Vector2Int(49, 0),
            new Vector2Int(49, 29),
            MapGeneratorHelper.GenerateRandomPointOnLimit(MapLimit.Up, 0.10f, 0.50f)
        };
        GeneratePath(pathControlPoints);

        List<Vector2Int> pathControlPoints2 = new List<Vector2Int>()
        {
            new Vector2Int(49, 29),
            MapGeneratorHelper.GenerateRandomPointOnLimit(MapLimit.Up, 0.60f, 0.90f)
        };
        GeneratePath(pathControlPoints2);
    }

    private void OffsetMapPosition()
    {
        steTilemap.transform.position = new Vector3(-mapSize.x * 0.5f, -0.5f, steTilemap.transform.position.z);
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
        TerrainTileMapping mapping = tileMappings.Find(x => x.type == type);
        steTilemap.SetTile(position.x, position.y, mapping.tileId, mapping.brushId);
    }
}
