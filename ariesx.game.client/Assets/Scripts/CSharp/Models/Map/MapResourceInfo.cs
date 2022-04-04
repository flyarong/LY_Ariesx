using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using Protocol;

namespace Poukoute {

    public class MapResouceInfo : MapInfo {
        public Dictionary<Vector2, Dictionary<uint, uint>> infoDict =
            new Dictionary<Vector2, Dictionary<uint, uint>>();
        public Dictionary<string, TextAsset> bytesDict =
            new Dictionary<string, TextAsset>();

        private Dictionary<string, TextAsset> blockAssetDict =
            new Dictionary<string, TextAsset>();

        private readonly List<Dictionary<uint, uint>> blockDictPool =
            new List<Dictionary<uint, uint>>();
        //private readonly List<string> directionList = new List<string> {
        //    "n", "nt", "e", "et", "s", "st", "w", "wt"
        //};

        // border
        private Rect borderRect;
        private Vector2 CornerA;
        private Vector2 CornerB;
        private Vector2 CornerC;
        private Vector2 CornerD;

        private readonly Dictionary<int, string> directionDict = new Dictionary<int, string> {
            { 255, "nnteetsstwwt" },//11111111
            { 239,"nntesstwwt"},//11101111
            { 191,"neetsstwwt"},//10111111
            { 251, "nnteetswwt"},//11111011
            { 254, "nnteetsstw" },//11111110
            { 235, "nnteswwt" },// 11101011
            { 250, "nnteetsw" },//11111010
            { 175, "nesstwwt"},//10101111
            { 190, "neetsstw"},//10111110

            { 62, "eetsstw" },//00111110
            { 143, "nsstwwt" },//10001111
            { 227, "nntewwt" },//11100011
            { 248, "nnteets" },//11111000

            { 184, "neets"},//10111000
            { 142, "nsstw"},//10001110
            { 226, "nntew" },//11100010
            { 58, "eetsw"},//00111010
            { 46, "esstw"},//00101110
            { 163, "newwt"},//10100011
            { 139,"nswwt" },//10001011
            { 232, "nntes"},//11101000

            { 170, "nesw" },//10101010
            { 42, "esw" },//00101010
            { 138, "nsw" },//10001010
            { 162, "new" },//10100010
            { 168, "nes" },//10101000
            { 56, "eets" },//00111000
            { 14, "sstw" },//00001110
            { 131, "nwwt" },//10000011
            { 224, "nnte" },//11100000
            { 34, "ew"},//00100010
            { 136, "ns"},//10001000
            { 40, "es" },//00101000
            { 10, "sw" },//00001010
            { 130, "nw" },//10000010
            { 160, "ne" },//10100000
            { 128, "n" },//10000000
            { 32, "e" },//00100000
            { 8, "s" },//00001000
            { 2, "w" },//00000010
        };

        //private readonly Dictionary<int, string> cityEntranceS = new Dictionary<int, string>() {
        //    {2, "ns" },
        //    {4, "ew" },
        //    {6, "ew" },
        //    {8, "ns" }
        //};

        //private readonly Dictionary<int, string> cityEntranceL = new Dictionary<int, string>() {
        //    {3, "ns" },
        //    {11, "ew" },
        //    {15, "ew" },
        //    {23, "ns" }
        //};

        private readonly Dictionary<Vector2, int> offsetDict = new Dictionary<Vector2, int> {
            { Vector2.up,  128 },
            { Vector2.one, 64 },
            { Vector2.right,  32 },
            { GameConst.RightDown,  16 },
            { Vector2.down, 8 },
            { GameConst.LeftDown, 4 },
            { Vector2.left, 2 },
            { GameConst.LeftUp, 1 }
        };

