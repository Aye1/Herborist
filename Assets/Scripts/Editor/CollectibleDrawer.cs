using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;
using Sirenix.OdinInspector.Editor;

public class CollectibleDrawer : OdinValueDrawer<CollectibleScriptableObject>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        Rect rect = EditorGUILayout.GetControlRect();
        CollectibleScriptableObject obj = ValueEntry.SmartValue;
        Rect leftRect = new Rect()
        {
            xMin = rect.xMin,
            xMax = rect.xMax * 0.8f,
            yMin = rect.yMin,
            yMax = rect.yMax
        };
        EditorGUI.LabelField(leftRect, obj.developmentName);
        Rect rightRect = new Rect()
        {
            xMin = rect.xMax * 0.8f,
            xMax = rect.xMax,
            yMin = rect.yMin,
            yMax = rect.yMax
        };
        DrawTexturePreview(rightRect, obj.sprite);
    }


    // Original code from https://forum.unity.com/threads/drawing-a-sprite-in-editor-window.419199/
    private void DrawTexturePreview(Rect position, Sprite sprite)
    {
        Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
        Vector2 size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

        Rect coords = sprite.textureRect;
        coords.x /= fullSize.x;
        coords.width /= fullSize.x;
        coords.y /= fullSize.y;
        coords.height /= fullSize.y;

        Vector2 ratio;
        ratio.x = position.width / size.x;
        ratio.y = position.height / size.y;
        float minRatio = Mathf.Min(ratio.x, ratio.y);

        Vector2 center = position.center;
        position.width = size.x * minRatio;
        position.height = size.y * minRatio;
        position.center = center;

        GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
    }
}
