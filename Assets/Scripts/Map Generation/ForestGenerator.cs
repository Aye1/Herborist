using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ForestGenerator : MonoBehaviour
{
    // TODO: manage different width/height (if necessary)
    public float size = 10.0f;
    [Range(0.0f, 1.0f)] public float density = 1.0f;
    [Range(0.0f, 1.0f)] public float noise = 0.5f;

    public float stepSize = 1.0f;
    public int numberBalances = 2;
    [Required, SerializeField]
    private ProbabilityMap treesTemplates;
    [HideInInspector]
    public List<Vector2Int> posToAvoid;

    private Dictionary<Vector2Int, GameObject> _createdTrees;
    private int _numberSteps;
    private Vector3 _offset;
    private bool[,] _treePositions;


    public static ForestGenerator Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if(treesTemplates == null)
                treesTemplates = new ProbabilityMap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _offset = new Vector3(-size * 0.5f, 0.0f, 0.0f);
        _numberSteps = (int)(size / stepSize);
        posToAvoid = new List<Vector2Int>();
    }

    public void LaunchTreesGeneration()
    {
        GenerateRandomTrees();
        for(int i = 0; i < numberBalances; i++)
        {
            BalanceTrees();
        }
        CreateTrees();
    }

    public bool HasTreeAtPos(Vector2Int position)
    {
        Vector2Int internalPosition = TilemapToInternalPosition(position);
        return IsInGrid(internalPosition) && _treePositions[internalPosition.x, internalPosition.y];
    }

    private void GenerateRandomTrees()
    {
        _createdTrees = new Dictionary<Vector2Int, GameObject>();
        _treePositions = new bool[_numberSteps, _numberSteps];
        if(density == 0.0f)
        {
            return;
        }
        for (int i = 0; i < _numberSteps; i++)
        {
            for (int j = 0; j < _numberSteps; j++)
            {
                _treePositions[i, j] = Alea.GetFloat(0.0f, 1.0f) <= density && !MustBeAvoided(InternalToTilemapPosition(new Vector2Int(i,j)));
            }
        }
    }

    private void BalanceTrees()
    {
        List<Vector2Int> treesToDestroy = new List<Vector2Int>();
        List<Vector2Int> treesToCreate = new List<Vector2Int>();
        for (int i = 0; i < _numberSteps; i++)
        {
            for (int j = 0; j < _numberSteps; j++)
            {
                bool isSurrounded = IsPositionSurroundedByTrees(new Vector2Int(i, j));
                Vector2Int position = new Vector2Int(i, j);
                Vector2Int realPosition = InternalToTilemapPosition(position);
                if(HasTreeAtPosition(position) && !isSurrounded)
                {
                    treesToDestroy.Add(position);
                } else if (!HasTreeAtPosition(position) && isSurrounded && !MustBeAvoided(realPosition))
                {
                    treesToCreate.Add(position);
                }
            }
        }

        // Change the values after checking the whole map, as any change will have impact on the algorithm
        foreach(Vector2Int pos in treesToDestroy)
        {
            _treePositions[pos.x, pos.y] = false;
        }
        foreach(Vector2Int pos in treesToCreate)
        {
            _treePositions[pos.x, pos.y] = true;
        }
    }

    private bool IsPositionSurroundedByTrees(Vector2Int position)
    {
        return GetNeighboursCount(position) >= 4;
    }

    private int GetNeighboursCount(Vector2Int position)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int x = position.x + i;
                int y = position.y + j;

                // Thanks && is lazy and does not compute right part if left part is false
                if (IsInGrid(x, y) && _treePositions[x, y])
                {
                    count++;
                }
            }
        }
        return count;
    }

    private bool IsInGrid(int x, int y)
    {
        return x >= 0 && x < _numberSteps && y >= 0 && y < _numberSteps ;
    }

    private bool IsInGrid(Vector2Int position)
    {
        return IsInGrid(position.x, position.y);
    }

    private bool HasTreeAtPosition(Vector2Int position)
    {
        return _treePositions[position.x, position.y];
    }

    private bool MustBeAvoided(Vector2Int position)
    {
        return posToAvoid.Contains(position);
    }

    private bool MustBeAvoided(int x, int y)
    {
        return MustBeAvoided(new Vector2Int(x, y));
    }

    private void RemoveTreePosition(Vector2Int position)
    {
        _treePositions[position.x, position.y] = false;
    }

    private void CreateTrees()
    {
        for(int i = 0; i < _numberSteps; i++)
        {
            for(int j = 0; j < _numberSteps; j++)
            {
                if(_treePositions[i, j])
                {
                    CreateTree(new Vector2Int(i, j));
                }
            }
        }
    }

    private void CreateTree(Vector2Int position)
    {
        float noiseX = Alea.GetFloat(0.0f, noise);
        float noiseY = Alea.GetFloat(0.0f, noise);
        Vector2 realPos = InternalToRealContinuousPosition(position);
        Vector3 pos = new Vector3(realPos.x + noiseX, realPos.y + noiseY, 0.0f) + _offset;
        GameObject tree = GenerateTreeObject(pos);
        _createdTrees.Add(new Vector2Int(position.x, position.y), tree);
    }

    private GameObject GenerateTreeObject(Vector3 position)
    {
        GameObject tree = Instantiate(treesTemplates.GetRandomObject(), Vector3.zero, Quaternion.identity, transform);
        tree.transform.localPosition = position;
        tree.transform.localScale = Vector3.one;
        return tree;
    }

    private Vector2 InternalToRealContinuousPosition(Vector2Int position)
    {
        return new Vector2(position.x * stepSize, position.y * stepSize);
    }

    private Vector2Int InternalToTilemapPosition(Vector2Int position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x * stepSize), Mathf.FloorToInt(position.y * stepSize));
    }

    private Vector2Int TilemapToInternalPosition(Vector2Int position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x / stepSize), Mathf.FloorToInt(position.y / stepSize));
    }
}
