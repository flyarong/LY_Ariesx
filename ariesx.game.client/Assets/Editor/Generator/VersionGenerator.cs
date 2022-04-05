using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Poukoute {

    public class VersionGenerator : EditorWindow {
        VersionType verionType = VersionType.develop;

        void OnGUI() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Version");
            verionType = (VersionType)EditorGUILayout.EnumPopup(verionType);
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Set")) {
                VersionController.GenerateVersionConst(verionType);
                AssetDatabase.Refresh();
            }

        }
    }
}
