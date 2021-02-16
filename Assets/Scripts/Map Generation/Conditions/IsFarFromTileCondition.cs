using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsFarFromTileCondition : MapCondition
{
    private uint _distance;

    public IsFarFromTileCondition(IEnumerable<Vector2Int> tilesList, Vector2Int minPos, Vector2Int maxPos, uint distance) : base(tilesList, minPos, maxPos)
    {
        _distance = distance;
    }

    public IsFarFromTileCondition(Vector2Int tile, Vector2Int minPos, Vector2Int maxPos, uint distance) : this(new List<Vector2Int>() { tile }, minPos, maxPos, distance) { }

    protected override void ComputeConditionList()
    {
        _conditionPositionsList = new List<Vector2Int>();
        IsCloseToTileCondition reverseCondition = new IsCloseToTileCondition(_referenceList, _minPosition, _maxPosition, _distance);
        List<Vector2Int> forbiddenPos = reverseCondition.GetAllSatisfyingPositions();
        for(int i=_minPosition.x; i<=_maxPosition.x; i++)
        {
            for(int j=_minPosition.y; j<= _maxPosition.y; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                if(!forbiddenPos.Contains(pos) && IsValid(pos))
                {
                    _conditionPositionsList.Add(pos);
                }
            }
        }
    }
}
