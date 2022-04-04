using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class SiegeUpConf : BaseConf {
        public int level;
        public float bonus;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.bonus = float.Parse(attrDict["siege_up"]);
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static SiegeUpConf() {
            ConfigureManager.Instance.LoadConfigure<SiegeUpConf>();
        }

        public static SiegeUpConf GetConf(string id) {
            return ConfigureManager.GetConfById<SiegeUpConf>(id);
        }

        public string GetBonus() {
            return string.Concat(this.bonus * 100, "%");
        }
    }
}
