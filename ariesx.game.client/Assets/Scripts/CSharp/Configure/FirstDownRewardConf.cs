using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class FirstDownRewardConf : BaseConf {
        public int level;
        public int gem;
        public string unlockChest;
        public Dictionary<Resource, int> resourceDict = new Dictionary<Resource, int>(6);

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.gem = int.Parse(attrDict["gem"]);
            this.unlockChest = attrDict["unlock_chest"];

            if (!attrDict["lumber"].CustomIsEmpty() && !attrDict["lumber"].CustomEquals("0")) {
                this.resourceDict.Add(Resource.Lumber, int.Parse(attrDict["lumber"]));
            }
            if (!attrDict["steel"].CustomIsEmpty() && !attrDict["steel"].CustomEquals("0")) {
                this.resourceDict.Add(Resource.Steel, int.Parse(attrDict["steel"]));
            }
            if (!attrDict["marble"].CustomIsEmpty() && !attrDict["marble"].CustomEquals("0")) {
                this.resourceDict.Add(Resource.Marble, int.Parse(attrDict["marble"]));
            }
            if (!attrDict["food"].CustomIsEmpty() && !attrDict["food"].CustomEquals("0")) {
                this.resourceDict.Add(Resource.Food, int.Parse(attrDict["food"]));
            }
            if (!attrDict["gold"].CustomIsEmpty() && !attrDict["gold"].CustomEquals("0")) {
                this.resourceDict.Add(Resource.Gold, int.Parse(attrDict["gold"]));
            }
            if (!attrDict["crystal"].CustomIsEmpty() && !attrDict["crystal"].CustomEquals("0")) {
                this.resourceDict.Add(Resource.Crystal, int.Parse(attrDict["crystal"]));
            }
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static FirstDownRewardConf() {
            ConfigureManager.Instance.LoadConfigure<FirstDownRewardConf>();
        }

        public static FirstDownRewardConf GetConf(string id) {
            return ConfigureManager.GetConfById<FirstDownRewardConf>(id);
        }
    }
}
