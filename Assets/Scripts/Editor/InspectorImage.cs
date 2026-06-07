using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(Image), true)]
    public class InspectorImage : ImageEditor
    {
        private Image _image = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            _image = (Image)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add Image Extension Script"))
            {
                if (_image.GetComponent<ImageExtension>() == null)
                {
                    _image.gameObject.AddComponent<ImageExtension>();
                }
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}