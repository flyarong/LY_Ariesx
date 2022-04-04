using Poukoute;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Poukoute {
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PreferenceAttribute))]
    public class PreferenceAttributeDrawer : PropertyDrawer {
        private PreferenceAttribute _attributeValue = null;
        private PreferenceAttribute attributeValue {
            get {
                if (_attributeValue == null) {
                    _attributeValue = (PreferenceAttribute)attribute;
                }
                return _attributeValue;
            }
        }
        private static Assembly unityEngineAssembly = typeof(GameObject).Assembly;
        private static string unityEnginePrefix = "UnityEngine.";
        private static Assembly unityUIAssembly = typeof(Button).Assembly;
        private static string unityUIPrefix = "UnityEngine.UI.";
        private static Assembly custom = typeof(CustomButton).Assembly;
        private static string customPrefix = "Poukoute.";
        private static Assembly Tmp = typeof(TextMeshProUGUI).Assembly;
        private static string TmpPrefix = "TMPro.";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Transform root = ((MonoBehaviour)property.serializedObject.targetObject).transform;
            EditorGUI.PropertyField(position, property);
            string path = attributeValue.preferencePath;
            string[] pathArray = path.CustomSplit('.');

            if (pathArray[0].Contains("$")) {
                string parent = pathArray[0].Substring(1);
                FieldInfo parentFiled = property.serializedObject.targetObject.GetType().GetField(parent);
                PreferenceAttribute parentAttribute = parentFiled.GetCustomAttributes(
                    typeof(PreferenceAttribute), false)[0] as PreferenceAttribute;
                path = string.Concat(parentAttribute.preferencePath, path.Substring(1 + parent.Length));
                pathArray = path.CustomSplit('.');
            }
            Transform target = root;
            foreach (string pathSingle in pathArray) {
                target = target.Find(pathSingle);
                if (target == null) {
                    Debug.LogErrorf("Wrong path of preference {0}", path);
                    return;
                }
            }

            int indexStart = property.type.IndexOf('<') + 2;
            int indexEnd = property.type.IndexOf('>');
            string typeName = property.type.Substring(indexStart, indexEnd - indexStart);
            if (typeName == "GameObject") {
                property.objectReferenceValue = target.gameObject;
            } else {
                property.objectReferenceValue = target.GetComponent(typeName);
            }
        }
    }
#endif
}
