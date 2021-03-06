﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;
using Sirenix.OdinInspector;
using System.Linq;

public class PointOfInterest : MonoBehaviour
{
    [SerializeField, Required, ChildGameObjectsOnly] private List<STETilemap> _tilemaps;
    private Vector2Int _bottomLeftTilePosition;
    private List<Vector2Int> _tilesPositions;

    public Vector2Int Size { get; private set; }

    public List<Vector2Int> TilesPositions
    {
        get
        {
            return _tilesPositions;
        }
    }

    private void Awake()
    {
        if (_tilemaps != null)
        {
            int x = _tilemaps.Max(t => t.GridWidth);
            int y = _tilemaps.Max(t => t.GridHeight);
            Size = new Vector2Int(x, y);
        }
        _tilesPositions = new List<Vector2Int>();
    }

    public void SetPositionOnTilemap(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, 0.0f);
        _bottomLeftTilePosition = new Vector2Int(Mathf.FloorToInt(position.x) - 1, Mathf.FloorToInt(position.y)); // sorry for the -1
        UpdateTilesPositions();
    }

    private void UpdateTilesPositions()
    {
        _tilesPositions.Clear();
        for(int i = 0 ; i < Size.x; i++)
        {
            int x = _bottomLeftTilePosition.x + i;
            for (int j = 0; j < Size.y; j++)
            {
                int y = _bottomLeftTilePosition.y + j;
                _tilesPositions.Add(new Vector2Int(x, y));
            }
        }
    }

    public void MergeTilemaps(STETilemap destTilemap)
    {
         _tilemaps.ForEach(t => MergeTilemap(t, destTilemap));
    }

    private void MergeTilemap(STETilemap srcTilemap, STETilemap destTilemap)
    {
        // Warning: there is a -1 here, check with other POIs if it's ok
        for(int i=0; i < srcTilemap.GridWidth-1; i++)
        {
            for(int j=0; j<srcTilemap.GridHeight; j++)
            {
                Vector2Int realPosition = new Vector2Int(_bottomLeftTilePosition.x + i + 1, _bottomLeftTilePosition.y + j);
                uint tileDataId = srcTilemap.GetTileData(i, j);
                TileData tileData = new TileData(tileDataId);
                destTilemap.SetTileData(realPosition, tileData.BuildData());
            }
        }
        srcTilemap.gameObject.SetActive(false);
    }
}
