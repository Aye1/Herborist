using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectMask2D))]
public class RectMaskSizeControl : MonoBehaviour
{
    private float _leftPadding;
    private float _rightPadding;
    private float _topPadding;
    private float _bottomPadding;

    public float LeftPadding
    {
        get { return _leftPadding; }
        set
        {
            _leftPadding = value;
            UpdateMaskSize();
        }
    }

    public float RightPadding
    {
        get { return _rightPadding; }
        set
        {
            _rightPadding = value;
            UpdateMaskSize();
        }
    }

    public float TopPadding
    {
        get { return _topPadding; }
        set
        {
            _topPadding = value;
            UpdateMaskSize();
        }
    }

    public float BottomPadding
    {
        get { return _bottomPadding; }
        set
        {
            _bottomPadding = value;
            UpdateMaskSize();
        }
    }

    private RectMask2D _mask;

    private void Awake()
    {
        _mask = GetComponent<RectMask2D>();
    }

    private void UpdateMaskSize()
    {
        float left = LeftPadding * _mask.rectTransform.rect.width;
        float right = RightPadding * _mask.rectTransform.rect.width;
        float top = TopPadding * _mask.rectTransform.rect.height;
        float bottom = BottomPadding * _mask.rectTransform.rect.height;
        _mask.padding = new Vector4(left, bottom, right, top);
    }
}
