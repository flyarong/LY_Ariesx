using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System.Text.RegularExpressions;

namespace Poukoute {
    public class StrongholdRecruitConf : BaseConf {
        public int level;
        public int recruitQueue;
        public float recruitResourceNeed;
        public float recruitGoldNeed;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.recruitQueue = int.Parse(attrDict["recruit_queue"]);
            this.recruitResourceNeed = float.Parse(attrDict["recruit_resource_need"]);
            this.recruitGoldNeed = float.Parse(attrDict["recruit_gold_need"]);
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static StrongholdRecruitConf() {
            ConfigureManager.Instance.LoadConfigure<StrongholdRecruitConf>();
        }

        public static StrongholdRecruitConf GetConf(string id) {
            return ConfigureManager.GetConfById<StrongholdRecruitConf>(id);
        }
    }
}
