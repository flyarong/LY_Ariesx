using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System.Text.RegularExpressions;

namespace Poukoute {
    public class ExploreConf : BaseConf {
        public string level;
        public int exp;
        public int food;
        public int lumber;
        public int marble;
        public int steel;

        public Dictionary<Resource, int> resourceDict = new Dictionary<Resource, int>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = attrDict["level"];
            this.exp = int.Parse(attrDict["exp"]);
            this.food = int.Parse(attrDict["food"]);
            this.lumber = int.Parse(attrDict["lumber"]);
            this.marble = int.Parse(attrDict["marble"]);
            this.steel = int.Parse(attrDict["steel"]);

            resourceDict.Add(Resource.Lumber, this.lumber);
            resourceDict.Add(Resource.Steel, this.steel);
            resourceDict.Add(Resource.Marble, this.marble);
            resourceDict.Add(Resource.Food, this.food);
        }

        public override string GetId() {
            return this.level;
        }

        static ExploreConf() {
            ConfigureManager.Instance.LoadConfigure<ExploreConf>();
        }

        public static ExploreConf GetConf(string id) {
            return ConfigureManager.GetConfById<ExploreConf>(id);
        }
    }
}
