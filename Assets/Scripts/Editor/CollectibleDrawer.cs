using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class CollectibleDrawer : OdinValueDrawer<CollectibleScriptableObject>
{
    GUIStyle iconStyle = new GUIStyle(EditorStyles.miniButton);

    protected override void DrawPropertyLayout(GUIContent label)
    {
        iconStyle.alignment = TextAnchor.MiddleCenter;
        iconStyle.fixedWidth = 20;

        CollectibleScriptableObject obj = ValueEntry.SmartValue;
        bool fullDisplay = ValueEntry.IsEditable;
        if (fullDisplay)
        {
            string boxTitle = label == null ? "Collectible" : label.text;
            SirenixEditorGUI.BeginBox(boxTitle);
            CallNextDrawer(new GUIContent());
        }
        if(obj != null)
        {
            //Rect rect = EditorGUILayout.GetControlRect();
            /*Rect leftRect = new Rect()
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
            };*/
            /*rect = EditorGUI.PrefixLabel(rect, new GUIContent(obj.developmentName));
            DrawTexturePreview(rect, obj.sprite);*/
            EditorGUILayout.BeginHorizontal();
            Texture2D texture = AssetPreview.GetAssetPreview(obj.sprite);
            GUILayout.Label(texture, iconStyle);
            EditorGUILayout.LabelField(obj.developmentName);
            //GUILayout.Label("test",  defaultStyle);
            EditorGUILayout.EndHorizontal();
        }
        if (fullDisplay)
        {
            SirenixEditorGUI.EndBox();
        }
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
