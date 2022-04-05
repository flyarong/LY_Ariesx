using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VisualDesignCafe.Editor.Prefabs;

namespace Poukoute {
    [ExecuteInEditMode]
    [CustomEditor(typeof(MiniMap), true)]
    public class MiniMapEditor : Editor {
        private MiniMap view;


        private GameObject configureManager;
        private GameObject poolManager;

        private Vector2 originCoordinate = new Vector2(510, 490);
        private const float miniSize = 3.0f;
        private const float maxSize = 7.0f;
        private const int MAP_NUM = 11;
        private Matrix4x4 originMatrixInvers = new Matrix4x4();
        private readonly Vector3 scaleVect = new Vector3(1.5f, 2.1f, 1);
        //private Dictionary<Vector2, MiniMapItem> MainMiniMapItems;
        //private Dictionary<int, Dictionary<Vector2, MiniMapItem>> AllMiniMapItems;

        private float ratio;
        private Vector2 maxCoordinate = new Vector2(1000, 1000);
        /*********************************/

        void OnEnable() {
            this.view = target as MiniMap;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset MiniMapInfo")) {
                this.ClearMiniMapInfo();
            }
            if (GUILayout.Button("Pre-Generate MiniMapInfo")) {
                this.GenerateMinimapItems();
            }
            GUILayout.EndHorizontal();
        }

        private void ClearMiniMapInfo() {
            GameHelper.ClearChildren(this.view.pnlMainCity, false);
            GameHelper.ClearChildren(this.view.pnlAllCity, false);
        }

        private void GenerateMinimapItems() {
            Caching.CleanCache();
            try {
                this.InitConfigure();
                this.ratio = 500 / this.maxCoordinate.y;
                Quaternion rotation = Quaternion.Euler(0, 0, 45);
                Matrix4x4 originMatrix = new Matrix4x4();
                originMatrix.SetTRS(Vector2.zero, rotation, scaleVect / (this.ratio));
                this.originMatrixInvers = Matrix4x4.Inverse(originMatrix);

                this.ClearMiniMapInfo();
                this.SetMainCity();
                this.SetAllCity();
            } finally {
                this.UninitConfigure();
            }
        }

        private void UninitConfigure() {
            GameObject.DestroyImmediate(this.configureManager);
            GameObject.DestroyImmediate(this.poolManager);
        }

        private void InitConfigure() {
            ConfigureGenerator.GeneratorConfigure();
            AssetDatabase.Refresh();
            this.configureManager = new GameObject();
            this.configureManager.name = "ConfigureManager";
            this.configureManager.transform.position = Vector3.zero;
            this.configureManager.transform.SetParent(this.view.transform);
            this.configureManager.AddComponent<ConfigureManager>();
            ConfigureManager.LoadCityEditorConfigures();

            this.poolManager = new GameObject();
            this.poolManager.name = "PoolManager";
            this.poolManager.transform.position = Vector3.zero;
            this.poolManager.transform.SetParent(this.view.transform);
            this.poolManager.AddComponent<PoolManager>();
            PoolManager poolManager = this.poolManager.GetComponent<PoolManager>();
            PoolManager.ConfigPoolManager(poolManager);
        }

