using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class MarchSpeedUpConf : BaseConf {
        public int level;
        public float percent;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.percent = float.Parse(attrDict["march_speed_up_percent"]);
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static MarchSpeedUpConf() {
            ConfigureManager.Instance.LoadConfigure<MarchSpeedUpConf>();
        }

        public static MarchSpeedUpConf GetConf(string id) {
            return ConfigureManager.GetConfById<MarchSpeedUpConf>(id);
        }
    }
}
