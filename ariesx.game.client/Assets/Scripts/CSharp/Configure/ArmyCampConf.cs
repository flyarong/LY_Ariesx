using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System.Text.RegularExpressions;

namespace Poukoute {
    public class ArmyCampConf : BaseConf {
        public int level;
        public int heroAmount;
        public float recruitmentSpeed;
        public List<int> unlockPositionList = new List<int>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.heroAmount = int.Parse(attrDict["hero_amount"]);
            this.recruitmentSpeed = float.Parse(attrDict["recruitment_speed"]);
            string[] positionArray = attrDict["unlock"].CustomSplit(',');
            foreach (string position in positionArray) {
                unlockPositionList.Add(int.Parse(position));
            }
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static ArmyCampConf() {
            ConfigureManager.Instance.LoadConfigure<ArmyCampConf>();
        }

        public static ArmyCampConf GetConf(string id) {
            return ConfigureManager.GetConfById<ArmyCampConf>(id);
        }
    }
}
