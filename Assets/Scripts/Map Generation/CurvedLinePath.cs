using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedLinePath
{
    public int width = 3;
    public bool polygonPointsGenerated = false;

    private List<Vector2Int> _polygonPoints;
    private List<Vector2Int> _allPoints;
    private List<Vector2Int> _controlPoints;
    private List<Vector2Int> _fullFirstLine;
    private List<Direction> _fullFirstLineNormals;
    private List<Vector2Int> _fullSecondLine;

    public CurvedLinePath(List<Vector2Int> controlPoints, int width)
    {
        _controlPoints = new List<Vector2Int>();
        _controlPoints.AddRange(controlPoints);
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

    public void GeneratePoints(int minAlea, int maxAlea)
    {
        GenerateFirstLine();
        MoveFirstLinePointsRandomly(minAlea, maxAlea);
        GenerateSecondLine();
        GeneratePolygonPoints();
    }

    private void GenerateFirstLine()
    {
        _fullFirstLine = new List<Vector2Int>();
        _fullFirstLine.AddRange(_controlPoints);
        MapGeneratorHelper.SubdivideLine(_fullFirstLine, 2);
        GenerateFirstLineNormals();
    }

    private void GenerateFirstLineNormals()
    {
        _fullFirstLineNormals = new List<Direction>();

        for(int i=0; i<_fullFirstLine.Count -1; i++)
        {
            _fullFirstLineNormals.Add(MapGeneratorHelper.GetSegmentNormalDirection(_fullFirstLine[i], _fullFirstLine[i + 1]));
        }
        // Last normal is the same as the previous one
        _fullFirstLineNormals.Add(_fullFirstLineNormals[_fullFirstLineNormals.Count-1]);
    }

    private void GenerateSecondLine()
    {
        _fullSecondLine = new List<Vector2Int>(_fullFirstLine);
        for(int i = 0; i< _fullSecondLine.Count; i++)
        {
            Vector2Int point = _fullSecondLine[i];
            MapGeneratorHelper.MovePoint(ref point, _fullFirstLineNormals[i], width);
            _fullSecondLine.RemoveAt(i);
            _fullSecondLine.Insert(i, point);
        }
    }

    private void GeneratePolygonPoints()
    {
        _polygonPoints = new List<Vector2Int>();
        _polygonPoints.AddRange(_fullFirstLine);
        _fullSecondLine.Reverse();
        _polygonPoints.AddRange(_fullSecondLine);
        Polygon polygon = new Polygon(_polygonPoints);
        polygon.GenerateFillingPoints();
        _allPoints = polygon.Points;
        polygonPointsGenerated = true;
    }

    private void MoveFirstLinePointsRandomly(int minAlea, int maxAlea)
    {
        Vector2Int[] mainArray = _fullFirstLine.ToArray();

        for (int i = 0; i < mainArray.Length; i++)
        {
            Vector2Int currentPoint = mainArray[i];
            // Ignore control points, to keep control on the polygon
            if (!_controlPoints.Contains(currentPoint))
            {
                MapGeneratorHelper.MovePointRandomly(ref mainArray[i], _fullFirstLineNormals[i], minAlea, maxAlea);
                _fullFirstLine.RemoveAt(i);
                _fullFirstLine.Insert(i, mainArray[i]);
            }
        }
    }

    // Old method to have correct width continuity
    // May or may not be useful in the future, so i just keep it

    /*private void GenerateMovingSigns()
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
    }*/
}
