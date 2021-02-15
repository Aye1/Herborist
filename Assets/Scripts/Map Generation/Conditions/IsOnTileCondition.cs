using System.Collections.Generic;
using UnityEngine;

public class IsOnTileCondition : MapCondition
{
    public IsOnTileCondition(IEnumerable<Vector2Int> tilesList, Vector2Int minPos, Vector2Int maxPos) : base(tilesList, minPos, maxPos) { }

    protected override void ComputeConditionList()
    {
        _conditionPositionsList = new List<Vector2Int>();
        _conditionPositionsList.AddRange(_referenceList);
    }
}
