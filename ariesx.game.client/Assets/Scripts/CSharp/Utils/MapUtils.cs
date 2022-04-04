using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class MapUtils {
        private static MapUtils self;
        public static MapUtils Instance {
            get {
                if (self == null) {
                    self = new MapUtils();
                }
                return self;
            }
        }

        private MapUtils() {
            this.uiRect = UIManager.UIRect;
        }

        public Vector2 tileSize = new Vector2(1.85f, 1.305f);
        public Vector2 tileHorizon = new Vector2(1.85f, -1.305f);
        public static Vector2 TileHorizon {
            get {
                return Instance.tileHorizon;
            }
        }
        public Vector2 tileVertical = new Vector2(1.85f, 1.305f);
        public static Vector2 TileVertical {
            get {
                return Instance.tileVertical;
            }
        }
        public static float ScreenUIRatio {
            get {
                return Screen.height / Instance.uiRect.height;
            }
        }
        public Rect uiRect = new Rect();
        public static Rect UIRect {
            get {
                return Instance.uiRect;
            }
        }

        public static Vector2 TileSize {
            get {
                return Instance.tileSize;
            }
        }
        private Vector2 originCoordinate = new Vector2(510, 490);
        public static Vector2 Center {
            get {
                return Instance.originCoordinate;
            }
        }

        public static Vector2 CoordinateToPosition(Vector2 coordinate) {
            return (coordinate.x - Instance.originCoordinate.x) * Instance.tileHorizon +
                (coordinate.y - Instance.originCoordinate.y) * Instance.tileVertical;
        }

        public static Vector2 PositionToCoordinate(Vector2 position, int oddEven = 0) {
            float positionX = position.x / Instance.tileSize.x;
            float positionY = position.y / Instance.tileSize.y;
            Vector2 coordinateX = positionX / 2 * Vector2.one;
            Vector2 coordinateY = positionY / 2 * GameConst.LeftUp;

            Vector2 coordinate = coordinateX + coordinateY + Instance.originCoordinate;

            Vector2[] alternateArray = {
                new Vector2(Mathf.Ceil(coordinate.x), Mathf.Ceil(coordinate.y)),
                new Vector2(Mathf.Floor(coordinate.x), Mathf.Floor(coordinate.y)),
                new Vector2(Mathf.Ceil(coordinate.x), Mathf.Floor(coordinate.y)),
                new Vector2(Mathf.Floor(coordinate.x), Mathf.Ceil(coordinate.y)),
            };

            if (oddEven == 1) {
                Vector2 total = Vector2.zero;
                foreach (Vector2 alternate in alternateArray) {
                    total += alternate;
                }
                if (alternateArray[0] == alternateArray[1]) {
                    float signX = Mathf.Sign(Instance.originCoordinate.x - alternateArray[0].x);
                    float signY = Mathf.Sign(Instance.originCoordinate.y - alternateArray[0].y);
                    return 0.25f * total + new Vector2(0.5f * signX, 0.5f * signY);
                } else {
                    return 0.25f * total;
                }
            }

            Vector2 selectedCoordinate = new Vector2(-99, -99);
            for (int i = 0; i < alternateArray.Length; i++) {
                if ((alternateArray[i] - coordinate).sqrMagnitude <
                    (selectedCoordinate - coordinate).sqrMagnitude) {
                    selectedCoordinate = alternateArray[i];
                }
            }
            return selectedCoordinate;
        }

        public static Vector2 WorldToScreenPoint(Vector2 point) {
            Vector2 offsetWorld = point - (Vector2)GameManager.MainCamera.transform.position;
            Vector2 offsetScreen = offsetWorld / (GameManager.MainCamera.orthographicSize * 2) * Screen.height;
            return offsetScreen + new Vector2(Screen.width / 2, Screen.height / 2);
        }

        public static Vector2 ScreenToWorldPoint(Vector2 point) {
            Vector2 offsetScreen = point - new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 offsetWorld = offsetScreen / Screen.height * (GameManager.MainCamera.orthographicSize * 2);
            return offsetWorld + (Vector2)GameManager.MainCamera.transform.position;
        }

        public static Vector2 WorldToUIPoint(Vector2 point, Camera camera = null) {
            if (camera == null) {
                camera = Camera.main;
            }
            Vector2 offsetWorld = point - (Vector2)camera.transform.position;
            Vector2 offsetUI = offsetWorld * (Instance.uiRect.height / (camera.orthographicSize * 2));
            return offsetUI + new Vector2(Instance.uiRect.width / 2, -Instance.uiRect.height / 2);
        }

        public static Vector2 UIToWorldPoint(Vector2 point) {
            Vector2 offsetUi = point - new Vector2(Instance.uiRect.width / 2, -Instance.uiRect.height / 2);
            Vector2 offsetWorld = offsetUi * ((GameManager.MainCamera.orthographicSize * 2) / Instance.uiRect.height);
            return offsetWorld + (Vector2)GameManager.MainCamera.transform.position;
        }

        public static Vector2 UIToScreenPoint(Vector2 point) {
            Vector2 offsetUi = point - new Vector2(Instance.uiRect.width / 2, -Instance.uiRect.height / 2);
            Vector2 offsetScreen = offsetUi * (Screen.height / Instance.uiRect.height);
            return offsetScreen + new Vector2(Screen.width / 2, Screen.height / 2);
        }

        public static Vector2 ScreenToUIPoint(Vector2 point) {
            Vector2 offsetScreen = point - new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 offsetUi = offsetScreen * (Instance.uiRect.height / Screen.height);
            return offsetUi + new Vector2(Instance.uiRect.width / 2, -Instance.uiRect.height / 2);
        }

        public static string GetTileName(MapTileInfo tileInfo) {
            if (tileInfo.type.CustomEquals(ElementCategory.building)) {
                return GetBuildingLocalName(tileInfo.buildingInfo);
            } else if (tileInfo.type.CustomEquals(ElementCategory.resource)) {
                return LocalManager.GetValue("resource_", tileInfo.name);
            } else if (tileInfo.type.CustomEquals(ElementCategory.pass)) {
                return LocalManager.GetValue("pass_", tileInfo.pass.id.Replace(',', '_'));
            } else if (tileInfo.type.CustomEquals(ElementCategory.npc_city)) {
                return NPCCityConf.GetNpcCityLocalName(tileInfo.name, tileInfo.city.isCenter);
            } else {
                return LocalManager.GetValue("tile_", tileInfo.name);
            }
        }

        public static string GetTileNameAndLevelLocal(MapTileInfo tileInfo) {
            string tileNameLocal = GetTileName(tileInfo);
            string tileLevelLocal = string.Empty;
            if (tileInfo.level > 0) {
                tileLevelLocal =
                    string.Format(LocalManager.GetValue(LocalHashConst.level), tileInfo.level);
            }
            return string.Concat(tileNameLocal, " ", tileLevelLocal);
        }

        public static string GetBuildingLocalName(ElementBuilding buildingInfo) {
            BuildingConf buildingConf = BuildingConf.GetConf(buildingInfo.GetId());
            return MapUtils.GetBuildingLocalName(buildingConf);
        }

        public static string GetBuildingLocalName(BuildingConf buildingConf) {
            return MapUtils.GetBuildingLocalName(buildingConf.type, buildingConf.buildingName);
        }

        public static string GetBuildingLocalName(string buildingType, string buildingName) {
            if (buildingType.CustomEquals("armycamp")) {
                return TroopModel.GetArmyCampLocal(buildingName);
            } else if (buildingType.Contains("produce")) {
                return TroopModel.GetProduceLocal(buildingName, buildingType);
            } else if (buildingType.CustomEquals("tribute_gold")) {
                return TroopModel.GetTributeLocal(buildingName, buildingType);
            } else {
                return LocalManager.GetValue("name_", buildingType);
            }
        }



        public static string GetUnlockBuildingName(string buildingName) {
            char[] nameList = buildingName.ToCharArray();
            if (char.IsUpper( nameList[nameList.Length - 1])) {
                string[] NameSplit = buildingName.Split('_');
                char[] charList = NameSplit[NameSplit.Length - 1].ToCharArray();
                int name = 0;
                for (int i = charList.Length - 1; i >= 0; i--) {
                    name += ((int)charList[i] - 64) * (int)(Math.Pow(26, 0));
                }
                string oldString = "_" + NameSplit[NameSplit.Length - 1];
                string newString = "," + name.ToString();
                buildingName = buildingName.Replace(oldString, newString);
            }
            return buildingName;
        }
    }




}
