using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {

    public class MonthCardConf : BaseConf {
        public string id;
        public string directGemAmount;
        public string dailyGemAmount;
        public string chestFreeAmount;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["month_card_id"];
            this.directGemAmount = attrDict["direct_gem_amount"];
            this.dailyGemAmount = attrDict["daily_gem_amount"];
            this.chestFreeAmount = attrDict["chest_amount"];
        }

        public override string GetId() {
            return this.id;
        }

        static MonthCardConf() {
            ConfigureManager.Instance.LoadConfigure<MonthCardConf>();
        }

        public static MonthCardConf GetConf(string id) {
            return ConfigureManager.GetConfById<MonthCardConf>(id);
        }
    }
}