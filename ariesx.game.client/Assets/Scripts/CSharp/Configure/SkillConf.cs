using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class SkillConf : BaseConf {
        public string name;
        public string description;
        public string cast;
        public string[] effects;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.name = attrDict["name"];
            this.description = attrDict["description"];
            this.cast = attrDict["cast_effect"];
            this.effects = attrDict["effect"].CustomSplit(',');
        }

        public override string GetId() {
            return name;
        }

        static SkillConf() {
            ConfigureManager.Instance.LoadConfigure<SkillConf>();
        }

        public static string GetName(string name) {
            return LocalManager.GetValue(name);
        }

        public static string GetDescription(string name) {
            SkillConf skillConf = SkillConf.GetConf(name);
            string descName = string.Empty;
            if (skillConf != null) {
                descName = skillConf.description;
            }
            return LocalManager.GetValue(descName);
        }

        public static SkillConf GetConf(string id) {
            return ConfigureManager.GetConfById<SkillConf>(id);
        }
    }
}
