using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System.Text.RegularExpressions;
using System.Text;

namespace Poukoute {
    public class DominantUpConf : BaseConf {
        public string name;
        public int level;
        public string type;
        public float bonus;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.name = attrDict["name"];
            this.level = int.Parse(attrDict["level"]);
            this.type = attrDict["army_type"];
            this.bonus = float.Parse(attrDict["dominant_up_percent"]);
        }

        public override string GetId() {
            return this.name + this.level;
        }

        static DominantUpConf() {
            ConfigureManager.Instance.LoadConfigure<DominantUpConf>();
        }

        public static DominantUpConf GetConf(string name, int level) {
            return ConfigureManager.GetConfById<DominantUpConf>(name + level);
        }

        public string GetDominantType() {
            switch (this.type) {
                case "scissors":
                    return "paper";
                case "paper":
                    return "rock";
                case "rock":
                    return "scissors";
                default:
                    return string.Empty;
            }

        }
    }
}
