using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class PassConf : BaseConf {
        public string level;
        public string type;
        public int durability;
        public int force;
        public int allianceExp;
        public Dictionary<Resource, int> rewardDict = new Dictionary<Resource, int>();


        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = attrDict["level"];
            this.type = attrDict["type"];
            this.durability = int.Parse(attrDict["durability"]);
            this.force = int.Parse(attrDict["force"]);
            this.allianceExp = int.Parse(attrDict["alliance_exp"]);

            this.ParseResource(Resource.Lumber, int.Parse(attrDict["lumber"]));
            this.ParseResource(Resource.Steel, int.Parse(attrDict["steel"]));
            this.ParseResource(Resource.Marble, int.Parse(attrDict["marble"]));
            this.ParseResource(Resource.Food, int.Parse(attrDict["food"]));
            this.ParseResource(Resource.Gold, int.Parse(attrDict["gold"]));
        }

        private void ParseResource(Resource resource, int value) {
            if (value != 0) {
                rewardDict.Add(resource, value);
            }
        }

        public override string GetId() {
            return string.Concat(this.level, this.type);
        }

        static PassConf() {
            ConfigureManager.Instance.LoadConfigure<PassConf>();
        }

        public static int GetPassDurability(int level, string type) {
            string id = string.Concat(level, type);
            PassConf passConf = PassConf.GetConf(id);
            return passConf.durability;
        }

        public static PassConf GetConf(string id) {
            return ConfigureManager.GetConfById<PassConf>(id);
        }
    }
}
