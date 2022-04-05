using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProtoBuf;
using Protocol;

namespace Poukoute {

    public class MapGenerator : MonoBehaviour {
        private static Dictionary<Vector2, Point> pointDict = new Dictionary<Vector2, Point>();
        private static Dictionary<Vector2, GameObject> borderDict = new Dictionary<Vector2, GameObject>();

        private static Dictionary<string, Point> cityCenterDict = new Dictionary<string, Point>();
        private static Dictionary<int, int> cityEntranceS = new Dictionary<int, int>() {
            {2, GameConst.CITY_ENTRANCE_NS },
            {4, GameConst.CITY_ENTRANCE_EW },
            {6, GameConst.CITY_ENTRANCE_EW },
            {8, GameConst.CITY_ENTRANCE_NS }
        };

        private static Dictionary<int, int> cityEntranceL = new Dictionary<int, int>() {
            {3, GameConst.CITY_ENTRANCE_NS },
            {11, GameConst.CITY_ENTRANCE_EW },
            {15, GameConst.CITY_ENTRANCE_EW },
            {23, GameConst.CITY_ENTRANCE_NS }
        };

        private static Vector2 size = new Vector2(1000, 1000);
        private static Vector2 blockSize = new Vector2(50, 50);
        private static StreamWriter writerCity;
        private static StreamWriter writerPass;

        private static GameObject configureManager;

        private static GameObject miniMapItemPrefab = UnityEngine.Resources.Load<GameObject>("MiniMapItem");
        private static int count = 0;

        [MenuItem("Poukoute/Generator/Generate Map")]
        private static void GenerateMap() {
            Caching.CleanCache();
            try {
                InitConfigure();
                DeserilizeMap();
                SerilizeMap();
            } finally {
                UninitConfigure();
            }
        }

        private static void InitConfigure() {
            ConfigureGenerator.GeneratorConfigure();
            AssetDatabase.Refresh();
            configureManager = new GameObject();
            configureManager.name = "ConfigureManager";
            configureManager.transform.position = UnityEngine.Vector3.zero;
            configureManager.AddComponent<ConfigureManager>();
            ConfigureManager.LoadCityEditorConfigures();
        }

        private static void UninitConfigure() {
            
            GameObject.DestroyImmediate(configureManager);
        }

        private static void DeserilizeMap() {
            string path = "Assets/Arts/Map/static_map.bytes";
            TextAsset blockText = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            MemoryStream m = new MemoryStream(blockText.bytes);
            StaticMap data = Serializer.Deserialize(typeof(StaticMap), m) as StaticMap;
            foreach (Point point in data.Points) {
                Vector2 coord = new Vector2(point.Coord.X, point.Coord.Y);
                //if (coord == new Vector2(249, 600)) {
                //    Debug.LogError(point.GetLevel());
                //}
                if (point.ElementType == (int)ElementType.npc_city &&
                    point.NpcCity.IsCenter) {
                    string sn = string.Concat(point.MapSN, ",", point.ZoneSN, ",", point.NpcCity.SN);
                    cityCenterDict.Add(sn, point);
                }
                pointDict.Add(coord, point);
            }
        }

