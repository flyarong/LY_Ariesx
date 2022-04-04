using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {

    public class GoldProductConf : BaseConf {
        public string id;
        public string gemPrice;
        public string goldAmount;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["gold_id"];
            this.gemPrice = attrDict["gem_price"];
            this.goldAmount = attrDict["gold_amount"];
        }

        public override string GetId() {
            return this.id;
        }

        static GoldProductConf() {
            ConfigureManager.Instance.LoadConfigure<GoldProductConf>();
        }

        public static GoldProductConf GetConf(string id) {
            return ConfigureManager.GetConfById<GoldProductConf>(id);
        }
    }
}
