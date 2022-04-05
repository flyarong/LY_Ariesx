using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Poukoute {
    [CustomEditor(typeof(Empty4Raycast))]
    public class Empty4RaycastEditor : Editor {

        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField(
              new GUIContent("Block Raycast")
          );
        }
    }
}
