using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Poukoute {
    public class PrefabLoader : MonoBehaviour {
        public string prefabPath;
        //public Vector3 localPosition;
        public Vector2 offsetMax;
        public Vector2 offsetMin;

        public bool autoLoad = true;
        public int autoLoadDelayFrams = 0;


        private void Awake() {
            if (this.transform.childCount < 1 && this.autoLoad) {
                StartCoroutine(this.InnterLoadSubPrefab());
            }
        }

        private IEnumerator InnterLoadSubPrefab() {
            yield return YieldManager.GetWaitForSeconds(autoLoadDelayFrams);
            this.LoadSubPrefab();
        }

        public GameObject LoadSubPrefab() {
            GameHelper.ClearChildren(this.transform);
            if (!this.prefabPath.CustomIsEmpty()) {
                GameObject prefabObj = UnityEngine.Resources.Load<GameObject>(this.prefabPath);
                if (prefabObj != null) {
                    GameObject subPrefabObj = GameObject.Instantiate(prefabObj);
                    subPrefabObj.name = subPrefabObj.name.Replace("(Clone)", "");
                    subPrefabObj.transform.SetParent(this.transform);
                    RectTransform subPrefabRect = subPrefabObj.GetComponent<RectTransform>();
                    subPrefabRect.localPosition = Vector3.zero;
                    subPrefabRect.localScale = Vector3.one;
                    subPrefabRect.offsetMax = this.offsetMax;
                    subPrefabRect.offsetMin = this.offsetMin;
#if UNITY_EDITOR
                    PrefabUtility.CreatePrefab("Assets/Resources/" + this.prefabPath + ".prefab",
                        subPrefabObj, ReplacePrefabOptions.ConnectToPrefab);
#endif
                    return subPrefabObj;
                }
#if UNITY_EDITOR
            else {
                    Debug.LogError("Prefab not exist at " + this.prefabPath);
                }
#endif
            }
#if UNITY_EDITOR
            else {
                Debug.LogError("Prefab name is empty or null!");
            }
#endif

            return null;
        }
    }
}