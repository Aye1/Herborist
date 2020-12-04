using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 *  Automatic filling polygon
 *  More information about the algorithm: https://core.ac.uk/download/pdf/234644791.pdf
 */


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

    public ActiveTableElement() { }

    public ActiveTableElement(EdgeInfo e)
    {
        yMax = e.yMax;
        xOfYmin = e.xOfYmin;
        invSlope = e.invSlope;
    }

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

public class Polygon
{
    private List<Vector2Int> _vertices;
    private List<Vector2Int> _fillingPoints;
    private List<Edge> _edges;
    private List<EdgeInfo> _edgesInfos;
    private List<EdgeInfo> _globalTable;
    private List<ActiveTableElement> _activeTable;
    private List<ScanInteriorLine> _drawingLines;

    public List<Vector2Int> Points
    {
        get { return _fillingPoints; }
    }

    public Polygon(List<Vector2Int> vertices)
    {
        _vertices = new List<Vector2Int>();
        _vertices.AddRange(vertices);
        _fillingPoints = new List<Vector2Int>();
    }

    public void GenerateFillingPoints()
    {
        GenerateEdgesList();
        GenerateEdgeTable();
        GenerateGlobalTable();
        GenerateActiveTable();
        Scan();
        GenerateAllCells();
    }

    private void GenerateEdgesList()
    {
        _edges = new List<Edge>();
        for(int i=0; i<_vertices.Count-1; i++)
        {
            Vector2Int first = _vertices.ToArray()[i];
            Vector2Int second = _vertices.ToArray()[i + 1];
            Edge e = new Edge() { p1 = first, p2 = second };
            _edges.Add(e);
        }
        Vector2Int lastVertex = _vertices.ToArray()[_vertices.Count - 1];
        Vector2Int firstVertex = _vertices.ToArray()[0];
        Edge lastEdge = new Edge() { p1 = lastVertex, p2 = firstVertex };
        _edges.Add(lastEdge);
    }

    private void GenerateEdgeTable()
    {
        _edgesInfos = new List<EdgeInfo>();
        foreach(Edge e in _edges)
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
            _edgesInfos.Add(info);
        }
    }

    private void GenerateGlobalTable()
    {
        _globalTable = _edgesInfos.Where(e => e.invSlope != float.PositiveInfinity).OrderBy(e => e.yMin).ThenBy(e => e.yMax).ThenBy(e => e.xOfYmin).ToList();
    }


    // TODO: refacto GenerateActiveTable and GenerateNewActiveTableElements together
    private void GenerateActiveTable()
    {
        int globalYMin = _globalTable.First().yMin;
        List<EdgeInfo> selectedInfos = _globalTable.Where(e => e.yMin == globalYMin).ToList();
        _globalTable.RemoveAll(e => e.yMin == globalYMin);
        _activeTable = selectedInfos.Select(e => new ActiveTableElement(e)).ToList();
    }

    private void GenerateNewActiveTableElements(int yMin)
    {
        List<EdgeInfo> selectedInfos =_globalTable.Where(e => e.yMin == yMin).ToList();
        _globalTable.RemoveAll(e => e.yMin == yMin);
        _activeTable.AddRange(selectedInfos.Select(e => new ActiveTableElement(e)).ToList());
    }

    private void UpdateActiveTable(int currentScanLine)
    {
        _activeTable = _activeTable.Where(e => e.yMax > currentScanLine + 1).ToList();

        foreach(ActiveTableElement elem in _activeTable)
        {
            elem.UpdateX();
        }
        GenerateNewActiveTableElements(currentScanLine + 1);
        _activeTable =  _activeTable.OrderBy(elem => elem.xOfYmin).ToList();
    }

    private void Scan()
    {
        _drawingLines = new List<ScanInteriorLine>();
        int ymin = _edgesInfos.Min(e => e.yMin);
        int ymax = _edgesInfos.Max(e => e.yMax);

        for (int y = ymin; y < ymax; y++)
        {
            ScanLine(y);
            UpdateActiveTable(y);
        }
    }

    private void ScanLine(int y)
    {
        ActiveTableElement[] tableArray = _activeTable.ToArray();
        if (tableArray[0].yMax != y)
        {
            for (int i = 0; i < _activeTable.Count() - 1; i += 2)
            {
                ScanInteriorLine line = new ScanInteriorLine()
                {
                    y = y,
                    xMin = Mathf.CeilToInt(tableArray[i].xOfYmin),
                    xMax = Mathf.CeilToInt(tableArray[i + 1].xOfYmin)
                };
                _drawingLines.Add(line);
            }
        }
    }

    private void GenerateAllCells()
    {
        foreach(ScanInteriorLine line in _drawingLines)
        {
            int y = line.y;
            for(int i=line.xMin; i<= line.xMax; i++)
            {
                Vector2Int cell = new Vector2Int(i, y);
                if(!_fillingPoints.Contains(cell))
                {
                    _fillingPoints.Add(cell);
                }
            }
        }
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
