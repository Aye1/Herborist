using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IsOnTileCondition : MapCondition
{
    public IsOnTileCondition(IEnumerable<Vector2Int> tilesList, Vector2Int minPos, Vector2Int maxPos) : base(tilesList, minPos, maxPos) { }

    protected override void ComputeConditionList()
    {
        _conditionPositionsList = new List<Vector2Int>();
        IEnumerable<Vector2Int> positions = _conditionPositionsList.Where(p => IsValid(p));
        _conditionPositionsList.AddRange(positions);
    }
}
