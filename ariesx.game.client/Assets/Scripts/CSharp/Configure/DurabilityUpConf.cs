using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class DurabilityUpConf : BaseConf {
        public int level;
        public float defence_up;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.defence_up = float.Parse(attrDict["defence_up"]);
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static DurabilityUpConf() {
            ConfigureManager.Instance.LoadConfigure<DurabilityUpConf>();
        }

        public static DurabilityUpConf GetConf(string id) {
            return ConfigureManager.GetConfById<DurabilityUpConf>(id);
        }
    }
}