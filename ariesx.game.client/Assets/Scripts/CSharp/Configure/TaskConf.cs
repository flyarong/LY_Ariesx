using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class TaskConf : BaseConf {
        public string id;
        public string category;
        public string type;
        public string buildingName;
        // target
        public int level;
        public int amount;
        //public int tier;
        public int force;
        public int produce;
        public int times;

        public string localTitle;
        public string localDesc;

        public Dictionary<Resource, int> resourcesDict = new Dictionary<Resource, int>(7);
        public Dictionary<string, int> heroDict = new Dictionary<string, int>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["id"];
            this.category = attrDict["category"];
            this.type = attrDict["type"];
            this.buildingName = attrDict["name"];
            this.localTitle = attrDict["locale_title"];
            this.localDesc = attrDict["locale_desc"];

            this.level = string.IsNullOrEmpty(attrDict["level"]) ? 0 : int.Parse(attrDict["level"]);
            this.amount = string.IsNullOrEmpty(attrDict["amount"]) ? 0 : int.Parse(attrDict["amount"]);
            this.force = string.IsNullOrEmpty(attrDict["force"]) ? 0 : int.Parse(attrDict["force"]);
            this.produce = string.IsNullOrEmpty(attrDict["produce"]) ? 0 : int.Parse(attrDict["produce"]);
            this.times = string.IsNullOrEmpty(attrDict["times"]) ? 0 : int.Parse(attrDict["times"]);

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

            if (!attrDict["crystal"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["crystal"]))
                this.resourcesDict.Add(Resource.Crystal, int.Parse(attrDict["crystal"]));

            if (!attrDict["gem"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["gem"]))
                this.resourcesDict.Add(Resource.Gem, int.Parse(attrDict["gem"]));

            string heroesStr = attrDict["hero_fragments"];
            if (!string.IsNullOrEmpty(heroesStr)) {
                string[] heroArray = heroesStr.CustomSplit(';');
                foreach (string heroStr in heroArray) {
                    string[] heroAttr = heroStr.CustomSplit(',');
                    this.heroDict.Add(heroAttr[0], int.Parse(heroAttr[1]));
                }
            }
        }

        public override string GetId() {
            return this.id;
        }

        static TaskConf() {
            ConfigureManager.Instance.LoadConfigure<TaskConf>();
        }

        public int GetTarget() {
            if (type.CustomEquals("resource_level")) {
                return this.times;
            } else {
                return this.level + this.amount +
                    this.force + this.produce + this.times;
            }
        }

        public static TaskConf GetConf(string id) {
            return ConfigureManager.GetConfById<TaskConf>(id);
        }
    }
}
