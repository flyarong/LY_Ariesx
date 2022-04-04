using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System.Text.RegularExpressions;
using System.Text;

namespace Poukoute {
    public class ResourceProduceConf : BaseConf {
        public string name;
        public int level;
        public Resource type;
        public int bonus;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.name = attrDict["name"];
            this.level = int.Parse(attrDict["level"]);
            if (attrDict["food"] != "0") {
                this.type = Resource.Food;
                this.bonus = int.Parse(attrDict["food"]);
            } else if (attrDict["lumber"] != "0") {
                this.type = Resource.Lumber;
                this.bonus = int.Parse(attrDict["lumber"]);
            } else if (attrDict["marble"] != "0") {
                this.type = Resource.Marble;
                this.bonus = int.Parse(attrDict["marble"]);
            } else {
                this.type = Resource.Steel;
                this.bonus = int.Parse(attrDict["steel"]);
            }
        }

        public override string GetId() {
            return this.name + this.level;
        }

        static ResourceProduceConf() {
            ConfigureManager.Instance.LoadConfigure<ResourceProduceConf>();
        }

        public static ResourceProduceConf GetConf(string name, int level) {
            return ConfigureManager.GetConfById<ResourceProduceConf>(name + level);
        }
    }
}
