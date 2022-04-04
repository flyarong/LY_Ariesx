using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class MapElementManager : MonoBehaviour {
        private static MapElementManager self;
        public static MapElementManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("MapElementManager is not initialized.");
                }
                return self;
            }
        }

        private GameObject cloudRoot;

        private List<GameObject> cloudList = new List<GameObject>(3) { null, null, null };

        void Awake() {
            self = this;

            this.cloudRoot = new GameObject();
            this.cloudRoot.transform.SetParent(this.transform);
            this.cloudRoot.name = "CloudRoot";
        }
        
        public void RemoveElement(int index) {
            if (this.cloudList[index] != null) {
                this.cloudList[index].SetActiveSafe(false);
                PoolManager.RemoveObject(this.cloudList[index]);
                this.cloudList[index] = null;
            }
            int cloudListCount = this.cloudList.Count;
            for (int i = 0; i < cloudListCount; i++) {
                if (this.cloudList[i] == null) {
                    this.cloudList[i] =
                        PoolManager.GetObject(PrefabPath.cloud, this.cloudRoot.transform);
                    this.cloudList[i].GetComponent<CloudView>().areaIndex = i;
                    this.cloudList[i].SetActiveSafe(true);
                }
            }
        }

        public static void ShowCloud() {
            foreach(GameObject cloud in Instance.cloudList) {
                cloud.GetComponent<SpriteRenderer>().enabled = true;
            }
        }

        public static void HideCloud() {
            foreach (GameObject cloud in Instance.cloudList) {
                cloud.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        public static void ShowElement() {
            Instance.gameObject.AddComponent<AnimalElementRootView>();
            Instance.gameObject.AddComponent<EagleElementRootView>();
        }
    }
}
