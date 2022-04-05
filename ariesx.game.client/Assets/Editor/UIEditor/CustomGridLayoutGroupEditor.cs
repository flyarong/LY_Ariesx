using UnityEngine.UI;

namespace UnityEditor.UI {
    [CustomEditor(typeof(CustomGridLayoutGroup), true)]
    [CanEditMultipleObjects]
    public class CustomGridLayoutGroupEditor : GridLayoutGroupEditor {

        public override void OnInspectorGUI() {
            CustomGridLayoutGroup component = (CustomGridLayoutGroup)target;


            base.OnInspectorGUI();
  
            component.size = EditorGUILayout.FloatField("Cell Size", component.size);
            component.scrollRect = (CustomScrollRect)EditorGUILayout.ObjectField("Scroll Rect", component.scrollRect, typeof(CustomScrollRect), true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}