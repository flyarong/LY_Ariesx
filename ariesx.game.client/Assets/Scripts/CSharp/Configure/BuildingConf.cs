using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System.Text.RegularExpressions;

namespace Poukoute {
    public class BuildingConf : BaseConf {
        public string type;
        public string buildingName;
        public int buildingLevel;
        public int fieldLevel;
        public long duration;
        public int durability;
        public string unlockCondition;
        public string unlockShow;

        public Dictionary<Resource, int> resourceDict = new Dictionary<Resource, int>(3);
        public static Dictionary<string, int> buildingCountDict = new Dictionary<string, int>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.type = attrDict["type"];
            this.buildingName = attrDict["name"];
            this.buildingLevel = int.Parse(attrDict["level"]);
            this.fieldLevel = int.Parse(attrDict["fieldlevel"]);
            this.unlockCondition = attrDict["unlock_condition"];
            this.unlockShow = attrDict["unlock_show"];
            this.duration = attrDict["duration"].CustomIsEmpty() ?
                                                0 : long.Parse(attrDict["duration"]);
            this.durability = int.Parse(attrDict["durability"]);
            if (this.buildingLevel == 1) {
                if (!buildingCountDict.ContainsKey(this.type)) {
                    buildingCountDict.Add(this.type, 1);
                } else {
                    buildingCountDict[this.type]++;
                }
            }

            if (!attrDict["lumber"].CustomIsEmpty() && !attrDict["lumber"].CustomEquals("0")) {
                resourceDict.Add(Resource.Lumber, int.Parse(attrDict["lumber"]));
            }
            if (!attrDict["marble"].CustomIsEmpty() && !attrDict["marble"].CustomEquals("0")) {
                resourceDict.Add(Resource.Marble, int.Parse(attrDict["marble"]));
            }
            if (!attrDict["steel"].CustomIsEmpty() && !attrDict["steel"].CustomEquals("0")) {
                resourceDict.Add(Resource.Steel, int.Parse(attrDict["steel"]));
            }
        }

        public override string GetId() {
            return this.buildingName + "_" + this.buildingLevel;
        }

        static BuildingConf() {
            ConfigureManager.Instance.LoadConfigure<BuildingConf>();
        }

        public static int GetDurability(string key) {
            BuildingConf buildingConf = BuildingConf.GetConf(key);
            BuildModel model = ModelManager.GetModelData<BuildModel>();
            if (model.GetDurabilityBonus() != 0) {
                return Mathf.RoundToInt(buildingConf.durability * (1 + model.GetDurabilityBonus()));
            }
            return buildingConf.durability;
        }

        public static BuildingConf GetConf(string id) {
            return ConfigureManager.GetConfById<BuildingConf>(id);
        }

        public static int GetBuildMaxLevelByName(string buildingName) {
            int buildingLevel = 0;
            List<BaseConf> buildingConfs =
                new List<BaseConf>(ConfigureManager.GetConfDict<BuildingConf>().Values);
            foreach (BuildingConf buildingConf in buildingConfs) {
                if (buildingName.CustomEquals(buildingConf.buildingName) &&
                    buildingLevel < buildingConf.buildingLevel) {
                    buildingLevel = buildingConf.buildingLevel;
                }
            }
            return buildingLevel;
        }

        public static string GetBuildType(string id) {
            return BuildingConf.GetConf(id).type;
        }
    }
}
