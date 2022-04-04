using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class SceneToModel {
        private static SceneToModel self;
        public static SceneToModel Instance {
            get {
                if (self == null) {
                    self = new SceneToModel();
                }
                return self;
            }
        }

        private Dictionary<string, List<string>> sceneToModelDict;

        private SceneToModel() {
            this.sceneToModelDict = new Dictionary<string, List<string>>();

        }

        private List<string> InnerGetModelListByScene(string scene) {
            List<string> modelList = new List<string>();
            this.sceneToModelDict.TryGetValue(scene, out modelList);
            return modelList;
            //if (this.sceneToModelDict.ContainsKey(scene)) {
            //    return this.sceneToModelDict[scene];
            //} else {
            //    Debug.LogError("There is no such scene model map.");
            //    return null;
            //}
        }

        public static List<string> GetModelListByScene(string scene) {
            return Instance.InnerGetModelListByScene(scene);
        }
    }
}
