using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoxCollider2D))]
[CanEditMultipleObjects]
public class InspectorBoxCollider2D : Editor
{
    private Editor _defaultEditor;

    private void OnEnable()
    {
        Type editorType = Type.GetType("UnityEditor.BoxCollider2DEditor, UnityEditor");

        if (editorType != null)
        {
            _defaultEditor = CreateEditor(targets, editorType);
        }
    }

    private void OnDisable()
    {
        if (_defaultEditor != null)
        {
            DestroyImmediate(_defaultEditor);
        }
    }

    public override void OnInspectorGUI()
    {
        if (_defaultEditor != null)
        {
            _defaultEditor.OnInspectorGUI();
        }
        else
        {
            DrawDefaultInspector();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Collision Event Trigger 2D"))
        {
            foreach (UnityEngine.Object targetObject in targets)
            {
                BoxCollider2D boxCollider = targetObject as BoxCollider2D;
                if (boxCollider == null) continue;

                if (boxCollider.GetComponent<CollisionEventTrigger2D>() == null)
                {
                    Undo.AddComponent<CollisionEventTrigger2D>(boxCollider.gameObject);
                }
            }
        }
    }

    private void OnSceneGUI()
    {
        if (_defaultEditor != null)
        {
            _defaultEditor.GetType()
                .GetMethod("OnSceneGUI",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Public)
                ?.Invoke(_defaultEditor, null);
        }
    }
}