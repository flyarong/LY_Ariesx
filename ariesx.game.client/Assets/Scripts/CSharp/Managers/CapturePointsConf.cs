using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class CapturePointsConf : BaseConf {
        public int level;
        public string type;
        public string point;

        private static Dictionary<CapturePointType, List<CapturePointsConf>>
            capturePointsConfDict = null;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.type = attrDict["type"];
            this.point = attrDict["point"];
        }

        public override string GetId() {
            return string.Concat(this.level, this.type);
        }

        static CapturePointsConf() {
            ConfigureManager.Instance.LoadConfigure<CapturePointsConf>();
        }

        public static Dictionary<CapturePointType, List<CapturePointsConf>>
            GetapturePointDict() {
            if (capturePointsConfDict != null) {
                return capturePointsConfDict;
            }

            capturePointsConfDict = new Dictionary<CapturePointType, List<CapturePointsConf>>(2);
            Dictionary<string, BaseConf> allOccupyPointsConf =
                ConfigureManager.GetConfDict<CapturePointsConf>();
            List<CapturePointsConf> pointList;
            CapturePointType pointType = CapturePointType.None;
            foreach (CapturePointsConf occupyPointConf in allOccupyPointsConf.Values) {
                switch (occupyPointConf.type) {
                    case "resource":
                        pointType = CapturePointType.Resoure;
                        break;
                    case "pass":
                        pointType = CapturePointType.Pass;
                        break;
                    default:
                        break;
                }

                if (!capturePointsConfDict.TryGetValue(pointType, out pointList)) {
                    pointList = new List<CapturePointsConf>(12);
                    capturePointsConfDict[pointType] = pointList;
                }
                pointList.TryAdd(occupyPointConf);
            }

            return capturePointsConfDict;
        }

        public static CapturePointsConf GetConf(string id) {
            return ConfigureManager.GetConfById<CapturePointsConf>(id);
        }

    }
}