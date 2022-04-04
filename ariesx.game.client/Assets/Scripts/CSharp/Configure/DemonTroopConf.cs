using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class DemonTroopConf : BaseConf {
        public string level;
        public string point;
        public string type;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = attrDict["level"];
            this.point = attrDict["point"];
            this.type = attrDict["type"];
        }

        public override string GetId() {
            return level + type;
        }

        static DemonTroopConf() {
            ConfigureManager.Instance.LoadConfigure<DemonTroopConf>();
        }

        public static DemonTroopConf GetConf(string id) {
            return ConfigureManager.GetConfById<DemonTroopConf>(id);
        }
    }
}
