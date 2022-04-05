using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Poukoute {
    [ExecuteInEditMode]
    [CustomEditor(typeof(PrefabLoader), true)]
    public class PrefabLoaderEditor : Editor {
        private PrefabLoader view;
        private Transform subPrefab;
        //private Transform subPrefabsParent;

        void OnEnable() {
            this.view = target as PrefabLoader;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load from Prefab")) {
                this.view.LoadSubPrefab();
            }
            if (GUILayout.Button("Save sub Prefab")) {
                this.SaveSubPrefab();
            }
            GUILayout.EndHorizontal();
        }

        private void SaveSubPrefab() {
            if (this.view.transform.childCount == 0) {
                Debug.LogError("No sub-preefab in holder!!");
                return;
            }
            this.subPrefab = this.view.transform.GetChild(0);
            PrefabType subPrefabType = PrefabUtility.GetPrefabType(this.subPrefab);
            if (subPrefabType == PrefabType.PrefabInstance) {
                var go = PrefabUtility.GetPrefabParent(this.subPrefab);
                string childPath = AssetDatabase.GetAssetPath(go);
                if (childPath.CustomIsEmpty()) {
                    Debug.LogError(this.view.name + " not a prefab!");
                    return;
                }
                childPath = childPath.Substring(childPath.IndexOf('/', childPath.IndexOf('/') + 1) + 1);
                Debug.Log(this.view.gameObject.name + " " + childPath);
                childPath = childPath.Substring(0, childPath.LastIndexOf("."));
                if (childPath.CustomIsEmpty()) {
                    return;
                }
                this.view.prefabPath = childPath;
                //this.view.localPosition = this.subPrefab.localPosition;
                RectTransform subPrefabRect = this.subPrefab.GetComponent<RectTransform>();
                this.view.offsetMax = subPrefabRect.offsetMax;
                this.view.offsetMin = subPrefabRect.offsetMin;
                this.OnSaveSubPrefab();
            } else {
                Debug.LogError(this.view.name + " not a prefab!");
            }
        }

        private void OnSaveSubPrefab() {
            if (this.subPrefab == null) {
                return;
            }
            Tool.SavePefabChange(this.subPrefab.gameObject);
            GameObject.DestroyImmediate(this.subPrefab.gameObject);
            Tool.SavePefabChange(this.view.gameObject);
            this.view.LoadSubPrefab();
        }
    }
}