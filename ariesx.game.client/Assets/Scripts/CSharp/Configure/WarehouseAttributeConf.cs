using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System.Text.RegularExpressions;

namespace Poukoute {
    public class WarehouseAttributeConf : BaseConf {
        public int level;
        public Dictionary<string, long> resourceBonus = new Dictionary<string, long>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);

            this.resourceBonus.Add("storage_lumber", long.Parse(attrDict["lumber_limit"]));
            this.resourceBonus.Add("storage_steel", long.Parse(attrDict["steel_limit"]));
            this.resourceBonus.Add("storage_marble", long.Parse(attrDict["marble_limit"]));
            this.resourceBonus.Add("storage_food", long.Parse(attrDict["food_limit"]));
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static WarehouseAttributeConf() {
            ConfigureManager.Instance.LoadConfigure<WarehouseAttributeConf>();
        }

        public static WarehouseAttributeConf GetConf(string id) {
            return ConfigureManager.GetConfById<WarehouseAttributeConf>(id);
        }
    }
}
