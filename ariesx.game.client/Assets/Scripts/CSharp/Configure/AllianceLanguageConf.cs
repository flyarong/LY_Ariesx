using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class AllianceLanguageConf : BaseConf {
        public string id;
        public string language;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["id"];
            this.language = attrDict["language"];
        }

        public override string GetId() {
            return this.id;
        }

        static AllianceLanguageConf() {
            ConfigureManager.Instance.LoadConfigure<AllianceLanguageConf>();
        }

        public static AllianceLanguageConf GetConf(string id) {
            return ConfigureManager.GetConfById<AllianceLanguageConf>(id);
        }

        public static string GetAllianceLanguage(string key) {
            if (key == "0")
                key = "1";
            AllianceLanguageConf allianceLanguageConf = AllianceLanguageConf.GetConf(key);
            return allianceLanguageConf.language;
        }
    }
}
