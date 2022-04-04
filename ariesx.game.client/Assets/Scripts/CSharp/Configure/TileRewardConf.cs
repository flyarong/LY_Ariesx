using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class TileRewardConf : BaseConf {
        public Dictionary<Resource, int> rewardDict = new Dictionary<Resource, int>();
        public int level;
        public string type;

        // Need a [type, struct] dictionary?
        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.type = attrDict["type"];
            this.Parse(Resource.Lumber, attrDict["lumber"]);
            this.Parse(Resource.Steel, attrDict["steel"]);
            this.Parse(Resource.Marble, attrDict["marble"]);
            this.Parse(Resource.Food, attrDict["food"]);
            this.Parse(Resource.Gold, attrDict["gold"]);
            this.Parse(Resource.Crystal, attrDict["crystal"]);
        }

        public override string GetId() {
            return this.type + this.level;
        }

        static TileRewardConf() {
            ConfigureManager.Instance.LoadConfigure<TileRewardConf>();
        }

        private void Parse(Resource resource, string production) {
            if (!production.CustomEquals("0")) {
                this.rewardDict.Add(resource, int.Parse(production));
            }
        }

        public static TileRewardConf GetConf(string id) {
            return ConfigureManager.GetConfById<TileRewardConf>(id);
        }
    }
}
