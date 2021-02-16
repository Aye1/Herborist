using System.Collections.Generic;
using UnityEngine;

public class AlgorithmPositionInformation
{
    public bool isFlagged;
    public int distance;
}

public class IsCloseToTileCondition : MapCondition
{
    protected uint _distance;

    private Dictionary<Vector2Int, AlgorithmPositionInformation> _tilesAlgoInfo;

    public IsCloseToTileCondition(IEnumerable<Vector2Int> tilesList, Vector2Int minPos, Vector2Int maxPos, uint distance) : base(tilesList, minPos, maxPos)
    {
        _distance = distance;
    }

    protected override void ComputeConditionList()
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        _conditionPositionsList = new List<Vector2Int>();

        _tilesAlgoInfo = new Dictionary<Vector2Int, AlgorithmPositionInformation>();

        foreach(Vector2Int pos in _referenceList)
        {
            TestPosition(pos, 0);
        }
        watch.Stop();
        Debug.LogFormat("Time for algo: {0}ms", watch.ElapsedMilliseconds);
    }

    private void TestPosition(Vector2Int pos, int distance)
    {
        if (!IsInLimits(pos))
        {
            return;
        }

        // First time this tile is tested, we create the information associated
        if(!_tilesAlgoInfo.ContainsKey(pos))
        {
            _tilesAlgoInfo.Add(pos, new AlgorithmPositionInformation() { distance = distance });
        }

        // We continue the algorithm only if the tile satisfies the distance condition
        // and if the current distance is better than any previous distance
        if(distance <= _distance)
        {
            if (!_tilesAlgoInfo[pos].isFlagged || distance < _tilesAlgoInfo[pos].distance)
            {
                if (!_conditionPositionsList.Contains(pos) && IsValid(pos))
                {
                     _conditionPositionsList.Add(pos);
                }
                _tilesAlgoInfo[pos].isFlagged = true;
                _tilesAlgoInfo[pos].distance = distance;
                TestPosition(pos + Vector2Int.left, distance + 1);
                TestPosition(pos + Vector2Int.up, distance + 1);
                TestPosition(pos + Vector2Int.right, distance + 1);
                TestPosition(pos + Vector2Int.down, distance + 1);
            }
        }
    } 
}
