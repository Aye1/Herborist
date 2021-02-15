using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Sirenix.OdinInspector;
using CreativeSpore.SuperTilemapEditor;

public enum TileType { Grass, Water, Dirt, Bridge, MapBorder, Tree };

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

    private List<Vector2Int> _testSatisfiesConditionList;

    public bool DontGenerateMap;
    [Title("Map Generation Parameters")]
    public Vector2Int mapSize;
    public int pathWidth = 5;
    public int riverWidth = 4;
    [Range(1,10)]
    public int numberPathes = 2;
    [Range(1,10)]
    public int numberRivers = 2;
    [Range(0,20)]
    public int pathSubdivisions = 5;
    [Range(0,20)]
    public int riverSubdivisions = 5;
    [MinMaxSlider(0,10, ShowFields = true)]
    public Vector2Int pathAlea;
    [MinMaxSlider(0,10, ShowFields = true)]
    public Vector2Int riverAlea;

    [Title("Tiles")]
    [Required]
    public List<TerrainTileMapping> tileMappings;

    [Required]
    [SceneObjectsOnly]
    [SerializeField]
    private STETilemap _terrainTilemap;

    [SerializeField, Required, AssetsOnly] private PointOfInterest _entrancePOI;

    private List<CurvedLinePath> _rivers;
    private List<CurvedLinePath> _pathes;
    private List<PointOfInterest> _spawnedPOIs;
    private Polygon _borderPolygon;
    private ForestGenerator[] _forestGenerators;

    private Vector2Int _minPosition;
    private Vector2Int _maxPosition;
    private List<Vector2Int> _poiPositions;
    private List<Vector2Int> _riverPositions;
    private List<Vector2Int> _pathPositions;
    private List<Vector2Int> _bridgePositions;
    private List<Vector2Int> _usedPositions;

    private int _borderWidth = 2;
    private Vector3 _playerStartPosition;
    private PlayerMovement _player;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _playerStartPosition = new Vector3(mapSize.x * 0.5f, 5, 0.0f);
            _forestGenerators = GetComponents<ForestGenerator>();
            _usedPositions = new List<Vector2Int>();
            _testSatisfiesConditionList = new List<Vector2Int>();
            _minPosition = Vector2Int.zero;
            _maxPosition = new Vector2Int(mapSize.x - 1, mapSize.y - 1);
            _riverPositions = new List<Vector2Int>();
            _pathPositions = new List<Vector2Int>();
            _bridgePositions = new List<Vector2Int>();
            _poiPositions = new List<Vector2Int>();
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
            foreach(ForestGenerator generator in _forestGenerators)
            {
                generator.posToAvoid.AddRange(_usedPositions);
                generator.size = mapSize.x;
                generator.LaunchTreesGeneration();
                _usedPositions.AddRange(generator.TreePositions);
            }
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
        _spawnedPOIs?.ForEach(poi => _usedPositions.AddRange(poi.TilesPositions));
        _rivers?.ForEach(r => _usedPositions.AddRange(r.AllPoints));
        _pathes?.ForEach(p => _usedPositions.AddRange(p.AllPoints));
        if(_bridgePositions != null)
            _usedPositions.AddRange(_bridgePositions);
        if(_borderPolygon != null)
            _usedPositions.AddRange(_borderPolygon.Points);
    }

    public void GenerateMap()
    {
        GenerateBasicGrass();
        GenerateMapBorders(_borderWidth);

        _spawnedPOIs = new List<PointOfInterest>();
        GenerateEntrancePOI();

        GenerateRivers();

        GeneratePathes();

        TestCondition();

        tileMappings.Select(m => m.tilemap).Distinct().ToList().ForEach(t => t?.UpdateMeshImmediate()); // Force updating tilemaps before ending the loading + only way to disable colliders
        //DisableColliders(_terrainTilemap, _bridgePositions);

    }

    private void GenerateBasicGrass()
    {
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                PutTile(new Vector2Int(i, j), TileType.Grass);
            }
        }
    }

    private void GenerateRivers()
    {
        List<MapCondition> riverSpawnConditions = new List<MapCondition>()
        {
            new IsFarFromTileCondition(_poiPositions, _minPosition, _maxPosition, 5),
            new IsCloseToTileCondition(_borderPolygon.Points, _minPosition, _maxPosition, 1)
        };
        // Generate rivers
        for (int i = 0; i < numberRivers; i++)
        {
            //MapLimit originLimit = (MapLimit)Alea.GetIntInc(0, 3);
            List<Vector2Int> controlPoints = new List<Vector2Int>();
            Vector2Int origin = MapCondition.GetRandomPointSatisfyingAll(riverSpawnConditions);
            //TODO: this is way too slow, how can we make it quicker?
            IsFarFromTileCondition riverLengthCondition = new IsFarFromTileCondition(origin, _minPosition, _maxPosition, 50);
            riverSpawnConditions.Add(riverLengthCondition);
            controlPoints.Add(origin);
            controlPoints.Add(MapCondition.GetRandomPointSatisfyingAll(riverSpawnConditions));
            //controlPoints.Add(MapGeneratorHelper.GenerateRandomPointOnLimit(originLimit, 0.2f, 0.8f));
            //controlPoints.Add(MapGeneratorHelper.GenerateRandomPointOnRandomLimitExcluded(originLimit, 0.2f, 0.8f));
            GenerateRiver(controlPoints);
            riverSpawnConditions.Remove(riverLengthCondition);
        }
    }

    private void GenerateRiver(List<Vector2Int> riverControlPoints)
    {
        if(_rivers == null)
        {
            _rivers = new List<CurvedLinePath>();
        }
        CurvedLinePath river = new CurvedLinePath(riverControlPoints, riverWidth);
        river.subdivisionCount = riverSubdivisions;
        river.GeneratePoints(riverAlea.x, riverAlea.y);
        PutTiles(river.AllPoints, TileType.Water);
        _rivers.Add(river);
        _riverPositions.AddRange(river.AllPoints);
    }

    private void GeneratePathes()
    {
        // Generate first path
        List<Vector2Int> firstPathControlPoints = new List<Vector2Int>()
        {
            new Vector2Int(47,2),
            new Vector2Int(49,29),
            MapGeneratorHelper.GenerateRandomPointOnRandomLimitExcluded(MapLimit.Down, 0.3f, 0.7f)
        };
        GeneratePath(firstPathControlPoints);

        // Generate pathes
        for (int i = 1; i < numberPathes; i++)
        {
            MapLimit originLimit = (MapLimit)Alea.GetIntInc(0, 3);
            List<Vector2Int> controlPoints = new List<Vector2Int>();
            controlPoints.Add(MapGeneratorHelper.GenerateRandomPointOnLimit(originLimit, 0.2f, 0.8f));
            controlPoints.Add(MapGeneratorHelper.GenerateRandomPointOnRandomLimitExcluded(originLimit, 0.2f, 0.8f));
            GeneratePath(controlPoints);
        }
    }

    private void GeneratePath(List<Vector2Int> pathControlPoints)
    {
        if(_pathes == null)
        {
            _pathes = new List<CurvedLinePath>();
        }
        CurvedLinePath path = new CurvedLinePath(pathControlPoints, pathWidth);
        path.subdivisionCount = pathSubdivisions;
        path.GeneratePoints(pathAlea.x, pathAlea.y);
        List<Vector2Int> riverPathCrossingPositions = path.AllPoints.Where(p => _rivers.Any(r => r.AllPoints.Contains(p))).ToList();
        List<Vector2Int> pathPositions = path.AllPoints.Where(p => !riverPathCrossingPositions.Contains(p)).ToList();
        PutTiles(pathPositions, TileType.Dirt);
        PutTiles(riverPathCrossingPositions, TileType.Bridge);
        _pathes.Add(path);
        _bridgePositions.AddRange(riverPathCrossingPositions);
        _pathPositions.AddRange(pathPositions);
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
            new Vector2Int(halfSizeX-pathWidth / 2, -2),
            new Vector2Int(-1, -2),
            new Vector2Int(-1, mapSize.y),
            new Vector2Int(mapSize.x, mapSize.y),
            new Vector2Int(mapSize.x, -2),
            new Vector2Int(halfSizeX + pathWidth, -2),
            new Vector2Int(halfSizeX + pathWidth + width, width),
            new Vector2Int(mapSize.x - width, width),
            new Vector2Int(mapSize.x - width, mapSize.y - width),
            new Vector2Int(width, mapSize.y - width),
            new Vector2Int(width, width),
            new Vector2Int(halfSizeX - pathWidth/2 - width, width),
            new Vector2Int(halfSizeX - pathWidth/2, -2)
        };
        _borderPolygon = new Polygon(polygonBoundaries);
        _borderPolygon.GenerateFillingPoints();
        PutTiles(_borderPolygon.Points, TileType.MapBorder);
    }

    private void GenerateEntrancePOI()
    {
        PointOfInterest newPOI = Instantiate(_entrancePOI, transform);
        newPOI.SetPositionOnTilemap(new Vector2(mapSize.x * 0.5f - newPOI.Size.x * 0.5f - 0.5F, -1.0f)); // TODO: remove arbitrary 0.5F
        _spawnedPOIs.Add(newPOI);
        newPOI.MergeTilemaps(_terrainTilemap);
        _poiPositions.AddRange(newPOI.TilesPositions);
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

        if(_forestGenerators != null && _forestGenerators.Any(f => f.HasTreeAtPos(position)))
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
        /*DebugDrawRiver();
        DebugDrawPath();
        DebugDrawUsedPositions();*/
        DebugDrawConditionPositions();
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

    private void DebugDrawUsedPositions()
    {
        if(_usedPositions != null)
        {
            //_usedPositions.ForEach(p => DebugDrawPoint(p, Color.red));
            _usedPositions.ForEach(p => DebugDrawTile(p, Color.green));
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

    private void DebugDrawPoint(Vector2Int position, Color color)
    {
        Vector3 upLeft = new Vector3(position.x - 0.4f, position.y + 0.4f, 0.0f);
        Vector3 upRight = new Vector3(position.x + 0.4f, position.y + 0.4f, 0.0f);
        Vector3 downLeft = new Vector3(position.x - 0.4f, position.y - 0.4f, 0.0f);
        Vector3 downRight = new Vector3(position.x + 0.4f, position.y - 0.4f, 0.0f);
        Debug.DrawLine(upLeft, downRight, color);
        Debug.DrawLine(upRight, downLeft, color);
    }

    private void DebugDrawTile(Vector2Int position, Color color)
    {
        Vector3 upLeft = new Vector3(position.x + 0.05f, position.y + 0.95f, 0.0f);
        Vector3 upRight = new Vector3(position.x + 0.95f, position.y + 0.95f, 0.0f);
        Vector3 downLeft = new Vector3(position.x + 0.05f, position.y + 0.05f, 0.0f);
        Vector3 downRight = new Vector3(position.x + 0.95f, position.y + 0.05f, 0.0f);
        Debug.DrawLine(upLeft, upRight, color);
        Debug.DrawLine(upRight, downRight, color);
        Debug.DrawLine(downRight, downLeft, color);
        Debug.DrawLine(downLeft, upLeft, color);
    }

    private void TestCondition()
    {

        List<Vector2Int> riverPositions = new List<Vector2Int>();
        List<Vector2Int> pathPositions = new List<Vector2Int>();
        _rivers?.ForEach(r => riverPositions.AddRange(r.AllPoints));
        _pathes.ForEach(p => pathPositions.AddRange(p.AllPoints));
        List<MapCondition> conditions = new List<MapCondition>()
        {
            /*new IsCloseToTileCondition(riverPositions, _minPosition, _maxPosition, 5),
            new IsCloseToTileCondition(pathPositions, _minPosition, _maxPosition, 5),
            new IsNotOnTileCondition(riverPositions, _minPosition, _maxPosition),
            new IsNotOnTileCondition(pathPositions, _minPosition, _maxPosition),
            new IsNotOnTileCondition(_borderPolygon.Points, _minPosition, _maxPosition)*/
            new IsFarFromTileCondition(_poiPositions, _minPosition, _maxPosition, 5),
            new IsCloseToTileCondition(_borderPolygon.Points, _minPosition, _maxPosition, 0)
        };
        _testSatisfiesConditionList.AddRange(MapCondition.GetPositionsSatisfyingAll(conditions));
    }

    private void DebugDrawConditionPositions()
    {
        foreach(Vector2Int pos in _testSatisfiesConditionList)
        {
            DebugDrawTile(pos, Color.blue);
        }
    }
    #endregion
}
