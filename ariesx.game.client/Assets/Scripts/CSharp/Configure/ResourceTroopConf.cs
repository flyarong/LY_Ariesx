using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class ResourceTroopConf : BaseConf {
        public string level;
        public int army;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = attrDict["level"];
            this.army = 1;
            //int.Parse(attrDict["hero_groups"]) * int.Parse(attrDict["group_amount"]);
        }

        public override string GetId() {
            return this.level;
        }

        static ResourceTroopConf() {
            ConfigureManager.Instance.LoadConfigure<ResourceTroopConf>();
        }

        public static ResourceTroopConf GetConf(string id) {
            return ConfigureManager.GetConfById<ResourceTroopConf>(id);
        }
    }
}
