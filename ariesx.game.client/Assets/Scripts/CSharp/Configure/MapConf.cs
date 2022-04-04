using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class MapConf : BaseConf {
        public int type;
        public int level;
        public int production;
        public string name;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.type = int.Parse(attrDict["type"]);
            this.level = int.Parse(attrDict["level"]);
            if (!string.IsNullOrEmpty(attrDict["production"])) {
                this.production = int.Parse(attrDict["production"]);
            }
            this.name = attrDict["assets"];
        }

        public override string GetId() {
            return (((this.type << 8) + this.level) & 65535).ToString();
        }

        static MapConf() {
            ConfigureManager.Instance.LoadConfigure<MapConf>();
        }
    }
}
