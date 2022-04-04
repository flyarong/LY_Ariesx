using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Poukoute {
    public class PointsLimitConf : BaseConf {
        public string type;
        public int level;
        public int amountReward;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.type = attrDict["type"];
            this.level = int.Parse(attrDict["level"]);
            this.amountReward = int.Parse(attrDict["amount_reward"]);
        }

        public override string GetId() {
            return string.Concat(this.type, this.level);
        }

        static PointsLimitConf() {
            ConfigureManager.Instance.LoadConfigure<PointsLimitConf>();
        }

        public static PointsLimitConf GetConf(string type, int level) {
            return ConfigureManager.GetConfById<PointsLimitConf>(string.Concat(type, level));
        }

    }
}