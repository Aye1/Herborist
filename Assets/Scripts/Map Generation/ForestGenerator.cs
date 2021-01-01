using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ForestGenerator : MonoBehaviour
{
    public float size = 10.0f;
    [Range(0.0f, 1.0f)] public float density = 1.0f;
    [Range(0.0f, 1.0f)] public float noise = 0.5f;

    // TODO: someday check how this is supposed to work and how it affects position offset
    [HideInInspector]public float stepSize = 1.0f;
    public int numberBalances = 0;
    public List<GameObject> treesTemplates;
    [HideInInspector]
    public List<Vector2Int> posToAvoid;

    private Dictionary<Vector2Int, GameObject> _createdTrees;
    private List<Vector2Int> _generatedPositions;
    private int _numberSteps;
    private Vector3 _offset;

    public IEnumerable<GameObject> Trees
    {
        get { return _createdTrees.Values.ToList(); }
    }

    public static ForestGenerator Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _offset = new Vector3(0.0f, size, 0.0f);
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
        CreateTrees(_generatedPositions);
    }

    public bool HasTreeAtPos(Vector2Int position)
    {
        return _generatedPositions.Contains(position);
    }

    private void GenerateRandomTrees()
    {
        _createdTrees = new Dictionary<Vector2Int, GameObject>();
        _generatedPositions = new List<Vector2Int>();
        if(density == 0.0f)
        {
            return;
        }
        for (int i = 0; i < 2 * _numberSteps; i++)
        {
            for (int j = 0; j < 2 * _numberSteps; j++)
            {
                if(Alea.GetFloat(0.0f, 1.0f) <= density && !MustBeAvoided(i, j))
                {
                     _generatedPositions.Add(new Vector2Int(i, j));
                }
            }
        }
    }

    private void BalanceTrees()
    {
        List<Vector2Int> treesToDestroy = new List<Vector2Int>();
        List<Vector2Int> treesToCreate = new List<Vector2Int>();
        for (int i = 0; i < 2 * _numberSteps; i++)
        {
            for (int j = 0; j < 2 * _numberSteps; j++)
            {
                bool isSurrounded = IsPositionSurroundedByTrees(new Vector2Int(i, j));
                Vector2Int position = new Vector2Int(i, j);
                if(HasTreeAtPosition(position) && !isSurrounded)
                {
                    treesToDestroy.Add(position);
                } else if (!HasTreeAtPosition(position) && isSurrounded && !MustBeAvoided(position))
                {
                    treesToCreate.Add(position);
                }
            }
        }
        _generatedPositions.RemoveAll(t => treesToDestroy.Contains(t));
        _generatedPositions.AddRange(treesToCreate);
    }

    private bool IsPositionSurroundedByTrees(Vector2Int position)
    {
        return GetTreeNeighbours(position).Count() >= 4;
    }

    private IEnumerable<Vector2Int> GetTreeNeighbours(Vector2Int position)
    {
        return _generatedPositions.Where(p =>
            {
                return Mathf.Abs(position.x - p.x) <= 1 && Mathf.Abs(position.y - p.y) <= 1;
            });
    }

    private bool HasTreeAtPosition(Vector2Int position)
    {
        return _generatedPositions.Contains(position);
    }

    private bool MustBeAvoided(Vector2Int position)
    {
        return posToAvoid.Contains(position);
    }

    private bool MustBeAvoided(int x, int y)
    {
        return MustBeAvoided(new Vector2Int(x, y));
    }

    private void DestroyTrees(IEnumerable<Vector2Int> treeList)
    {
        foreach(Vector2Int pos in treeList)
        {
            DestroyTree(pos);
        }
    }

    private void DestroyTree(Vector2Int position)
    {
        _generatedPositions.Remove(position);
    }

    private void CreateTrees(IEnumerable<Vector2Int> treeList)
    {
        foreach(Vector2Int pos in treeList)
        {
            CreateTree(pos);
        }
    }

    private void CreateTree(Vector2Int position)
    {
        float noiseX = Alea.GetFloat(0.0f, noise);
        float noiseY = Alea.GetFloat(0.0f, noise);
        Vector3 pos = new Vector3((position.x + noiseX - _numberSteps) * stepSize, (position.y + noiseY - _numberSteps) * stepSize, 0.0f) + _offset;
        GameObject tree = GenerateTree(pos);
        _createdTrees.Add(new Vector2Int(position.x, position.y), tree);
    }

    private GameObject GenerateTree(Vector3 position)
    {
        int id = Alea.GetInt(0, treesTemplates.Count);
        GameObject tree = Instantiate(treesTemplates[id], Vector3.zero, Quaternion.identity, transform);
        tree.transform.localPosition = position;
        tree.transform.localScale = Vector3.one;
        return tree;
    }
}
