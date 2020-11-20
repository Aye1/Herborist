using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public struct Edge
{
    public Vector2Int p1;
    public Vector2Int p2;
    public override string ToString()
    {
        return "[" + p1 + ", " + p2 + "]";
    }
}

public struct EdgeInfo
{
    public Edge edge;
    public int yMin;
    public int yMax;
    public int xOfYmin;
    public float invSlope;
    public override string ToString()
    {
        return "[" + yMin + ", " + yMax + ", " + xOfYmin + ", " + invSlope + "]";
    }
}

public class ActiveTableElement
{
    public int yMax;
    public float xOfYmin;
    public float invSlope;
    public override string ToString()
    {
        return "[" + yMax + ", " + xOfYmin + ", " + invSlope + "]";
    }

    public void UpdateX()
    {
        xOfYmin += invSlope;
    }
}

public struct ScanInteriorLine
{
    public int xMin;
    public int xMax;
    public int y;
    public override string ToString()
    {
        return "[(" + xMin + ", " + y + ") => (" + xMax + ", " + y + ")]";  
    }
}

public static class Polygon
{

    public static List<Vector2Int> GenerateFillingPoints(List<Vector2Int> vertices)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        List<Edge> edges = GenerateEdgesList(vertices);
        List<EdgeInfo> edgeTable = GenerateEdgeTable(edges);
        List<EdgeInfo> globalTable = GenerateGlobalTable(edgeTable);
        List<ActiveTableElement> activeTable = GenerateActiveTable(globalTable);
        List<ScanInteriorLine> drawingLines = Scan(activeTable, globalTable, edgeTable.First().yMin);
        List<Vector2Int> allCells = GenerateAllCells(drawingLines);
        DumpList(allCells);
        res.AddRange(allCells);
        return res;
    }

    private static List<Edge> GenerateEdgesList(List<Vector2Int> vertices)
    {
        List<Edge> res = new List<Edge>();
        for(int i=0; i<vertices.Count-1; i++)
        {
            Vector2Int first = vertices.ToArray()[i];
            Vector2Int second = vertices.ToArray()[i + 1];
            Edge e = new Edge() { p1 = first, p2 = second };
            res.Add(e);
        }
        Vector2Int lastVertex = vertices.ToArray()[vertices.Count - 1];
        Vector2Int firstVertex = vertices.ToArray()[0];
        Edge lastEdge = new Edge() { p1 = lastVertex, p2 = firstVertex };
        res.Add(lastEdge);
        return res;
    }

    private static List<EdgeInfo> GenerateEdgeTable(List<Edge> edges)
    {
        List<EdgeInfo> infos = new List<EdgeInfo>();
        foreach(Edge e in edges)
        {
            EdgeInfo info = new EdgeInfo();
            info.edge = e;
            if(e.p1.y < e.p2.y)
            {
                info.yMin = e.p1.y;
                info.xOfYmin = e.p1.x;
                info.yMax = e.p2.y;
            } else
            {
                info.yMin = e.p2.y;
                info.xOfYmin = e.p2.x;
                info.yMax = e.p1.y;
            }
            int dx = e.p2.x - e.p1.x;
            int dy = e.p2.y - e.p1.y;
            if (dx == 0)
            {
                info.invSlope = 0;
            }
            else if (dy == 0)
            {
                info.invSlope = float.PositiveInfinity;
            } else
            {
                info.invSlope = (float)dx / (float)dy;
            }
            infos.Add(info);
        }
        return infos;
    }

    private static List<EdgeInfo> GenerateGlobalTable(List<EdgeInfo> edgeTable)
    {
        return edgeTable.Where(e => e.invSlope != float.PositiveInfinity).OrderBy(e => e.yMin).ThenBy(e => e.yMax).ThenBy(e => e.xOfYmin).ToList();
    }

    private static List<ActiveTableElement> GenerateActiveTable(List<EdgeInfo> globalTable)
    {
        int globalYMin = globalTable.First().yMin;
        List<EdgeInfo> selectedInfos = globalTable.Where(e => e.yMin == globalYMin).ToList();
        globalTable.RemoveAll(e => e.yMin == globalYMin);
        return selectedInfos.Select(e => CreateActiveElement(e)).ToList();
    }

    private static List<ActiveTableElement> GenerateNewActiveTableElements(List<EdgeInfo> globalTable, int yMin)
    {
        List<EdgeInfo> selectedInfos = globalTable.Where(e => e.yMin == yMin).ToList();
        globalTable.RemoveAll(e => e.yMin == yMin);
        return selectedInfos.Select(e => CreateActiveElement(e)).ToList();
    }

    private static ActiveTableElement CreateActiveElement(EdgeInfo e)
    {
        return new ActiveTableElement()
        {
            yMax = e.yMax,
            xOfYmin = e.xOfYmin,
            invSlope = e.invSlope
        };
    }

    private static List<ActiveTableElement> UpdateActiveTable(List<ActiveTableElement> activeTable, List<EdgeInfo> globalTable, int currentScanLine)
    {
        activeTable = activeTable.Where(e => e.yMax > currentScanLine + 1).ToList();

        foreach(ActiveTableElement elem in activeTable)
        {
            elem.UpdateX();
        }
        activeTable.AddRange(GenerateNewActiveTableElements(globalTable, currentScanLine + 1));
        return activeTable.OrderBy(elem => elem.xOfYmin).ToList();
    }

    private static List<ScanInteriorLine> Scan(List<ActiveTableElement> activeTable, List<EdgeInfo> globalTable, int yMin)
    {
        List<ScanInteriorLine> drawingLines = new List<ScanInteriorLine>();
        int ymax = activeTable.Last().yMax;

        for (int y = yMin; y < ymax; y++)
        {
            drawingLines.AddRange(ScanLine(y, activeTable));
            activeTable = UpdateActiveTable(activeTable.ToList(), globalTable, y);
        }
        DumpList(drawingLines);
        return drawingLines;
    }

    private static List<ScanInteriorLine> ScanLine(int y, List<ActiveTableElement> activeTable)
    {
        Debug.Log("Scanning line " + y);
        List<ScanInteriorLine> drawingLines = new List<ScanInteriorLine>();
        ActiveTableElement[] tableArray = activeTable.ToArray();
        if (tableArray[0].yMax != y)
        {
            for (int i = 0; i < activeTable.Count() - 1; i += 2)
            {
                ScanInteriorLine line = new ScanInteriorLine()
                {
                    y = y,
                    xMin = Mathf.CeilToInt(tableArray[i].xOfYmin),
                    xMax = Mathf.CeilToInt(tableArray[i + 1].xOfYmin)
                };
                drawingLines.Add(line);
            }
        }
        return drawingLines;
    }

    private static List<Vector2Int> GenerateAllCells(List<ScanInteriorLine> drawingLines)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        foreach(ScanInteriorLine line in drawingLines)
        {
            int y = line.y;
            for(int i=line.xMin; i<= line.xMax; i++)
            {
                Vector2Int cell = new Vector2Int(i, y);
                if(!res.Contains(cell))
                {
                    res.Add(cell);
                }
            }
        }
        return res;
    }

    public static void DumpList(IEnumerable elements)
    {
        string dump = "";
        foreach(object e in elements)
        {
            dump += e.ToString() + " \n";
        }
        Debug.Log(dump);
    }
}
