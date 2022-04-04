using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class HeroSkillDefaultConf : BaseConf {
        public string id;
        public List<string> attrList = new List<string>();
        public int hp;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["default_skill"];
            for (int i = 0; i < 8; i++) {
                attrList.Add(attrDict["{" + i + "}"]);
            }
        }

        public override string GetId() {
            return this.id;
        }

        static HeroSkillDefaultConf() {
            ConfigureManager.Instance.LoadConfigure<HeroSkillDefaultConf>();
        }

        public static HeroSkillDefaultConf GetConf(string heroId, int index = 1) {
            return ConfigureManager.GetConfById<HeroSkillDefaultConf>(
                string.Format("default_{0}_skill_{1}", heroId, index)
            );
        }
    }
}
