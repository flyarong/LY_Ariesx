using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class OccupyPointsConf : BaseConf {
        public int level;
        public string type;
        public string point;

        private static Dictionary<OccupyPointType, List<OccupyPointsConf>>
            occupyPointConfDict = null;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.type = attrDict["type"];
            this.point = attrDict["point"];
        }

        public override string GetId() {
            return string.Concat(this.level, this.type);
        }

        static OccupyPointsConf() {
            ConfigureManager.Instance.LoadConfigure<OccupyPointsConf>();
        }

        public static Dictionary<OccupyPointType, List<OccupyPointsConf>>
            GetOccupyPointDict() {
            if (occupyPointConfDict != null) {
                return occupyPointConfDict;
            }

            occupyPointConfDict = new Dictionary<OccupyPointType, List<OccupyPointsConf>>(3);
            Dictionary<string, BaseConf> allOccupyPointsConf =
                ConfigureManager.GetConfDict<OccupyPointsConf>();

            List<OccupyPointsConf> pointList;

            OccupyPointType pointType = OccupyPointType.None;

            foreach (OccupyPointsConf occupyPointConf in allOccupyPointsConf.Values) {
                switch (occupyPointConf.type) {
                    case "resource":
                        pointType = OccupyPointType.Resoure;
                        break;
                    case "bridge":
                        pointType = OccupyPointType.Bridge;
                        break;
                    case "npc_city":
                        pointType = OccupyPointType.NpcCity;
                        break;
                    default:
                        pointType = OccupyPointType.None;
                        break;
                }

                if (!occupyPointConfDict.TryGetValue(pointType, out pointList)) {
                    pointList = new List<OccupyPointsConf>(12);
                    occupyPointConfDict[pointType] = pointList;
                }

                pointList.TryAdd(occupyPointConf);
            }

            return occupyPointConfDict;
        }


        public static OccupyPointsConf GetConf(string id) {
            return ConfigureManager.GetConfById<OccupyPointsConf>(id);
        }
    }
}
