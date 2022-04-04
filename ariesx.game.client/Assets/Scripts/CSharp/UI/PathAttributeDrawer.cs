using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
namespace Poukoute {
    [CustomPropertyDrawer(typeof(PathAttribute))]
    public class PathAttributeDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            BaseViewPreference viewPreference = (BaseViewPreference)property.serializedObject.targetObject;
            EditorGUI.ObjectField(position, property);
            if (property.objectReferenceValue != null) {
                GameObject target = property.objectReferenceValue as GameObject;
                if (target == null) {
                    target = ((Component)property.objectReferenceValue).gameObject;
                }
                viewPreference.pathDict[property.name] = target;
            }
            
        }
    }
}
#endif
