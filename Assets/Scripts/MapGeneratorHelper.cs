using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MapLimit { Left, Up, Right, Down };

public static class MapGeneratorHelper
{

    public static void SubdivideLine(List<Vector2Int> completeLine, Vector2Int begin, Vector2Int end, int subdivideCount)
    {
        Vector2Int middlePoint = GeneratePointInMiddle(begin, end, new Vector2Int(0, 1));
        int index = completeLine.IndexOf(begin) + 1;
        completeLine.Insert(index, middlePoint);
        if(subdivideCount >  0)
        {
            SubdivideLine(completeLine, begin, middlePoint, subdivideCount - 1);
            SubdivideLine(completeLine, middlePoint, end, subdivideCount - 1);
        }
    }

    public static Vector2Int GeneratePointInMiddle(Vector2Int begin, Vector2Int end, Vector2Int maxAlea)
    {
        Vector2Int alea = new Vector2Int(Alea.GetInt(0, maxAlea.x), Alea.GetInt(0, maxAlea.y));
        Vector2Int newPoint = (begin + end) / 2 + alea;
        return newPoint;
    }

    public static void FillLine(List<Vector2Int> line)
    {
        List<Vector2Int> copyLine = new List<Vector2Int>();
        for(int i=0; i<line.Count-1; i++)
        {
            Vector2Int firstPoint = line.ToArray()[i];
            Vector2Int secondPoint = line.ToArray()[i + 1];

            copyLine.Add(firstPoint);
            copyLine.AddRange(CreateFillingBetweenTwoPoints(firstPoint, secondPoint));
        }
        copyLine.Add(line.ToArray()[line.Count - 1]);
        line.Clear();
        line.AddRange(copyLine);
    }

    private static List<Vector2Int> CreateFillingBetweenTwoPoints(Vector2Int begin, Vector2Int end)
    {
        List<Vector2Int> newPoints = new List<Vector2Int>();
        Vector2Int diff = end - begin;

        if (diff.x >= diff.y)
        {
            // Iterate on x
            float slope = (end.y - begin.y) / (float)(end.x - begin.x);
            for (int j = 1; j < diff.x; j++)
            {
                Vector2Int newPos = new Vector2Int(begin.x + j, Mathf.RoundToInt(slope * j) + begin.y);
                newPoints.Add(newPos);
            }
        }
        else
        {
            // Iterate on y
            float slope = (end.x - begin.x) / (float)(end.y - begin.y);
            for (int j = 1; j < diff.y; j++)
            {
                Vector2Int newPos = new Vector2Int(begin.x + Mathf.RoundToInt(slope * j), begin.y + j);
                newPoints.Add(newPos);
            }
        }
        return newPoints;
    }

    public static Vector2Int GeneratePointOnLimit(MapLimit limit, int min, int max, Vector2Int mapSize)
    {
        int position = Alea.GetInt(min, max);
        Vector2Int res = new Vector2Int();
        switch(limit)
        {
            case MapLimit.Left:
                res.x = 0;
                res.y = position;
                break;
            case MapLimit.Right:
                res.x = mapSize.x - 1;
                res.y = position;
                break;
            case MapLimit.Up:
                res.x = position;
                res.y = mapSize.y - 1;
                break;
            case MapLimit.Down:
                res.x = position;
                res.y = 0;
                break;

        }
        return res;
    }

}
