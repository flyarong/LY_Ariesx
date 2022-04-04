using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class MapResourceProductionConf : BaseConf {
        public string type;
        public Dictionary<Resource, int> productionDict =
            new Dictionary<Resource, int>(6);
        public int level;


        // Need a [type, struct] dictionary?
        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.type = attrDict["type"];
            this.level = int.Parse(attrDict["level"]);
            this.Parse(Resource.Lumber, attrDict["lumber"]);
            this.Parse(Resource.Steel, attrDict["steel"]);
            this.Parse(Resource.Marble, attrDict["marble"]);
            this.Parse(Resource.Food, attrDict["food"]);
            this.Parse(Resource.Gold, attrDict["gold"]);
            this.Parse(Resource.Crystal, attrDict["crystal"]);
        }

        public override string GetId() {
            return string.Concat(this.type, this.level);
        }

        static MapResourceProductionConf() {
            ConfigureManager.Instance.LoadConfigure<MapResourceProductionConf>();

        }

        private void Parse(Resource resource, string production) {
            if (!production.CustomEquals("0")) {
                this.productionDict.Add(resource, int.Parse(production));
            }
        }

        public static string GetProduction(int value) {
            return string.Concat("+", GameHelper.GetFormatNum(value),
                "/", LocalManager.GetValue(LocalHashConst.time_hour));
        }

        public static MapResourceProductionConf GetConf(string id) {
            return ConfigureManager.GetConfById<MapResourceProductionConf>(id);
        }

        public static string GetTribute(int value) {
            return string.Concat("+", GameHelper.GetFormatNum(value),
                "/", LocalManager.GetValue(LocalHashConst.tribute));
        }
    }
}
