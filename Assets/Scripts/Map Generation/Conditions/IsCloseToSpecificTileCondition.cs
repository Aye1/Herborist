using System.Collections.Generic;
using UnityEngine;

public class IsCloseToSpecificTileCondition : MapCondition
{
    private uint _distance;

    public IsCloseToSpecificTileCondition(Vector2Int tile, Vector2Int minPosition, Vector2Int maxPosition, uint distance) : base(new List<Vector2Int>() { tile }, minPosition, maxPosition)
    {
        _distance = distance;
    }

    protected override void ComputeConditionList()
    {
        _conditionPositionsList = new List<Vector2Int>();
        Vector2Int refPos = _referenceList[0];
        for (int i = _minPosition.x; i <= _maxPosition.x; i++)
        {
            for (int j = _minPosition.y; j <= _maxPosition.y; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                if (ManhattanDistance(pos, refPos) <= _distance && IsValid(pos))
                {
                    _conditionPositionsList.Add(pos);
                }
            }
        }
    }
}
