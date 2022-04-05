using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Poukoute;

namespace UnityEditor.UI {
    [CustomEditor(typeof(CustomButton), true)]
    [CanEditMultipleObjects]
    public class CustomButtonEditor : ButtonEditor {
        SerializedProperty m_btnType;

        protected override void OnEnable() {
            base.OnEnable();
            m_btnType = serializedObject.FindProperty("m_btnType");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_btnType);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
