using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class TributeConf : BaseConf {
        public string name;
        public int force;
        public int interval;
        public int gem;
        //public Dictionary<string, int> heroDict = new Dictionary<string, int>();
        public Dictionary<Resource, int> resourceDict = new Dictionary<Resource, int>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.name = attrDict["name"];
            this.force = int.Parse(attrDict["force"]);
            this.interval = int.Parse(attrDict["interval"]);
            this.gem = int.Parse(attrDict["gem"]);
            this.resourceDict.Add(Resource.Lumber, int.Parse(attrDict["lumber"]));
            this.resourceDict.Add(Resource.Steel, int.Parse(attrDict["steel"]));
            this.resourceDict.Add(Resource.Marble, int.Parse(attrDict["marble"]));
            this.resourceDict.Add(Resource.Food, int.Parse(attrDict["food"]));
            this.resourceDict.Add(Resource.Gold, int.Parse(attrDict["gold"]));
            this.resourceDict.Add(Resource.Crystal, int.Parse(attrDict["crystal"]));
        }

        public override string GetId() {
            return this.name;
        }

        static TributeConf() {
            ConfigureManager.Instance.LoadConfigure<TributeConf>();
        }

        public static TributeConf GetConf(string id) {
            return ConfigureManager.GetConfById<TributeConf>(id);
        }
    }
}