        private static void SerilizeMap() {
            List<Vector2> offsetList = new List<Vector2> {
                Vector2.right,
                Vector2.down,
                Vector2.right + Vector2.down
            };

            List<ElementType> noMoutainList = new List<ElementType> {
                ElementType.pass,
                ElementType.road,
                ElementType.mountain,
                ElementType.river,
                ElementType.camp,
            };
            int count = 0;
            foreach (var pair in pointDict) {
                Point point = pair.Value;
                Vector2 coordinate = new Vector2(point.Coord.X, point.Coord.Y);
                if (point.ElementType == (int)ElementType.mountain) {
                    count++;
                    bool hasPass = false;

                    foreach (Vector2 mountainOffset in offsetList) {
                        Vector2 mountain = coordinate + mountainOffset;
                        if (pointDict.ContainsKey(mountain) &&
                            !noMoutainList.Contains((ElementType)(pointDict[mountain].ElementType))) {
                            pointDict[mountain].ElementType = (int)ElementType.townhall;
                            pointDict[mountain].Building = new ElementBuilding {
                                Level = 0
                            };
                        } else {
                            hasPass = true;
                        }
                    }
                    if (hasPass) {
                        foreach (Vector2 mountainOffset in offsetList) {
                            Vector2 mountain = coordinate + mountainOffset;
                            if (pointDict.ContainsKey(mountain) &&
                           !noMoutainList.Contains((ElementType)(pointDict[mountain].ElementType))) {
                                pointDict[mountain].ElementType = (int)ElementType.townhall;
                                pointDict[mountain].Building = new ElementBuilding {
                                    Level = 8
                                };
                            }
                        }
                        pointDict[pair.Key].ElementType = (int)ElementType.townhall;
                        pointDict[pair.Key].Building = new ElementBuilding {
                            Level = 8
                        };
                    } else {

                    }
                }
            }
            Debug.LogError(count);
            // return;
            /////////
            borderDict.Clear();
            string pathCity = Application.dataPath + "/Configures/mini_map_city.csv";
            string pathPass = Application.dataPath + "/Configures/mini_map_pass.csv";
            File.WriteAllText(pathCity, "");
            File.WriteAllText(pathPass, "");
            writerCity = new StreamWriter(pathCity, true, Encoding.UTF8);
            writerPass = new StreamWriter(pathPass, true, Encoding.UTF8);
            writerCity.WriteLine("id,x,y");
            writerPass.WriteLine("id,type,x,y,level,state");
            Vector2 blockCount = new Vector2(Mathf.Ceil(size.x / blockSize.x), Mathf.Ceil(size.y / blockSize.y));
            for (int i = 0; i < blockCount.x; i++) {
                for (int j = 0; j < blockCount.y; j++) {
                    GenerateBlock(i, j);
                }
            }
            writerCity.Flush();
            writerPass.Flush();
            writerCity.Close();
            writerPass.Close();
        }

        private static void GenerateBlock(float x, float y) {
            string offset = x + "," + y;
            List<byte> byteList = new List<byte>();
            for (float i = blockSize.x * x + 1; i <= blockSize.x * (x + 1); i++) {
                for (float j = blockSize.y * y + 1; j <= blockSize.y * (y + 1); j++) {
                    Vector2 coordinate = new Vector2(i, j);
                    Point point = pointDict[coordinate];

                    GenerateBytes(point, byteList);
                    GenerateMiniMap(point);
                    GenerateCityConfigure(point, coordinate);
                }
            }
            File.WriteAllBytes(Application.dataPath + "/Resources/" + Poukoute.Path.mapData + offset + ".proto.bytes", byteList.ToArray());
        }

