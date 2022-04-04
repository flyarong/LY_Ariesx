using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class HeroArmyTypeConf : BaseConf {
        public string id;
        public int attack;
        public int hp;

        public Dictionary<string, float> attributes = new Dictionary<string, float>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["army_type"];
            this.attack = int.Parse(attrDict["attack"]);
            this.hp = int.Parse(attrDict["health"]);
        }

        public override string GetId() {
            return id;
        }

        static HeroArmyTypeConf() {
            ConfigureManager.Instance.LoadConfigure<HeroArmyTypeConf>();
        }
    }
}
