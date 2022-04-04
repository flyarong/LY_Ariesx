using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class HeroAttackUpConf : BaseConf {
        public int level;
        public int attack_up;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.attack_up = int.Parse(attrDict["attack_up"]);
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static HeroAttackUpConf() {
            ConfigureManager.Instance.LoadConfigure<HeroAttackUpConf>();
        }

        public static HeroAttackUpConf GetConf(string id) {
            return ConfigureManager.GetConfById<HeroAttackUpConf>(id);
        }
    }
}