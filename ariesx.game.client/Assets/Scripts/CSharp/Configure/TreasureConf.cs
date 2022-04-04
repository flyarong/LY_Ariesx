using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class TreasureConf : BaseConf {
        public string level;
        //public string freeLottery;
        public int gem;
        public Dictionary<string, int> heroDict = new Dictionary<string, int>();
        public Dictionary<Resource, int> resourceDict = new Dictionary<Resource, int>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = attrDict["level"];
            //this.freeLottery = attrDict["free_cards"];
            this.gem = int.Parse(attrDict["gem"]);
            this.resourceDict.Add(Resource.Lumber, int.Parse(attrDict["lumber"]));
            this.resourceDict.Add(Resource.Steel, int.Parse(attrDict["steel"]));
            this.resourceDict.Add(Resource.Marble, int.Parse(attrDict["marble"]));
            this.resourceDict.Add(Resource.Food, int.Parse(attrDict["food"]));
            this.resourceDict.Add(Resource.Gold, int.Parse(attrDict["gold"]));
            this.resourceDict.Add(Resource.Crystal, int.Parse(attrDict["crystal"]));
        }

        public override string GetId() {
            return this.level;
        }

        static TreasureConf() {
            ConfigureManager.Instance.LoadConfigure<TreasureConf>();
        }

        public static TreasureConf GetConf(string id) {
            return ConfigureManager.GetConfById<TreasureConf>(id);
        }
    }
}