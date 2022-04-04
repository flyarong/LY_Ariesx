using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class ResourceBasicTypeConf : BaseConf {
        public string type;
        public int flag;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.type = attrDict["resource"];
            this.flag = int.Parse(attrDict["flag"]);
        }

        public override string GetId() {
            return this.flag.ToString();
        }

        static ResourceBasicTypeConf() {
            ConfigureManager.Instance.LoadConfigure<ResourceBasicTypeConf>();
        }
    }
}
