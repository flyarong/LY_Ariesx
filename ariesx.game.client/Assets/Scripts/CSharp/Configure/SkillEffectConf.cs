using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class SkillEffectConf : BaseConf {
        public string name;
        public string hit;
        public string last;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.name = attrDict["name"];
            this.hit = attrDict["drop_effect"];
            this.last = attrDict["hit_effect"];
        }

        public override string GetId() {
            return name;
        }

        static SkillEffectConf() {
            ConfigureManager.Instance.LoadConfigure<SkillEffectConf>();
        }

        public static SkillEffectConf GetConf(string id) {
            return ConfigureManager.GetConfById<SkillEffectConf>(id);
        }
    }
}
