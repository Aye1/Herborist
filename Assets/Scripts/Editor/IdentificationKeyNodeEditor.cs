using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

[CustomEditor(typeof(IdentificationKeyNode))]
public class IdentificationKeyNodeEditor : OdinEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        IdentificationKeyNode obj = target  as IdentificationKeyNode;

        SirenixEditorGUI.BeginBox();
        DisplayNodeRecursively(obj, "", 1);
        SirenixEditorGUI.EndBox();
    }

    private void DisplayNodeRecursively(IdentificationKeyNode node, string prefix, int level)
    {
        if(node.IsLeaf())
        {
            GUI.color = Color.green;
        }
        if (node.identificationData != null)
        {
            EditorGUILayout.LabelField(prefix + level + ". " + node.identificationData.identificationTitle);
        }
        foreach(IdentificationKeyNode child in node.treeNodes)
        {
            if(child != null)
                DisplayNodeRecursively(child, prefix + "    ", level + 1);
        }

        if (node.IsLeaf())
        {
            GUI.color = Color.white;
        }
    }
}
