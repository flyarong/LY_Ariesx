using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class MapBasicTypeConf : BaseConf {
        public string type;
        public string category;
        public int flag;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.type = attrDict["type"];
            this.category = attrDict["category"];
            this.flag = int.Parse(attrDict["flag"]);
        }

        public override string GetId() {
            return this.flag.ToString();
        }

        static MapBasicTypeConf() {
            ConfigureManager.Instance.LoadConfigure<MapBasicTypeConf>();
        }

        public static int GetMapBasicTypeFlag(string id) {
            return MapBasicTypeConf.GetConf(id).flag;
        }

        public static MapBasicTypeConf GetConf(string id) {
            return ConfigureManager.GetConfById<MapBasicTypeConf>(id);
        }
    }
}
