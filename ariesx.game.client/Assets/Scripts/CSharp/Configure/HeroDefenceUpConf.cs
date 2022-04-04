using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class HeroDefenceUpConf : BaseConf {
        public int level;
        public int defence_up;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.defence_up = int.Parse(attrDict["defence_up"]);
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static HeroDefenceUpConf() {
            ConfigureManager.Instance.LoadConfigure<HeroDefenceUpConf>();
        }

        public static HeroDefenceUpConf GetConf(string id) {
            return ConfigureManager.GetConfById<HeroDefenceUpConf>(id);
        }
    }
}