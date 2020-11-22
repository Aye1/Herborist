using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MapLimit { Left, Up, Right, Down };
public enum Direction { DownUp, LeftRight };

public static class MapGeneratorHelper
{

    public static void SubdivideLine(List<Vector2Int> completeLine, Vector2Int begin, Vector2Int end, int subdivideCount)
    {
        Vector2Int middlePoint = GeneratePointInMiddle(begin, end);
        int index = completeLine.IndexOf(begin) + 1;
        completeLine.Insert(index, middlePoint);
        if(subdivideCount >  0)
        {
            SubdivideLine(completeLine, begin, middlePoint, subdivideCount - 1);
            SubdivideLine(completeLine, middlePoint, end, subdivideCount - 1);
        }
    }

    public static Vector2Int GeneratePointInMiddle(Vector2Int begin, Vector2Int end)
    {
        Vector2Int newPoint = (begin + end) / 2;
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

        if (GetSegmentDirection(begin, end) == Direction.LeftRight)
        {
            // Iterate on x
            float slope = (end.y - begin.y) / (float)(end.x - begin.x);
            int sign = diff.x / Mathf.Abs(diff.x);
            for (int j = 1; j < Mathf.Abs(diff.x); j++)
            {
                Vector2Int newPos = new Vector2Int(begin.x + j * sign, Mathf.RoundToInt(slope * j * sign) + begin.y);
                newPoints.Add(newPos);
            }
        }
        else
        {
            // Iterate on y
            float slope = (end.x - begin.x) / (float)(end.y - begin.y);
            int sign = diff.y / Mathf.Abs(diff.y);
            for (int j = 1; j < Mathf.Abs(diff.y); j++)
            {
                Vector2Int newPos = new Vector2Int(begin.x + Mathf.RoundToInt(slope * j * sign), begin.y + j * sign);
                newPoints.Add(newPos);
            }
        }
        return newPoints;
    }

    public static void MovePointRandomly(ref Vector2Int point, Direction dir, int min, int max)
    {
        int rand = Alea.GetInt(min, max);
        MovePoint(ref point, dir, rand);
    }

    public static void MovePoint(ref Vector2Int point, Direction dir, int move)
    {
        if(dir == Direction.DownUp)
        {
            point.Set(point.x, Mathf.Clamp(point.y + move, 0, MapGenerator.Instance.mapSize.y - 1));
        } else
        {
            point.Set(Mathf.Clamp(point.x + move, 0, MapGenerator.Instance.mapSize.x - 1), point.y);
        }
    }

    public static Direction GetSegmentDirection(Vector2Int begin, Vector2Int end)
    {
        Vector2Int diff = end - begin;
        if(Mathf.Abs(diff.x) >= Mathf.Abs(diff.y))
        {
            return Direction.LeftRight;
        } else
        {
            return Direction.DownUp;
        }
    }

    public static Direction GetSegmentNormalDirection(Vector2Int begin, Vector2Int end)
    {
        Vector2Int diff = end - begin;
        if (Mathf.Abs(diff.x) >= Mathf.Abs(diff.y))
        {
            return Direction.DownUp;
        }
        else
        {
            return Direction.LeftRight;
        }
    }

    public static Vector2Int GeneratePointOnLimit(MapLimit limit, float minPercent, float maxPercent)
    {
        Vector2Int res = new Vector2Int();
        Vector2Int mapSize = MapGenerator.Instance.mapSize;
        switch(limit)
        {
            case MapLimit.Left:
                res.x = 0;
                res.y = Alea.GetInt(Mathf.RoundToInt(minPercent * mapSize.y), Mathf.RoundToInt(maxPercent * mapSize.y));
                break;
            case MapLimit.Right:
                res.x = mapSize.x - 1;
                res.y = Alea.GetInt(Mathf.RoundToInt(minPercent * mapSize.y), Mathf.RoundToInt(maxPercent * mapSize.y));
                break;
            case MapLimit.Up:
                res.x = Alea.GetInt(Mathf.RoundToInt(minPercent * mapSize.x), Mathf.RoundToInt(maxPercent * mapSize.x));
                // Rasterization algo doesn't fill first line, so we move the polygon one cell up
                res.y = mapSize.y;
                break;
            case MapLimit.Down:
                res.x = Alea.GetInt(Mathf.RoundToInt(minPercent * mapSize.x), Mathf.RoundToInt(maxPercent * mapSize.x));
                res.y = 0;
                break;

        }
        return res;
    }

    public static void DumpPoints(List<Vector2Int> points)
    {
        string dump = "Points: ";
        foreach(Vector2Int point in points)
        {
            dump += point + " ";
        }
        Debug.Log(dump);
    }

}
