using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class HeroBattleConf : BaseConf {
        public string name;
        public string model;
        public int type; // 0 normal, 1 campaign.
#if UNITY_EDITOR
        public static Dictionary<string, string> modelDict = new Dictionary<string, string>();
        public static Dictionary<string, string> heroDict = new Dictionary<string, string>();
#endif
        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.name = attrDict["name"];
            this.model = attrDict["path"];
#if UNITY_EDITOR
            heroDict[this.name] = this.model;
            if (modelDict.ContainsKey(this.model)) {
                this.model = modelDict[this.model];
            } else {
                modelDict[this.model] = this.name;
                this.model = this.name;
            }
#endif
            this.type = int.Parse(attrDict["type"]);
        }

        public override string GetId() {
            return this.name;
        }

        static HeroBattleConf() {
            ConfigureManager.Instance.LoadConfigure<HeroBattleConf>();
        }

        public static string GetModelPath(string name) {
            HeroBattleConf heroBattleConf = GetConf(name);
            return (heroBattleConf != null) ? heroBattleConf.model : string.Empty;
        }

        public static bool IsCampaignHero(string heroId) {
            return ConfigureManager.GetConfById<HeroBattleConf>(heroId).type == 1;
        }

        public static HeroBattleConf GetConf(string id) {
            return ConfigureManager.GetConfById<HeroBattleConf>(id);
        }
    }
}