        public MapResouceInfo() {
            this.block = Vector2.zero;
            this.blockSize = new Vector2(50, 50);
            this.blockRect = new Rect(Vector2.zero, new Vector2(70f, 70f));
            this.maxBlock = new Vector2(
                Mathf.Ceil(this.maxCoordinate.x / this.blockSize.x) - 1,
                Mathf.Ceil(this.maxCoordinate.y / this.blockSize.y) - 1
            );
            this.CornerA = this.minCoordinate - Vector2.one;
            this.CornerB = new Vector2(this.minCoordinate.x - 1, this.maxCoordinate.y + 1);
            this.CornerC = this.maxCoordinate + Vector2.one;
            this.CornerD = new Vector2(this.maxCoordinate.x + 1, this.minCoordinate.y - 1);
            this.borderRect = new Rect(this.minCoordinate - Vector2.one,
                this.maxCoordinate - this.minCoordinate + Vector2.one * 3);
            //      Debug.LogError(borderRect + ":" + borderRect.Contains(Vector2.one * 1000));
        }

        private void InitBytesDict() {
            string blockStr = string.Empty;
            for (int i = 0; i < this.maxCoordinate.x / this.blockSize.x; i++) {
                for (int j = 0; j < this.maxCoordinate.y / this.blockSize.y; j++) {
                    blockStr = i + "," + j + ".proto";
                    this.blockAssetDict[blockStr] = UnityEngine.Resources.Load<TextAsset>(Path.mapData + blockStr);
                }
            }
        }

        public override void Refresh() {
            List<Vector2> blockList = this.GetBlockList(this.block);
            Dictionary<uint, uint> blockDict;
            int blockListCount = blockList.Count;
            for (int i = 0; i < blockListCount; i++) {
                blockDict = this.CreateBlockDict(blockList[i]);
                if (blockDict != null) {
                    this.infoDict.Add(blockList[i], blockDict);
                }
            }
        }

        public Dictionary<uint, uint> CreateBlockDict(Vector2 block) {
            //string blockStr = string.Concat(block.x, ",", block.y, ".proto");
            if (block.x < 0 || block.x > this.maxBlock.x || block.y < 0 || block.y > this.maxBlock.y) {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0},{1}.proto", block.x, block.y);
            TextAsset text = UnityEngine.Resources.Load<TextAsset>(
                string.Concat(Path.mapData, stringBuilder.ToString()));
            if (text == null) {
                return null;
            }
            byte[] bytes = text.bytes;
            int bytesLength = bytes.Length;

            //long start = System.DateTime.Now.Ticks;
            float originX = this.blockSize.x * block.x + 1;
            float originY = this.blockSize.y * block.y + 1;
            Dictionary<uint, uint> blockDict = this.GetBlockDict();
            float count;
            uint coordinate;
            uint info;
            for (int i = 0; i < bytesLength; i += 4) {
                count = Mathf.Floor(i / 4);
                coordinate = Vector2ToUint(
                    originX + Mathf.Floor(count / this.blockSize.x),
                    originY + count % this.blockSize.y
                );
                info = (uint)(bytes[i] << 24 | bytes[i + 1] << 16 |
                        bytes[i + 2] << 8 | bytes[i + 3]);
                blockDict.Add(coordinate, info);
            }
            //long end = System.DateTime.Now.Ticks;
            return blockDict;
        }

        private Dictionary<uint, uint> GetBlockDict() {
            if (this.blockDictPool.Count == 0) {
                return new Dictionary<uint, uint>();
            } else {
                Dictionary<uint, uint> dict = this.blockDictPool[0];
                this.blockDictPool.RemoveAt(0);
                dict.Clear();
                return dict;
            }
        }

        public void RemoveBlockDict(Dictionary<uint, uint> dict) {
            this.blockDictPool.Add(dict);
        }

        public string GetTileMapSnLocal(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                var blockData = this.CreateBlockDict(block);
                if (blockData != null) {
                    this.infoDict[block] = blockData;
                } else {
                    return string.Empty;
                }
            }
            uint key = this.Vector2ToUint(coordinate);
            if (!this.infoDict[block].ContainsKey(key)) {
                throw new PONullException();
            }
            uint info = this.infoDict[block][key];
            int mapSN = (int)info >> 24;
            //int zoneSN = (int)((info >> 16) & 255);
            //string mapSNLocal = NPCCityConf.GetMapSNLocalName(mapSN);
            //return string.Concat(mapSNLocal);
            return NPCCityConf.GetMapSNLocalName(mapSN);
        }

