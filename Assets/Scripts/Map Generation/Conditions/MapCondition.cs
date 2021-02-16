using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class MapCondition
{
    protected List<Vector2Int> _referenceList;
    protected List<Vector2Int> _conditionPositionsList;
    protected List<Vector2Int> _authorizedPositions;
    protected List<Vector2Int> _excludedPositions;
    protected Vector2Int _minPosition;
    protected Vector2Int _maxPosition;


    public MapCondition(IEnumerable<Vector2Int> tilesList, Vector2Int minPosition, Vector2Int maxPosition)
    {
        _referenceList = new List<Vector2Int>();
        _referenceList.AddRange(tilesList);
        _minPosition = minPosition;
        _maxPosition = maxPosition;
    }

    protected abstract void ComputeConditionList();

    public bool SatisfiesCondition(Vector2Int testPosition)
    {
        if(_conditionPositionsList == null)
        {
            ComputeConditionList();
        }
        return _conditionPositionsList.Contains(testPosition);
    }

    public List<Vector2Int> GetAllSatisfyingPositions()
    {
        if(_conditionPositionsList == null)
        {
            ComputeConditionList();
        }
        return _conditionPositionsList;
    }

    public void AddAuthorizedPositions(IEnumerable<Vector2Int> positions)
    {
        if(_authorizedPositions == null)
        {
            _authorizedPositions = new List<Vector2Int>();
        }
        foreach(Vector2Int pos in positions)
        {
            if(!_authorizedPositions.Contains(pos))
            {
                _authorizedPositions.Add(pos);
            }
        }
    }

    public void AddExcludedPositions(IEnumerable<Vector2Int> positions)
    {
        if(_excludedPositions == null)
        {
            _excludedPositions = new List<Vector2Int>();
        }
        foreach (Vector2Int pos in positions)
        {
            if (!_excludedPositions.Contains(pos))
            {
                _excludedPositions.Add(pos);
            }
        }
    }

    protected bool IsInLimits(Vector2Int position)
    {
        return position.x >= _minPosition.x
            && position.x <= _maxPosition.x
            && position.y >= _minPosition.y
            && position.y <= _maxPosition.y;
    }

    protected bool IsValid(Vector2Int position)
    {
        bool res = IsInLimits(position);
        if(_excludedPositions != null && _excludedPositions.Count > 0)
        {
            res = res && !_excludedPositions.Contains(position);
        }
        if(_authorizedPositions != null && _authorizedPositions.Count > 0)
        {
            res = res && _authorizedPositions.Contains(position);
        }
        return res;
    }

    public static List<Vector2Int> GetPositionsSatisfyingAll(IEnumerable<MapCondition> conditions)
    {
        if(conditions == null || conditions.Count() == 0)
        {
            // return empty list, just to avoid null pointer issues
            return new List<Vector2Int>();
        }

        IEnumerable<Vector2Int> res = conditions.First().GetAllSatisfyingPositions();
        foreach(MapCondition cond in conditions)
        {
            // TODO: avoid passing twice on the first condition ?
            res = res.Intersect(cond.GetAllSatisfyingPositions());
        }
        return res.ToList();
    }

    public static Vector2Int GetRandomPointSatisfyingAll(IEnumerable<MapCondition> conditions)
    {
        List<Vector2Int> possiblePoints = GetPositionsSatisfyingAll(conditions);
        if(possiblePoints.Count() == 0)
        {
            Debug.LogWarning("No point satisfying conditions, returning (0,0)");
            return Vector2Int.zero;
        }
        int index = Alea.GetInt(0, possiblePoints.Count);
        return possiblePoints[index];
    }

    public static int ManhattanDistance(Vector2Int v1, Vector2Int v2)
    {
        return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y);
    }
}
