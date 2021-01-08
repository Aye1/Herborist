using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Sirenix.OdinInspector;

public class MiniMap : MonoBehaviour
{
    private Texture2D _texture;
    private RawImage _image;
    private Vector2Int _size;
    private List<Vector2Int> _discoveredPixels;
    private Vector3 _cursorOffset;
    PlayerMovement _player;
    [SerializeField, Required, ChildGameObjectsOnly] Image _cursor;

    public int visionRadius;

    // Start is called before the first frame update
    void Start()
    {
        _player = PlayerMovement.Instance;
        _discoveredPixels = new List<Vector2Int>();
        _size = MapGenerator.Instance.mapSize;
        _image = GetComponent<RawImage>();
        _texture = new Texture2D(_size.x, _size.y);
        _cursorOffset = new Vector3(-_size.x * 0.5f, -_size.y * 0.5f + 1, 0.0f); // Not sure about the +1, but it seems more accurate

        // If the original picture has the right size, we use it as our base canvas
        if(_image.texture.width == _size.x && _image.texture.height == _size.y)
        {
            Graphics.CopyTexture(_image.texture, _texture);
        }
        _image.texture = _texture;
    }

    private void Update()
    {
        UpdateMap();
    }

    private void TestSetPixel()
    {
        for (int i = 0; i < _size.x; i++)
        {
            for (int j = 0; j < _size.y; j++)
            {
                SetPixel(i, j, TileType.Grass);
            }
        }
        _texture.Apply();
    }

    public void SetPixel(int x, int y, TileType type, bool update = false)
    {
        if(!IsPixelInBounds(x, y) || (!update && IsPixelDiscovered(x, y)))
        {
            return;
        }
        _texture.SetPixel(x, y, GetColor(type));
        _discoveredPixels.Add(new Vector2Int(x, y));
    }

    public void SetPixel(Vector2Int pos, TileType type)
    {
        SetPixel(pos.x, pos.y, type);
    }

    private bool IsPixelInBounds(int x, int y)
    {
        return x >= 0 && x < _size.x && y >= 0 && y < _size.y;
    }

    private bool IsPixelDiscovered(int x, int y)
    {
        return IsPixelDiscovered(new Vector2Int(x, y));
    }

    private bool IsPixelDiscovered(Vector2Int position)
    {
        return _discoveredPixels.Contains(position);
    }

    private Color GetColor(TileType type)
    {
        return MapGenerator.Instance.tileMappings.Where(m => m.type == type).First().minimapColor;
    }

    private void UpdateMap()
    {
        Vector2Int currentPos = MapGenerator.Instance.GetPosOnTilemap(_player.transform.position);
        Vector2 currentContPos = _player.transform.position;
        _cursor.transform.localPosition = new Vector3(currentContPos.x, currentContPos.y, 0.0f)  + _cursorOffset;

        foreach(Vector2Int pixel in GetPixelsInRadius(currentPos))
        {
            if (!IsPixelDiscovered(pixel)) {
                TileType tileType = MapGenerator.Instance.GetTypeAtPos(pixel);
                SetPixel(pixel, tileType);
            }
        }
        _texture.Apply();
    }

    private List<Vector2Int> GetPixelsInRadius(Vector2Int center)
    {
        List<Vector2Int> pixels = new List<Vector2Int>();

        int xMin = center.x - visionRadius;
        int xMax = center.x + visionRadius;
        int yMin = center.y - visionRadius;
        int yMax = center.y + visionRadius;

        for(int i=0; i<=xMax-xMin; i++)
        {
            for (int j = 0; j <= yMax - yMin; j++)
            {
                int x = xMin + i;
                int y = yMin + j;
                Vector2Int vec = new Vector2Int(x, y);
                int dist = Mathf.RoundToInt(Vector2Int.Distance(vec, center));
                if(dist <= visionRadius && IsPixelInBounds(x, y))
                {
                    pixels.Add(vec);
                }
            }
        }
        return pixels;
    }
}