        public string GetTileZone(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                var blockData = this.CreateBlockDict(block);
                if (blockData != null) {
                    this.infoDict[block] = blockData;
                } else {
                    return string.Empty;
                }
            }
            uint key = this.Vector2ToUint(coordinate);
            if (!this.infoDict[block].ContainsKey(key)) {
                throw new PONullException();
            }

            uint info = this.infoDict[block][key];
            int mapSN = (int)info >> 24;
            int zoneSN = (int)((info >> 16) & 255);
            if (mapSN == 0) {
                return string.Empty;
            }
            string mapSNLocal = NPCCityConf.GetMapSNLocalName(mapSN);
            string zoneLocal = NPCCityConf.GetZoneSnLocalName(mapSN, zoneSN);
            return string.Concat(mapSNLocal, ",", zoneLocal);
        }

        private uint Vector2ToUint(Vector2 val) {
            return (uint)val.x << 16 | (uint)val.y;
        }

        private static uint Vector2ToUint(float x, float y) {
            return (uint)x << 16 | (uint)y;
        }

        public string GetLayerAbove(Vector2 coordinate,
            bool getLayerAboveLevel = false) {
            Vector2 block = this.GetBlock(coordinate);
            uint key = this.Vector2ToUint(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                Debug.LogError("Block and cooridinate map Error!");
                return string.Empty;
            }
            if (!this.infoDict[block].ContainsKey(key)) {
                Debug.LogError("Vector2ToUint map Error!");
                return string.Empty;
            }
            uint tileInfo = this.infoDict[block][key];
            int type = (int)((tileInfo >> 8) & 255);
            string level = (tileInfo & 255).ToString();
            if (getLayerAboveLevel)
                return level;
            switch ((ElementType)type) {
                case ElementType.mountain:
                    return !level.CustomEquals("0") ?
                        string.Concat(ElementName.mountain, level) : string.Empty;
                case ElementType.camp:
                    return string.Concat("forest_", level);
                case ElementType.plain:
                case ElementType.road:
                case ElementType.npc_city:
                case ElementType.river:
                    return string.Empty;
                case ElementType.pass:
                    return this.GetPassBase(tileInfo, coordinate);
                default:
                    return string.Concat((ElementType)type, level);
            }
        }

        public string GetLayerLevel(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            uint key = this.Vector2ToUint(coordinate);
            uint tileInfo = this.infoDict[block][key];
            int type = (int)((tileInfo >> 8) & 255);
            int level = (int)(tileInfo & 255);
            MapBasicTypeConf typeConf =
                MapBasicTypeConf.GetConf(((tileInfo >> 8) & 255).ToString());
            return ((level - 1) / 2 + 1).ToString();
        }

        public string GetLayerBase(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            uint key = this.Vector2ToUint(coordinate);
            uint tileInfo = this.infoDict[block][key];
            int type = (int)((tileInfo >> 8) & 255);
            if ((ElementType)type == ElementType.road) {
                return string.Concat("road_",
                    this.GetRoadBase(coordinate, ElementType.road, (int)(tileInfo) & 255));
            } else if ((ElementType)type == ElementType.npc_city) {
                int level = (int)(tileInfo & 255);
                if (level < GameConst.CITY_CENTER) {
                    if (level >= GameConst.CITY_ENTRANCE_EW) {
                        return "road_ew_1";
                    } else if (level >= GameConst.CITY_ENTRANCE_NS) {
                        return "road_ns_1";
                    }
                }
                return string.Empty;
            } else {
                return string.Empty;
            }
        }

        public string GetLayerBelow(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            uint key = this.Vector2ToUint(coordinate);
            if (!this.infoDict.ContainsKey(block) || !this.infoDict[block].ContainsKey(key)) {
                Debug.LogError("Not contains this coordinate: " + coordinate);
                return string.Empty;
            }
            uint tileInfo = this.infoDict[block][key];
            int type = (int)((tileInfo >> 8) & 255);
            if ((ElementType)type == ElementType.river) {
                return string.Concat("river_",
                    this.GetRoadBase(coordinate, ElementType.river));
            } else {
                return string.Empty;
            }

        }

        public ElementType GetTileType(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            uint key = this.Vector2ToUint(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                var blockData = this.CreateBlockDict(block);
                if (blockData == null) {
                    if (this.IsBorder(coordinate)) {
                        return ElementType.border;
                    }
                    return ElementType.none;
                } else {
                    this.infoDict[block] = blockData;
                }
            }
            if (!this.infoDict[block].ContainsKey(key)) {
                if (this.IsBorder(coordinate)) {
                    return ElementType.border;
                }
                return ElementType.none;
            }
            uint tileInfo = this.infoDict[block][key];
            int type = (int)((tileInfo >> 8) & 255);
            return (ElementType)type;
        }

        public int GetLevel(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            uint key = this.Vector2ToUint(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                var blockData = this.CreateBlockDict(block);
                if (blockData != null) {
                    this.infoDict[block] = blockData;
                } else {
                    return -1;
                }
            }
            if (!this.infoDict[block].ContainsKey(key)) {
                Debug.LogError("Block and cooridinate map Error!");
                return -1;
            }

            uint tileInfo = this.infoDict[block][key];
            int type = (int)((tileInfo >> 8) & 255);
            switch ((ElementType)type) {
                case ElementType.npc_city:
                    string citykey = NPCCityConf.GetCityKey(tileInfo);
                    return NPCCityConf.GetConf(citykey).level;
                case ElementType.pass:
                    string passKey = MiniMapPassConf.GetPassKey(tileInfo, coordinate);
                    return MiniMapPassConf.GetConf(passKey).level;
                case ElementType.mountain:
                case ElementType.river:
                case ElementType.camp:
                    return -1;
                case ElementType.road:
                    return 1;
                default:
                    return (int)(tileInfo & 255);
            }
        }

        public int GetTileArmy(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            uint key = this.Vector2ToUint(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return 0;
            }
            if (!this.infoDict[block].ContainsKey(key)) {
                return 0;
            }
            int level = (int)(this.infoDict[block][key] & 255);
            ResourceTroopConf troopConf = ResourceTroopConf.GetConf(level.ToString());
            return troopConf != null ? troopConf.army : 0;
        }

        public bool GetTileInfo(MapTileInfo tileInfo) {
            Vector2 block = this.GetBlock(tileInfo.coordinate);
            uint key = this.Vector2ToUint(tileInfo.coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return false;
            }
            if (!this.infoDict[block].ContainsKey(key)) {
                return false;
            }
            if (tileInfo.endurance == int.MaxValue) {
                tileInfo.endurance = GameConst.MAX_ENDURANCE;
            }
            tileInfo.maxEndurance = GameConst.MAX_ENDURANCE;
            uint tile = this.infoDict[block][key];
            string id = (tile >> 8 & 255).ToString();
            int level = (int)(tile & 255);
            MapBasicTypeConf typeConf = ConfigureManager.GetConfById<MapBasicTypeConf>(id);
            switch (typeConf.category) {
                case "resource":
                    if (typeConf.type == ElementName.road) {
                        level = 1;
                    }
                    MapResourceProductionConf resConf = MapResourceProductionConf.GetConf(
                            string.Concat(typeConf.type, level));
                    tileInfo.name = resConf.type;
                    tileInfo.level = level;
                    //foreach (var pair in resConf.productionDict) {
                    //    tileInfo.infoDict.Add(
                    //        pair.Key.ToString().ToLower(),
                    //        pair.Value.ToString()
                    //    );
                    //}
                    tileInfo.defender = level;
                    tileInfo.type = ElementCategory.resource;
                    break;
                case "npc_city":
                    string cityKey = NPCCityConf.GetCityKey(tile);
                    NPCCityConf cityConf = NPCCityConf.GetConf(cityKey);
                    tileInfo.city = cityConf;
                    tileInfo.name = cityConf.name;
                    tileInfo.type = ElementCategory.npc_city;
                    tileInfo.level = cityConf.level;
                    tileInfo.defender = cityConf.level;
                    tileInfo.endurance = cityConf.durability;
                    tileInfo.maxEndurance = cityConf.durability;
                    break;
                case "pass":
                    tileInfo.name = ElementCategory.pass;
                    tileInfo.type = ElementCategory.pass;
                    tileInfo.defender = level;
                    string passKey = (tile >> 24) + "," + ((tile >> 16) & 255) + ","
                        + (int)tileInfo.coordinate.x + "," + (int)tileInfo.coordinate.y;
                    tileInfo.pass = MiniMapPassConf.GetConf(passKey);
                    tileInfo.level = tileInfo.pass.level;
                    tileInfo.endurance =
                        PassConf.GetPassDurability(tileInfo.pass.level, tileInfo.GetPassType());
                    tileInfo.maxEndurance = tileInfo.endurance;
                    break;
                case "camp":
                    tileInfo.name =
                    tileInfo.type = ElementCategory.forest;
                    break;
                default:
                    tileInfo.name = typeConf.type;
                    tileInfo.type = typeConf.type;
                    break;
            }
            return true;
        }

        public List<Vector2> GetBlockList(Vector2 center) {
            List<Vector2> blockList = new List<Vector2>();
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    Vector2 block = new Vector2(i, j);
                    blockList.Add(block);
                }
            }
            blockList.Sort(Comparer);
            int blockListCount = blockList.Count;
            for (int i = 0; i < blockListCount; i++) {
                blockList[i] += center;
            }
            return blockList;
        }

        public bool IsCoordinateInBlocks(Vector2 coordinate) {
            Vector2 block = this.GetBlock(coordinate);
            uint key = this.Vector2ToUint(coordinate);
            if (!this.infoDict.ContainsKey(block)) {
                return false;
            }
            if (!this.infoDict[block].ContainsKey(key)) {
                return false;
            }
            return true;
        }

        // To do: Check the city border and load city, that can make sight smaller.
        public string GetLayerCityBase(Vector2 coordinate, ref Vector2 offset) {
            Vector2 block = this.GetBlock(coordinate);
            uint key = this.Vector2ToUint(coordinate);
            uint tileInfo = this.infoDict[block][key];
            int isCenter = (int)(tileInfo & 255);
            if (isCenter >= GameConst.CITY_CENTER) {
                string cityKey = NPCCityConf.GetCityKey(tileInfo);
                NPCCityConf cityConf = NPCCityConf.GetConf(cityKey);
                offset = Vector2.zero;
                return string.Format("city_{0}_{1}", cityConf.race, cityConf.size);
            } else {
                string cityKey = NPCCityConf.GetCityKey(tileInfo + GameConst.CITY_CENTER);
                MiniMapCityConf miniCityConf = MiniMapCityConf.GetConf(cityKey);
                offset = miniCityConf.coordinate - coordinate;
                return string.Empty;
            }
        }

        public Vector2 GetCityCenterCoord(Vector2 coordinate) {
            uint key = this.Vector2ToUint(coordinate);
            Vector2 block = this.GetBlock(coordinate);
            uint tileInfo = this.infoDict[block][key];
            string cityKey = NPCCityConf.GetCityKey(tileInfo);
            string id = cityKey.Replace(",0", ",1");
            int isCenter = (int)(tileInfo & 255);
            return (isCenter < GameConst.CITY_CENTER) ?
                MiniMapCityConf.GetConf(id).coordinate : Vector2.zero;
        }

        private string GetRoadBase(Vector2 center, ElementType roadType, int level = 0) {
            int result = 0;
            foreach (var pair in this.offsetDict) {
                Vector2 coordinate = center + pair.Key;
                Vector2 block = this.GetBlock(coordinate);
                uint key = this.Vector2ToUint(coordinate);
                if (!this.infoDict.ContainsKey(block) || !this.infoDict[block].ContainsKey(key)) {
                    continue;
                }
                uint tile = this.infoDict[block][key];
                //int cityLevel = (int)(tile & 255);
                int type = (int)((tile >> 8) & 255);
                if (type == (int)roadType || type == (int)ElementType.pass ||
                    (type == (int)ElementType.npc_city && (int)(tile & 255) >= GameConst.CITY_ENTRANCE_NS)) {
                    result += pair.Value;
                }
            }
            foreach (var pair in this.directionDict) {
                if ((pair.Key & result) == pair.Key) {
                    if ((pair.Value.CustomEquals("ns") ||
                        pair.Value.CustomEquals("ew")) && level != 0) {
                        return string.Concat(pair.Value, "_", level);
                    }
                    return pair.Value;
                }
            }
            return string.Empty;
        }

        private string GetPassBase(uint tileInfo, Vector2 coordinate) {
            string key = string.Concat((tileInfo >> 24), ",",
                                        (tileInfo >> 16 & 255), ",",
                                        coordinate.x, ",", coordinate.y);
            MiniMapPassConf passConf = MiniMapPassConf.GetConf(key);
            Vector2 leftPoint = coordinate + Vector2.right;
            Vector2 block = this.GetBlock(leftPoint);
            uint leftKey = this.Vector2ToUint(leftPoint);
            string direction;
            if (!this.IsCoordinateInBlocks(leftPoint)) {
                direction = "ns";
            } else {
                uint tile = this.infoDict[block][leftKey];
                int type = (int)((tile >> 8) & 255);
                if (type == (int)ElementType.mountain || type == (int)ElementType.river) {
                    direction = "we";
                } else {
                    direction = "ns";
                }
            }
            return string.Concat(
                MiniMapPassConf.GetMainPassList().Contains(passConf) ? "pass_" : "bridge_",
                direction);
        }

        private bool IsBorder(Vector2 coordinate) {
            return this.borderRect.Contains(coordinate);
        }

        public string GetBorderType(Vector2 coordinate) {
            if (coordinate == this.CornerA) {
                return "border_sw";
            } else if (coordinate == this.CornerB) {
                return "border_nw";
            } else if (coordinate == this.CornerC) {
                return "border_ne";
            } else if (coordinate == this.CornerD) {
                return "border_es";
            } else if (coordinate.x == this.CornerA.x &&
                 coordinate.y > this.CornerA.y && coordinate.y < this.CornerB.y) {
                return "border_w";
            } else if (coordinate.x == this.CornerC.x &&
                coordinate.y > this.CornerD.y && coordinate.y < this.CornerC.y) {
                return "border_e";
            } else if (coordinate.y == this.CornerA.y &&
                coordinate.x > this.CornerA.x && coordinate.x < this.CornerD.x) {
                return "border_s";
            } else if (coordinate.y == this.CornerB.y &&
                coordinate.x > this.CornerB.x && coordinate.x < this.CornerC.x) {
                return "border_n";
            }
            return string.Empty;

        }

        private bool CheckCoordinate(Vector2 coordinate) {
            if (coordinate.x > this.maxCoordinate.x || coordinate.y > this.maxCoordinate.y ||
                coordinate.x < this.minCoordinate.x || coordinate.y < this.minCoordinate.y) {
                return false;
            }
            return true;
        }

        private bool CheckBlock(Vector2 block) {
            if (block.x > this.maxBlock.x || block.x < this.minBlock.x ||
                block.y > this.maxBlock.y || block.y < this.minBlock.y) {
                return false;
            }
            return true;
        }

        private int Comparer(Vector2 a, Vector2 b) {
            if (b.x.Abs() > 1 || b.y.Abs() > 1) {
                return -1;
            } else {
                return 1;
            }
        }

    }
}
