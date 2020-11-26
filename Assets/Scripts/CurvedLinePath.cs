using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedLinePath
{
    public MapLimit beginLimit;
    public MapLimit endLimit;
    public int width = 3;
    public bool polygonPointsGenerated = false;

    private List<Vector2Int> _mainPoints;
    private List<Vector2Int> _secondaryLine;
    private Direction _beginNormal;
    private Direction _endNormal;
    private List<Vector2Int> _polygonPoints;
    private List<Vector2Int> _allPoints;
    private int _beginMoveSign;
    private int _endMoveSign;

    public CurvedLinePath(MapLimit begin, MapLimit end, int width)
    {
        beginLimit = begin;
        endLimit = end;
        this.width = width;
    }

    public List<Vector2Int> AllPoints
    {
        get { return _allPoints; }
    }

    public List<Vector2Int> PolygonPoints
    {
        get { return _polygonPoints; }
    }

    public void GeneratePoints()
    {
        GenerateMainPoints();
        MoveMainPointsRandomly(1, 5);
        GenerateSecondaryLine();
        GeneratePolygonPoints();
    }

    private void GenerateMainPoints()
    {
        polygonPointsGenerated = false;
        GenerateLimitsNormals();
        GenerateMovingSigns();
        Vector2Int begin = MapGeneratorHelper.GeneratePointOnLimit(beginLimit, 0.25f, 0.75f);
        Vector2Int end = MapGeneratorHelper.GeneratePointOnLimit(endLimit, 0.25f, 0.75f);

        _mainPoints = new List<Vector2Int>();
        _mainPoints.Add(begin);
        _mainPoints.Add(end);
        MapGeneratorHelper.SubdivideLine(_mainPoints, begin, end, 3);
    }

    private void GenerateLimitsNormals()
    {
        _beginNormal = (beginLimit == MapLimit.Down || beginLimit == MapLimit.Up) ? Direction.LeftRight : Direction.DownUp;
        _endNormal = (endLimit == MapLimit.Down || endLimit == MapLimit.Up) ? Direction.LeftRight : Direction.DownUp;
    }

    private void GenerateMovingSigns()
    {
        switch(beginLimit)
        {
            case MapLimit.Left:
                _beginMoveSign = endLimit == MapLimit.Down ? 1 : -1;
                _endMoveSign = endLimit == MapLimit.Right ? -1 : 1;
                break;
            case MapLimit.Up:
                _beginMoveSign = endLimit == MapLimit.Right ? -1 : 1;
                _endMoveSign = endLimit == MapLimit.Down ? 1 : -1;
                break;
            case MapLimit.Right:
                _beginMoveSign = endLimit == MapLimit.Down ? 1 : -1;
                _endMoveSign = -1;
                break;
            case MapLimit.Down:
                _beginMoveSign = endLimit == MapLimit.Right ? -1 : 1;
                _endMoveSign = 1;
                break;
        }
    }

    public void MoveMainPointsRandomly(int min, int max)
    {
        MapGeneratorHelper.DumpPoints(_mainPoints);

        Vector2Int[] mainArray = _mainPoints.ToArray();

        for(int i=0; i<mainArray.Length; i++)
        {
            Direction dir = i < mainArray.Length / 2 ? _beginNormal : _endNormal;
            MapGeneratorHelper.MovePointRandomly(ref mainArray[i], dir, min, max);
            _mainPoints.RemoveAt(i);
            _mainPoints.Insert(i, mainArray[i]);
        }

        MapGeneratorHelper.DumpPoints(_mainPoints);
    }

    public void GenerateSecondaryLine()
    {
        _secondaryLine = new List<Vector2Int>();
        _secondaryLine.AddRange(_mainPoints);

        Vector2Int[] secondArray = _secondaryLine.ToArray();

        for(int i=0; i<secondArray.Length; i++)
        {
            Direction movingDirection = i < secondArray.Length / 2 ? _beginNormal : _endNormal;
            int sign = i < secondArray.Length / 2 ? _beginMoveSign : _endMoveSign;
            MapGeneratorHelper.MovePoint(ref secondArray[i], movingDirection, width * sign);
            _secondaryLine.RemoveAt(i);
            _secondaryLine.Insert(i, secondArray[i]);
        }
    }

    public void GeneratePolygonPoints()
    {
        _polygonPoints = new List<Vector2Int>();
        _polygonPoints.AddRange(_mainPoints);
        _secondaryLine.Reverse();
        _polygonPoints.AddRange(_secondaryLine);
        Polygon polygon = new Polygon(_polygonPoints);
        polygon.GenerateFillingPoints();
        MapGeneratorHelper.DumpPoints(_polygonPoints);
        _allPoints = polygon.Points;
        polygonPointsGenerated = true;
    }
}
