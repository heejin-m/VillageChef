using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

[CustomEditor(typeof(LoopScrollRectCustom), true)]
public class LoopScrollRectCustomInspector : LoopScrollRectInspector
{
    SerializedProperty listItem;

    protected override void OnEnable()
    {
        base.OnEnable();
        listItem = serializedObject.FindProperty("listItem");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(listItem);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}