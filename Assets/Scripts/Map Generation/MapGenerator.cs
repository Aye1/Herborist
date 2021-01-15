using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Sirenix.OdinInspector;
using CreativeSpore.SuperTilemapEditor;

public enum TileType { Grass, Water, Dirt, Bridge, MapBorder, Tree, Grass256, Dirt256 };

[Serializable]
public struct TerrainTileMapping
{
    public TileType type;
    public int tileId;
    public int brushId;
    public STETilemap tilemap;
    public Color minimapColor;
}

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    public Vector2Int mapSize;
    public bool DontGenerateMap;


    [Required]
    public List<TerrainTileMapping> tileMappings;

    [Required]
    [SceneObjectsOnly]
    [SerializeField]
    private STETilemap _terrainTilemap;

    [SerializeField, Required, AssetsOnly] private PointOfInterest _entrancePOI;

    private List<CurvedLinePath> _rivers;
    private List<CurvedLinePath> _pathes;
    private List<Vector2Int> _bridgePositions;
    private Polygon _borderPolygon;
    private ForestGenerator _forestGenerator;
    private List<PointOfInterest> _spawnedPOIs;

    private int _firstPathWidth = 5;
    private int _borderWidth = 2;
    private Vector3 _playerStartPosition;
    private PlayerMovement _player;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _playerStartPosition = new Vector3(mapSize.x * 0.5f, 5, 0.0f);
            _forestGenerator = GetComponent<ForestGenerator>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetPlayerPosition();
        if (!DontGenerateMap)
        {
            GenerateMap();
            SetPositionsToAvoid();
            _forestGenerator.size = mapSize.x;
            _forestGenerator.LaunchTreesGeneration();
            //TestFillPolygon();
        }
    }

    private void Update()
    {
        DebugDrawElements();
    }

    private void SetPlayerPosition()
    {
        _player = PlayerMovement.Instance;
        _player.transform.position = _playerStartPosition;
    }

    private void SetPositionsToAvoid()
    {
        _spawnedPOIs?.ForEach(poi => _forestGenerator?.posToAvoid.AddRange(poi.TilesPositions));
        _rivers?.ForEach(r => _forestGenerator?.posToAvoid.AddRange(r.AllPoints));
        _pathes?.ForEach(p => _forestGenerator?.posToAvoid.AddRange(p.AllPoints));
        if(_bridgePositions != null)
            _forestGenerator?.posToAvoid.AddRange(_bridgePositions);
        if(_borderPolygon != null)
            _forestGenerator?.posToAvoid.AddRange(_borderPolygon.Points);
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
        _spawnedPOIs = new List<PointOfInterest>();
        GenerateEntrancePOI();


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
            new Vector2Int(49, 6),
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
        GenerateMapBorders(_borderWidth);

        tileMappings.Select(m => m.tilemap).Distinct().ToList().ForEach(t => t?.UpdateMeshImmediate()); // Force updating tilemaps before ending the loading + only way to disable colliders
        DisableColliders(_terrainTilemap, _bridgePositions);

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
        if(_bridgePositions == null)
        {
            _bridgePositions = new List<Vector2Int>();
        }
        CurvedLinePath path = new CurvedLinePath(pathControlPoints, _firstPathWidth);
        path.GeneratePoints(0, 4);
        List<Vector2Int> riverPathCrossingPositions = path.AllPoints.Where(p => _rivers.Any(r => r.AllPoints.Contains(p))).ToList();
        List<Vector2Int> pathPositions = path.AllPoints.Where(p => !riverPathCrossingPositions.Contains(p)).ToList();
        //List<Vector2Int> pathPositions = path.AllPoints;
        PutTiles(pathPositions, TileType.Dirt);
        PutTiles(riverPathCrossingPositions, TileType.Bridge);
        _pathes.Add(path);
        _bridgePositions.AddRange(riverPathCrossingPositions);
    }

    private void GenerateMapBorders(int width)
    {

        /**
         * Polygon points creation, starting bottom left
         *    +--->
         *    +------------------+ +
         *    | +--------------+ | |
         *    | |      <---+   | | |
         *    | | +            | | v
         *    | | |            | |
         *    | | |          ^ | |
         *    | | v          | | |
         *    | |            | | |
         * ^  | |            + | |
         * |  | |        +---> | |
         * |  | +----+  +------+ |
         * +  +------+  +--------+
         *           S       <---+
         * */

        int halfSizeX = mapSize.x / 2 - 3; // TODO: change this arbitrary constant value (if needed)
        List< Vector2Int> polygonBoundaries = new List<Vector2Int>()
        {
            new Vector2Int(halfSizeX-_firstPathWidth / 2, -2),
            new Vector2Int(-1, -2),
            new Vector2Int(-1, mapSize.y),
            new Vector2Int(mapSize.x, mapSize.y),
            new Vector2Int(mapSize.x, -2),
            new Vector2Int(halfSizeX + _firstPathWidth, -2),
            new Vector2Int(halfSizeX + _firstPathWidth + width, width),
            new Vector2Int(mapSize.x - width, width),
            new Vector2Int(mapSize.x - width, mapSize.y - width),
            new Vector2Int(width, mapSize.y - width),
            new Vector2Int(width, width),
            new Vector2Int(halfSizeX - _firstPathWidth/2 - width, width),
            new Vector2Int(halfSizeX - _firstPathWidth/2, -2)
        };
        _borderPolygon = new Polygon(polygonBoundaries);
        _borderPolygon.GenerateFillingPoints();
        PutTiles(_borderPolygon.Points, TileType.MapBorder);
    }

    private void GenerateEntrancePOI()
    {
        PointOfInterest entrancePOI = Instantiate(_entrancePOI, transform);
        entrancePOI.SetPositionOnTilemap(new Vector2(mapSize.x * 0.5f - entrancePOI.Size.x / 2, -0.5f));
        _spawnedPOIs.Add(entrancePOI);
    }

    private void PutTiles(List<Vector2Int> positions, TileType type)
    {
        foreach(Vector2Int pos in positions)
        {
            PutTile(pos, type);
        } 
    }

    private IEnumerator PutTilesAsync(List<Vector2Int> positions, TileType type)
    {
        int i = 0;
        Debug.Log(positions.Count);
        while (i < positions.Count)
        {
            PutTile(positions[i], type);
            Debug.Log(i);
            yield return null;
            i++;
        }
    }

    private void PutTile(Vector2Int position, TileType type)
    {
        TerrainTileMapping mapping = tileMappings.Find(x => x.type == type);
        mapping.tilemap.SetTile(position.x, position.y, mapping.tileId, mapping.brushId);
    }

    private void DisableColliders(STETilemap tilemap, List<Vector2Int> positions)
    {
        positions.ForEach(p => DisableCollider(tilemap, p));
    }

    private void DisableCollider(STETilemap tilemap, Vector2Int position)
    {
        GameObject tile = tilemap.GetTileObject(position.x, position.y);
        Tile t = tilemap.GetTile(position.x, position.y);
        if (tile == null)
        {
            Debug.LogWarning("Can't find tile at pos (" + position.x + "," + position.y + ")");
        }
        else
        {
            tile.GetComponent<Collider2D>().enabled = false;
        }
    }

    public TileType GetTypeAtPos(Vector2Int position)
    {
        if(_borderPolygon != null && _borderPolygon.Points.Contains(position))
        {
            return TileType.MapBorder;
        }

        if(_bridgePositions != null && _bridgePositions.Contains(position))
        {
            return TileType.Bridge;
        }

        if(_forestGenerator != null && _forestGenerator.HasTreeAtPos(position))
        {
            return TileType.Tree;
        }

        if(_pathes != null && _pathes.Any(p => p.AllPoints.Contains(position)))
        {
            return TileType.Dirt;
        }

        if(_rivers != null && _rivers.Any(r => r.AllPoints.Contains(position)))
        {
            return TileType.Water;
        }

        return TileType.Grass;
    }

    public TileType GetTypeAtPos(int x, int y)
    {
        return GetTypeAtPos(new Vector2Int(x, y));
    }

    public Vector2Int GetPosOnTilemap(Vector3 position)
    {
        Vector3 worldPosition = position;
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
    }

    #region Debug & Test
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
        if (_pathes != null)
        {
            _pathes.Where(p => p.polygonPointsGenerated).ToList().ForEach(p => DebugDraw(p.PolygonPoints, Color.black));
        }
    }

    private void DebugDraw(List<Vector2Int> points, Color color)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            // The +1 are due to the floor done when snapping to grid
            Vector3 begin = new Vector3(points[i].x + 1, points[i].y + 1, 0);
            Vector3 end = new Vector3(points[i + 1].x + 1, points[i + 1].y + 1, 0);
            Debug.DrawLine(begin, end, color);
        }
    }
    #endregion
}
