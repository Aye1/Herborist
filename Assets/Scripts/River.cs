using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River
{
    public MapLimit beginLimit;
    public MapLimit endLimit;
    public int width = 3;
    public List<Vector2Int> mainPoints;
    public bool mainPointsGenerated = false;
    public bool allPointsGenerated = false;

    public List<Vector2Int> secondaryLine;
    private Direction _beginNormal;
    private Direction _endNormal;
    private List<Vector2Int> _polygonPoints;
    private List<Vector2Int> _allPoints;

    public List<Vector2Int> AllPoints
    {
        get { return _allPoints; }
    }

    public List<Vector2Int> PolygonPoints
    {
        get { return _polygonPoints; }
    }

    public void GenerateMainPoints()
    {
        //CleanPoints();
        GenerateLimitsNormals();
        Vector2Int begin = MapGeneratorHelper.GeneratePointOnLimit(beginLimit, 5, 15);
        Vector2Int end = MapGeneratorHelper.GeneratePointOnLimit(endLimit, 5, 15);

        mainPoints = new List<Vector2Int>();
        mainPoints.Add(begin);
        mainPoints.Add(end);
        MapGeneratorHelper.SubdivideLine(mainPoints, begin, end, 2);
        mainPointsGenerated = true;    }

    private void GenerateLimitsNormals()
    {
        _beginNormal = (beginLimit == MapLimit.Down || beginLimit == MapLimit.Up) ? Direction.LeftRight : Direction.DownUp;
        _endNormal = (endLimit == MapLimit.Down || endLimit == MapLimit.Up) ? Direction.LeftRight : Direction.DownUp;
    }

    public void MoveMainPointsRandomly(int min, int max)
    {
        MapGeneratorHelper.DumpPoints(mainPoints);

        Vector2Int[] mainArray = mainPoints.ToArray();

        // Move first point, only depends on side of the map
        Vector2Int begin = mainArray[0];
        MapGeneratorHelper.MovePointRandomly(ref begin, _beginNormal, min, max);
        mainPoints.RemoveAt(0);
        mainPoints.Insert(0, begin);


        // Move last point, only depends on side of the map
        Vector2Int end = mainArray[mainPoints.Count - 1];
        MapGeneratorHelper.MovePointRandomly(ref end, _endNormal, min, max);
        mainPoints.RemoveAt(mainPoints.Count - 1);
        mainPoints.Add(end);

        for(int i=1; i<mainArray.Length-1; i++)
        {
            MapGeneratorHelper.MoveFirstPointOfSegmentRandomly(ref mainArray[i], mainArray[i + 1], min, max);
            mainPoints.RemoveAt(i);
            mainPoints.Insert(i, mainArray[i]);
        }

        MapGeneratorHelper.DumpPoints(mainPoints);
    }

    public void GenerateSecondaryLine()
    {
        secondaryLine = new List<Vector2Int>();
        secondaryLine.AddRange(mainPoints);

        Vector2Int[] secondArray = secondaryLine.ToArray();

        MapGeneratorHelper.MovePoint(ref secondArray[0], _beginNormal, -width);
        secondaryLine.RemoveAt(0);
        secondaryLine.Insert(0, secondArray[0]);

        for(int i=1; i<secondArray.Length-1; i++)
        {
            //MapGeneratorHelper.MovePoint(ref secondArray[i], MapGeneratorHelper.GetSegmentDirection(secondArray[i], secondArray[i+1]), -2);
            MapGeneratorHelper.MovePoint(ref secondArray[i], Direction.DownUp, -width);
            secondaryLine.RemoveAt(i);
            secondaryLine.Insert(i, secondArray[i]);
        }


        MapGeneratorHelper.MovePoint(ref secondArray[secondArray.Length - 1], _endNormal, -width);
        secondaryLine.RemoveAt(secondArray.Length - 1);
        secondaryLine.Add(secondArray[secondArray.Length - 1]);
        Debug.Log("second line");
        MapGeneratorHelper.DumpPoints(secondaryLine);
    }

    public void GeneratePolygonPoints()
    {
        _polygonPoints = new List<Vector2Int>();
        _polygonPoints.AddRange(mainPoints);
        secondaryLine.Reverse();
        _polygonPoints.AddRange(secondaryLine);
        Polygon polygon = new Polygon(_polygonPoints);
        polygon.GenerateFillingPoints();
        MapGeneratorHelper.DumpPoints(_polygonPoints);
        _allPoints = polygon.Points;
    }
}