        private void SetMainCity() {
            List<MiniMapPassConf> mainPassList =
                MiniMapPassConf.GetMainPassList();
            foreach (MiniMapPassConf pass in mainPassList) {
                GameObject gameObject = PoolManager.GetObject(PrefabPath.pnlMiniMapPass, this.view.pnlMainCity);
                gameObject.name =
                    string.Concat("Pass_", pass.coordinate.ToString());
                gameObject.transform.localScale *= 1 / miniSize;
                gameObject.GetComponent<RectTransform>().anchoredPosition =
                     this.originMatrixInvers * (pass.coordinate - originCoordinate);
                PrefabUtility.DisconnectPrefabInstance(gameObject);
            }

            List<NPCCityConf> npcCityList = NPCCityConf.GetMainCityList();
            MiniMapCityConf miniMapCityConf = null;
            foreach (NPCCityConf city in npcCityList) {
                miniMapCityConf = MiniMapCityConf.GetConf(city.id);
                MinimapItemType minimapItemType;
                string path;
                this.GetCityItemTypeAndPath(city.type, out minimapItemType, out path);
                if (path.CustomIsEmpty()) {
                    continue;
                }
                GameObject gameObject = PoolManager.GetObject(path, this.view.pnlMainCity);
                gameObject.name =
                    string.Concat(minimapItemType.ToString(), "_", miniMapCityConf.coordinate.ToString());
                gameObject.transform.localScale *= 1 / miniSize;
                gameObject.GetComponent<RectTransform>().anchoredPosition =
                    this.originMatrixInvers * (miniMapCityConf.coordinate - originCoordinate);
                PrefabUtility.DisconnectPrefabInstance(gameObject);
            }
        }

        private void SetAllCity() {
            List<NPCCityConf> npcCityConf = null;
            MiniMapCityConf miniMapCityConf = null;
            List<MiniMapPassConf> passBridgeInMap = new List<MiniMapPassConf>();
            for (int i = 1; i <= MAP_NUM; i++) {
                npcCityConf = NPCCityConf.GetCityIn(i);
                foreach (NPCCityConf city in npcCityConf) {
                    MinimapItemType minimapItemType;
                    string path;
                    this.GetCityItemTypeAndPath(city.type, out minimapItemType, out path);
                    if (path.CustomIsEmpty()) {
                        continue;
                    }
                    miniMapCityConf = MiniMapCityConf.GetConf(city.id);
                    GameObject gameObject = PoolManager.GetObject(path, this.view.pnlAllCity);
                    gameObject.name =
                        string.Concat(minimapItemType, "_", miniMapCityConf.coordinate.ToString());
                    gameObject.transform.localScale *= 1 / maxSize;
                    gameObject.GetComponent<RectTransform>().anchoredPosition =
                        this.originMatrixInvers * (miniMapCityConf.coordinate - originCoordinate);
                    PrefabUtility.DisconnectPrefabInstance(gameObject);
                }
                passBridgeInMap = MiniMapPassConf.GetPassIn(i);
                foreach (MiniMapPassConf pass in passBridgeInMap) {
                    GameObject gameObject = PoolManager.GetObject(PrefabPath.pnlMiniMapPass, this.view.pnlAllCity);
                    gameObject.name =
                        string.Concat("Pass_", pass.coordinate.ToString());
                    gameObject.transform.localScale *= 1 / maxSize;
                    gameObject.GetComponent<RectTransform>().anchoredPosition =
                        this.originMatrixInvers * (pass.coordinate - originCoordinate);
                    PrefabUtility.DisconnectPrefabInstance(gameObject);
                }
            }
        }


        private void GetCityItemTypeAndPath(string cityType,
            out MinimapItemType minimapItemType, out string path) {
            minimapItemType = MinimapItemType.Zone;
            path = string.Empty;
            switch (cityType) {
                case GameConst.CAPITAL:
                    minimapItemType = MinimapItemType.Capital;
                    path = PrefabPath.pnlMiniMapCapital;
                    break;
                case GameConst.REGION_CAPITAL:
                    minimapItemType = MinimapItemType.State;
                    path = PrefabPath.pnlMiniMapState;
                    break;
                case GameConst.ZONE_CAPITAL:
                    minimapItemType = MinimapItemType.Zone;
                    path = PrefabPath.pnlMiniMapZone;
                    break;
                case GameConst.NORMAL_CITY:
                    minimapItemType = MinimapItemType.City;
                    path = PrefabPath.pnlMiniMapCity;
                    break;
                default:
                    Debug.LogError("No such city type " + cityType);
                    break;
            }
        }

    }
}