using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class DailyRewardConf : BaseConf {
        public int level;
        public int chest;
        public int gold;
        public int food;
        public int lumber;
        public int marble;
        public int steel;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.chest = int.Parse(attrDict["chest"]);
            this.gold = int.Parse(attrDict["gold"]);
            this.food = int.Parse(attrDict["food"]);
            this.lumber = int.Parse(attrDict["lumber"]);
            this.marble = int.Parse(attrDict["marble"]);
            this.steel = int.Parse(attrDict["steel"]);
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static DailyRewardConf() {
            ConfigureManager.Instance.LoadConfigure<DailyRewardConf>();
        }

        public static DailyLimit GetNewDailyLimit(string level) {
            DailyRewardConf dailyRewardConf = DailyRewardConf.GetConf(level);
            DailyLimit dailyLimit = new DailyLimit();
            Protocol.Resources resourceCurrent = new Protocol.Resources();
            Protocol.Resources resourceLimit = new Protocol.Resources();
            dailyLimit.ChestCurrent = 0;
            dailyLimit.GoldCurrent = 0;
            resourceCurrent.Food = 0;
            resourceCurrent.Lumber = 0;
            resourceCurrent.Marble = 0;
            resourceCurrent.Steel = 0;
            dailyLimit.ChestLimit = dailyRewardConf.chest;
            dailyLimit.GoldLimit = dailyRewardConf.gold;
            resourceLimit.Food = dailyRewardConf.food;
            resourceLimit.Lumber = dailyRewardConf.lumber;
            resourceLimit.Marble = dailyRewardConf.marble;
            resourceLimit.Steel = dailyRewardConf.steel;
            dailyLimit.ResourceLimit = resourceLimit;
            dailyLimit.ResourceCurrent = resourceCurrent;
            return dailyLimit;
        }

        public static DailyRewardConf GetConf(string id) {
            return ConfigureManager.GetConfById<DailyRewardConf>(id);
        }
    }
}
