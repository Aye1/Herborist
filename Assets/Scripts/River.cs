using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River
{
    public MapLimit beginLimit;
    public MapLimit endLimit;
    public List<Vector2Int> mainPoints;
    public List<Vector2Int> allPoints;
    public bool mainPointsGenerated = false;
    public bool allPointsGenerated = false;

    public void GenerateMainPoints()
    {
        CleanPoints();
        Vector2Int begin = MapGeneratorHelper.GeneratePointOnLimit(beginLimit, 5, 15);
        Vector2Int end = MapGeneratorHelper.GeneratePointOnLimit(endLimit, 5, 15);

        mainPoints = new List<Vector2Int>();
        mainPoints.Add(begin);
        mainPoints.Add(end);
        MapGeneratorHelper.SubdivideLine(mainPoints, begin, end, 2);
        mainPointsGenerated = true;
    }

    public void MoveMainPointsRandomly(int min, int max)
    {
        MapGeneratorHelper.DumpPoints(mainPoints);

        // Move first point, only depends on side of the map
        Direction beginDir = Direction.DownUp;
        if(beginLimit == MapLimit.Down || beginLimit == MapLimit.Up)
        {
            beginDir = Direction.LeftRight;
        }
        Vector2Int begin = mainPoints.ToArray()[0];
        MapGeneratorHelper.MovePointRandomly(ref begin, beginDir, min, max);
        mainPoints.RemoveAt(0);
        mainPoints.Insert(0, begin);


        // Move last point, only depends on side of the map
        Direction endDir = Direction.DownUp;
        if (endLimit == MapLimit.Down || endLimit == MapLimit.Up)
        {
            endDir = Direction.LeftRight;
        }
        Vector2Int end = mainPoints.ToArray()[mainPoints.Count - 1];
        MapGeneratorHelper.MovePointRandomly(ref end, endDir, min, max);
        mainPoints.RemoveAt(mainPoints.Count - 1);
        mainPoints.Add(end);

        MapGeneratorHelper.DumpPoints(mainPoints);
    }

    public void GenerateInterpolationPoints()
    {
        if (mainPointsGenerated)
        {
            allPoints = new List<Vector2Int>();
            allPoints.AddRange(mainPoints);
            MapGeneratorHelper.FillLine(allPoints);
            allPointsGenerated = true;
        }
    }

    private void CleanPoints()
    {
        if(mainPoints != null)
            mainPoints.Clear();
        if(allPoints != null)
            allPoints.Clear();
        mainPointsGenerated = false;
        allPointsGenerated = false;
    }

}
