using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {

    public class GemProductConf : BaseConf {
        public string id;
        public string basicGem;
        public string extraGem;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["product_id"];
            this.basicGem = attrDict["basic_gem"];
            this.extraGem = attrDict["extra_gem"];
        }

        public override string GetId() {
            return this.id;
        }

        static GemProductConf() {
            ConfigureManager.Instance.LoadConfigure<GemProductConf>();
        }

        public static GemProductConf GetConf(string id) {
            return ConfigureManager.GetConfById<GemProductConf>(id);
        }
    }
}