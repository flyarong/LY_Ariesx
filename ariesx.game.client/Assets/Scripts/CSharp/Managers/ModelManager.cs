using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {

    public class ModelManager : MonoBehaviour {
        private static ModelManager self;
        //private List<Dictionary<string, IExtensible>> sceneStack;
        private Dictionary<string, BaseModel> sceneDatas;

        //private const int stackLength = 100;
        public static ModelManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("ModelManager is not initialized");
                }
                return self;
            }
        }

        void Awake() {
            self = this;
            //this.sceneStack = new List<Dictionary<string, IExtensible>>(10);
            this.sceneDatas = new Dictionary<string, BaseModel>(20);
        }

        #region Scene

        /// <summary>
        /// sync load scene, get the scene data after scene loading.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="isAdditive"></param>
        public static void LoadScene(string sceneName, bool isAdditive = false) {
            Instance.InnerLoadScene(sceneName, isAdditive);
        }

        public static AsyncOperation LoadSceneAsync(string sceneName, bool isAdditive = false) {
            return Instance.InnerLoadSceneAsync(sceneName, isAdditive);
        }

        public static void UnLoadScene(string name) {
            Instance.InnerUnLoadScene(name);
        }

        private bool IsSceneAvaliable(string name) {
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++) {
                if (SceneManager.GetSceneAt(i).name.CustomEquals(name)) {
                    return true;
                }
            }
            return false;
        }

        private void InnerLoadScene(string sceneName, bool isAddtive) {
            if (!this.IsSceneAvaliable(sceneName)) {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
        }

        private AsyncOperation InnerLoadSceneAsync(string sceneName, bool isAdditive) {
            if (!this.IsSceneAvaliable(sceneName)) {
                return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
            Debug.LogError(sceneName + " is already loaded!!");
            return null;
        }

        private void InnerUnLoadScene(string sceneName) {
            if (this.IsSceneAvaliable(sceneName)) {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            }
        }

        //private void ScenePush(string name, IExtensible req) {
        //    Dictionary<string, IExtensible> curScenes = new Dictionary<string, IExtensible>();
        //    curScenes.Add(name, req);
        //    if (this.sceneStack.Count > stackLength) {
        //        this.sceneStack.RemoveAt(0);
        //    }
        //    this.sceneStack.Add(curScenes);
        //}
        #endregion

        #region SceneData
        private BaseModel InnerGetModelData(string modelName) {
            BaseModel baseModel;
            return Instance.sceneDatas.TryGetValue(modelName, out baseModel) ? baseModel : null;
        }

        private void InnerSetModelData(string modelName, BaseModel sceneData) {
            if (sceneData != null) {
                this.sceneDatas[modelName] = sceneData;
            }
        }

        public static T GetModelData<T>() where T : BaseModel {
            string name = typeof(T).Name;
            
            T model = (T)Instance.InnerGetModelData(typeof(T).Name);
            if (model != null) {
                return (T)model;
            } else {
                model = (T)Activator.CreateInstance(typeof(T));
                Instance.InnerSetModelData(name, model);
                return model;
            }
        }


        #endregion
    }
}
