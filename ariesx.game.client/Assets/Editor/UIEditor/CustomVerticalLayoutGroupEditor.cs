using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

namespace Poukoute {

    [CustomEditor(typeof(CustomVerticalLayoutGroup))]
    public class CustomVerticalLayoutGroupEditor : UnityEditor.UI.HorizontalOrVerticalLayoutGroupEditor {
        public override void OnInspectorGUI() {

            CustomVerticalLayoutGroup component = (CustomVerticalLayoutGroup)target;

            base.OnInspectorGUI();

            component.size = EditorGUILayout.FloatField("Cell Size", component.size);
            component.scrollRect = (CustomScrollRect)EditorGUILayout.ObjectField("Scroll Rect", component.scrollRect, typeof(CustomScrollRect), true);
        }
    }
}
