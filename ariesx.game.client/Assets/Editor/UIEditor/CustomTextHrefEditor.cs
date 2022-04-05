using UnityEngine;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using Poukoute;

namespace UnityEditor.UI {
    [CustomEditor(typeof(CustomTextHref), true)]
    [CanEditMultipleObjects]
    public class CustomTextHrefEditor : TextEditor {
        public override void OnInspectorGUI() {
            CustomTextHref component = (CustomTextHref)target;
            base.OnInspectorGUI();
            component.maxWidth = EditorGUILayout.FloatField("Max text width", component.maxWidth);
            serializedObject.ApplyModifiedProperties();
        }
    }
}