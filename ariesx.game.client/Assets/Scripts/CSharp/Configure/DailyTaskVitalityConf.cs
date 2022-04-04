using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class DailyTaskVitalityConf : BaseConf {
        public string id;
        public string townhallLevel;
        public int vitality;
        public string chest;
        public Dictionary<Resource, int> resourcesDict = new Dictionary<Resource, int>(5);

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["id"];
            this.townhallLevel = attrDict["townhall_level"];
            this.vitality = int.Parse(attrDict["vitality"]);
            this.chest = attrDict["chest"];

            if (!attrDict["lumber"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["lumber"]))
                this.resourcesDict.Add(Resource.Lumber, int.Parse(attrDict["lumber"]));

            if (!attrDict["steel"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["steel"]))
                this.resourcesDict.Add(Resource.Steel, int.Parse(attrDict["steel"]));

            if (!attrDict["marble"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["marble"]))
                this.resourcesDict.Add(Resource.Marble, int.Parse(attrDict["marble"]));

            if (!attrDict["food"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["food"]))
                this.resourcesDict.Add(Resource.Food, int.Parse(attrDict["food"]));

            if (!attrDict["gold"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["gold"]))
                this.resourcesDict.Add(Resource.Gold, int.Parse(attrDict["gold"]));
        }

        public override string GetId() {
            return string.Concat(this.townhallLevel, this.id);
        }

        static DailyTaskVitalityConf() {
            ConfigureManager.Instance.LoadConfigure<DailyTaskVitalityConf>();
        }

        public static DailyTaskVitalityConf GetConf(string id) {
            return ConfigureManager.GetConfById<DailyTaskVitalityConf>(id);
        }
    }
}
