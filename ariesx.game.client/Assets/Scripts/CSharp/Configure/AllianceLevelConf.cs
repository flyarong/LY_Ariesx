using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System.Text.RegularExpressions;

namespace Poukoute {
    public class AllianceLevelConf : BaseConf {
        public int level;
        public int exp;
        public int maxMember;
        public float lumberbuff;
        public float marblebuff;
        public float steelbuff;
        public float foodbuff;
        private static int AllianceMaxLevel = 0;
        //private static int AllianceMaxMembers = 0;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.exp = int.Parse(attrDict["exp"]);
            this.maxMember = int.Parse(attrDict["max_member"]);
            this.lumberbuff = float.Parse(attrDict["lumber_buff"]);
            this.marblebuff = float.Parse(attrDict["marble_buff"]);
            this.steelbuff = float.Parse(attrDict["steel_buff"]);
            this.foodbuff = float.Parse(attrDict["food_buff"]);
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static AllianceLevelConf() {
            ConfigureManager.Instance.LoadConfigure<AllianceLevelConf>();
        }

        public static int GetAllianceLevelByExp(int exp) {
            int allianceMaxLevel = AllianceLevelConf.GetAllianceMaxLevel();
            for (int i = 2; i < allianceMaxLevel + 1; i++) {
                AllianceLevelConf allianceLevelConf = AllianceLevelConf.GetConf(i.ToString());
                if (exp <= allianceLevelConf.exp) {
                    return i - 1;
                }
            }
            return 0;
        }

        public static int GetAllianceUpgradeExpByLevel(int curLevel) {
            AllianceLevelConf allianceLevelConf =
                AllianceLevelConf.GetConf((curLevel + 1).ToString());
            return allianceLevelConf.exp;
        }

        public static int GetAllianceMaxLevel() {
            if (AllianceMaxLevel == 0) {
                foreach (AllianceLevelConf allianceLevelConf in
                        ConfigureManager.GetConfDict<AllianceLevelConf>().Values) {
                    if (AllianceMaxLevel < allianceLevelConf.level) {
                        AllianceMaxLevel = allianceLevelConf.level;
                    }
                }
            }
            return AllianceMaxLevel;
        }

        public static int GetAllianceMaxMembers() {
            if (AllianceMaxLevel == 0) {
                GetAllianceMaxLevel();
            }
            return AllianceLevelConf.GetConf(AllianceMaxLevel.ToString()).maxMember;
        }

        public static AllianceLevelConf GetConf(string id) {
            return ConfigureManager.GetConfById<AllianceLevelConf>(id);
        }
    }
}
