using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {

    public class DailyGiftConf : BaseConf {
        public string id;
        public string GenAmount;
        public string ChestAmount;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["gift_id"];
            this.GenAmount = attrDict["gem_amount"];
            this.ChestAmount = attrDict["chest_pay_amount"];
        }

        public override string GetId() {
            return this.id;
        }

        static DailyGiftConf() {
            ConfigureManager.Instance.LoadConfigure<DailyGiftConf>();
        }

        public static DailyGiftConf GetConf(string id) {
            return ConfigureManager.GetConfById<DailyGiftConf>(id);
        }
    }
}
