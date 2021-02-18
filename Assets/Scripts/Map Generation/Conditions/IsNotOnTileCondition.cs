using System.Collections.Generic;
using UnityEngine;

public class IsNotOnTileCondition : MapCondition
{
    public IsNotOnTileCondition(IEnumerable<Vector2Int> tilesList, Vector2Int minPos, Vector2Int maxPos) : base(tilesList, minPos, maxPos) { }

    protected override void ComputeConditionList()
    {
        _conditionPositionsList = new List<Vector2Int>();
        for(int i=_minPosition.x; i<=_maxPosition.x; i++)
        {
            for(int j=_minPosition.y; j<=_maxPosition.y; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                if(!_referenceList.Contains(pos) && IsValid(pos))
                {
                    _conditionPositionsList.Add(pos);
                }
            }
        }
    }
}

