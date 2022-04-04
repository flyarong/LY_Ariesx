using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class LocalConf : BaseConf {
        public string key;
        public string value;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.key = attrDict["KEY"];
            this.value = attrDict["CN"];
        }

        public override string GetId() {
            return this.key;
        }

        static LocalConf() {
            ConfigureManager.Instance.LoadConfigure<LocalConf>();

        }
    }


}
