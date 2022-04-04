using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class MiniMapPassConf : BaseConf {
        public string id;
        //public int type;
        public Vector2 coordinate;
        private static List<MiniMapPassConf> mainPassList =
            new List<MiniMapPassConf>();
        private static Dictionary<int, List<MiniMapPassConf>> allPass =
            new Dictionary<int, List<MiniMapPassConf>>();
        public string allianceName;
        public int level = 0;
        public string LocalName {
            get {
                return LocalManager.GetValue("pass_", this.id.Replace(',', '_'));
            }
        }
        public string state;
        //public int mapState1 = -1;
        //public int mapState2 = -1;


        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["id"];
            this.coordinate = new Vector2(
                int.Parse(attrDict["x"]),
                int.Parse(attrDict["y"])
            );
            this.level = int.Parse(attrDict["level"]);
            string[] idArray = this.id.CustomSplit(',');
            int sn = int.Parse(idArray[0]);
            if (sn == 0) {
                mainPassList.Add(this);
                string[] snArray = attrDict["state"].CustomSplit(',');
                foreach (string snStr in snArray) {
                    int snInt = int.Parse(snStr);
                    if (snInt != 0) {
                        if (!allPass.ContainsKey(snInt)) {
                            allPass[snInt] = new List<MiniMapPassConf>();
                        }
                        if (!allPass[snInt].Contains(this)) {
                            allPass[snInt].Add(this);
                        }
                    }
                }
            }
            this.state = attrDict["state"];
        }

        public override string GetId() {
            return this.id;
        }

        static MiniMapPassConf() {
            ConfigureManager.Instance.LoadConfigure<MiniMapPassConf>();
        }

        public string GetPassDescription() {
            string[] idArray = this.id.CustomSplit(',');
            int sn = int.Parse(idArray[0]);
            if (sn == 0) {
                int mapState1 = -1;
                int mapState2 = -1;
                Dictionary<int, int> mapSNDict = new Dictionary<int, int>(2);
                string[] snArray = this.state.CustomSplit(',');
                foreach (string snStr in snArray) {
                    int snInt = int.Parse(snStr);
                    if (snInt != 0) {
                        mapSNDict[snInt] = 1;
                    }
                }
                List<int> mapSNList = new List<int>(mapSNDict.Keys);
                if (mapSNList.Count == 2) {
                    mapState1 = mapSNList[0];
                    mapState2 = mapSNList[1];
                }

                return string.Format(LocalManager.GetValue(LocalHashConst.map_tile_pass_intro),
                        NPCCityConf.GetMapSNLocalName(mapState1),
                        NPCCityConf.GetMapSNLocalName(mapState2));
            }
            return string.Empty;
        }

        public static List<MiniMapPassConf> GetMainPassList() {
            return mainPassList;
        }

        public static List<MiniMapPassConf> GetPassIn(int sn = 0) {
            if (allPass.ContainsKey(sn)) {
                return allPass[sn];
            } else {
                return new List<MiniMapPassConf>();
            }
        }

        public static Vector2 GetNearestPassCoord(Vector2 startCoord, int mapSN) {
            Vector2 passCoord = Vector2.zero;
            List<MiniMapPassConf> passList = MiniMapPassConf.GetPassIn(mapSN);
            int passCount = passList.Count;
            MiniMapPassConf passConf = null;
            float distance = 0;
            float mixDistance = 0;
            for (int i = 0; i < passCount; i++) {
                passConf = passList[i];
                distance = Vector2.Distance(startCoord, passConf.coordinate);
                if (mixDistance < distance) {
                    mixDistance = distance;
                    passCoord = passConf.coordinate;
                }
            }

            return passCoord;
        }

        public static MiniMapPassConf GetConf(string id) {
            return ConfigureManager.GetConfById<MiniMapPassConf>(id);
        }

        public static string GetPassKey(uint tileInfo, Vector2 coordinate) {
            return (tileInfo >> 24) + "," +
                (tileInfo >> 16 & 255) + "," + coordinate.x + "," + coordinate.y;
        }

        public static void BeforeRead() {
            mainPassList.Clear();
            allPass.Clear();
        }
    }
}