        private static void GenerateBytes(Point point, List<byte> byteList) {
            ushort level = 0;
            if (point.ElementType == (int)ElementType.plain) {
                level = (ushort)1;
            } else if (point.ElementType >= (int)ElementType.food &&
                point.ElementType <= (int)ElementType.crystal) {
                level = (ushort)point.Resource.Level;
            } else if (point.ElementType == (int)ElementType.npc_city) {
                string sn = string.Concat(point.MapSN, ",", point.ZoneSN, ",", point.NpcCity.SN);
                NPCCityConf cityConf = NPCCityConf.GetConf(string.Concat(sn, ",", 1));
                Point cityCenter;
                Dictionary<int, int> cityEntrance = null;
                level = (ushort)(point.NpcCity.SN - GameConst.NPC_CITY_SN);
                if (cityCenterDict.TryGetValue(sn, out cityCenter)) {
                    Vector2 coordinate = new Vector2(point.Coord.X, point.Coord.Y);
                    Vector2 center = new Vector2(cityCenter.Coord.X, cityCenter.Coord.Y);
                    Vector2 origin = Vector2.zero;
                    int columnLength = 0;
                    int entrance = 0;
                    switch (cityConf.size) {
                        case "l":
                            origin = Vector2.one * -2;
                            columnLength = 5;
                            cityEntrance = cityEntranceL;
                            break;
                        case "m":
                        case "s":
                            origin = Vector2.one * -1;
                            columnLength = 3;
                            cityEntrance = cityEntranceS;
                            break;
                    }
                    Vector2 offset = coordinate - (center + origin);
                    int index = Mathf.RoundToInt(offset.x + offset.y * columnLength) + 1;
                    if (cityEntrance.TryGetValue(index, out entrance)) {
                        level += (ushort)entrance;
                    }
                }

                if (point.NpcCity.IsCenter) {
                    level += GameConst.CITY_CENTER;
                }
            } else if (point.ElementType == (int)ElementType.townhall) {
                level = (ushort)point.Building.Level;
                point.ElementType = (int)ElementType.mountain;
            } else if (point.ElementType == (int)ElementType.mountain) {
                level = (ushort)Random.Range(1, 8);
            } else if (point.ElementType == (int)ElementType.camp) {
                level = (ushort)Random.Range(1, 4);
            } else if (point.ElementType == (int)ElementType.road) {
                level = (ushort)Random.Range(1, 4);
            }
            byteList.Add((byte)point.MapSN);
            byteList.Add((byte)point.ZoneSN);
            byteList.Add((byte)point.ElementType);
            byteList.Add((byte)level);
        }

        private static void GenerateMiniMap(Point point) {
            if (EditorSceneManager.GetActiveScene().name == "SceneMiniMap") {
                Vector2 coordinate = new Vector2(
                    Mathf.Floor(point.Coord.X / 10),
                    Mathf.Floor(point.Coord.Y / 10)
                );
                if (!borderDict.ContainsKey(coordinate) || point.MapSN == 0) {
                    GameObject miniMapItem = null;
                    if (borderDict.ContainsKey(coordinate)) {
                        miniMapItem = borderDict[coordinate];
                    } else {
                        count++;
                        miniMapItem = Instantiate(miniMapItemPrefab);
                        borderDict.Add(coordinate, miniMapItem);
                    }
                    miniMapItem.transform.position = coordinate;
                    SpriteRenderer renderer = miniMapItem.GetComponent<SpriteRenderer>();
                    if (point.MapSN == 0) {
                        renderer.color = Color.red;
                    } else {
                        renderer.color = new Color(231 / 255f, 165 / 255f, 10 / 255f);
                    }
                }
            }
        }

        private static void GenerateCityConfigure(Point point, Vector2 coordinate) {
            if (point.ElementType == (int)ElementType.npc_city && (point.NpcCity.IsCenter)) {
                string SN = "\"" + point.MapSN + "," + point.ZoneSN + "," + point.NpcCity.SN + ",1" + "\"";
                writerCity.WriteLine(SN + "," + point.Coord.X + "," + point.Coord.Y);
            } else if (point.ElementType == (int)ElementType.pass) {
                Point upPoint = pointDict[new Vector2(coordinate.x + 1, coordinate.y)];
                Point rightPoint = pointDict[new Vector2(coordinate.x, coordinate.y + 1)];
                Point downPoint = pointDict[new Vector2(coordinate.x - 1, coordinate.y)];
                Point leftPoint = pointDict[new Vector2(coordinate.x, coordinate.y - 1)];

                string key = "\"" + point.MapSN + "," + point.ZoneSN + "," + point.Coord.X + "," + point.Coord.Y + "\"";
                writerPass.WriteLine(key + "," + point.Pass.Type + "," + point.Coord.X + "," + point.Coord.Y +
                    "," + point.Pass.Level + ",\"" + upPoint.MapSN + "," + rightPoint.MapSN + "," +
                    downPoint.MapSN + "," + leftPoint.MapSN + "\"");
            }
        }
    }
}
