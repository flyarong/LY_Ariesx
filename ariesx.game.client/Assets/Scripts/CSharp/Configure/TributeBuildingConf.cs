using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System.Text.RegularExpressions;
using System.Text;

namespace Poukoute {
    public class TributeBuildingConf : BaseConf {
        public string name;
        public int level;
        public int gold;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.name = attrDict["name"];
            this.level = int.Parse(attrDict["level"]);
            this.gold = int.Parse(attrDict["gold"]);
        }

        public override string GetId() {
            return this.name + this.level;
        }

        static TributeBuildingConf() {
            ConfigureManager.Instance.LoadConfigure<TributeBuildingConf>();
        }

        public static TributeBuildingConf GetConf(string name, int level) {
            return ConfigureManager.GetConfById<TributeBuildingConf>(name + level);
        }
    }
}
