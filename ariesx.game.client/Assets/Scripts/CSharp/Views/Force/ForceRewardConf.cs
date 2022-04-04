using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class ForceRewardConf : BaseConf {
        public static Dictionary<int, ForceRewardConf> AllForceConfDict =
            new Dictionary<int, ForceRewardConf>(13);
        //public const int maxLevel = 12;
        public int level;
        public string iconPath;
        public int force;
        public string chest;
        public string chestLocal;
        public string forceLocal;
        public Dictionary<Resource, int> resourcesDict = new Dictionary<Resource, int>();
        public string gemAmount;
        public int unlockTownhallLevel;

        private int iconStart = 76;
        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["level"]);
            this.force = int.Parse(attrDict["force"]);
            this.unlockTownhallLevel = int.Parse(attrDict["unlock_townhall_level"]);
            this.chest = attrDict["chest"];
            string chestName = this.chest;
            this.chestLocal = LocalManager.GetValue(chestName);
            this.forceLocal = LocalManager.GetValue(attrDict["local_name"]);
            this.iconPath = string.Concat("Sprites/v4ui/ui_force/icn", iconStart + this.level);

            if (!attrDict["lumber"].CustomEquals("0"))
                this.resourcesDict.Add(Resource.Lumber, int.Parse(attrDict["lumber"]));

            if (!attrDict["steel"].CustomEquals("0"))
                this.resourcesDict.Add(Resource.Steel, int.Parse(attrDict["steel"]));

            if (!attrDict["marble"].CustomEquals("0"))
                this.resourcesDict.Add(Resource.Marble, int.Parse(attrDict["marble"]));

            if (!attrDict["food"].CustomEquals("0"))
                this.resourcesDict.Add(Resource.Food, int.Parse(attrDict["food"]));

            if (!attrDict["gold"].CustomEquals("0"))
                this.resourcesDict.Add(Resource.Gold, int.Parse(attrDict["gold"]));

            if (!attrDict["gem"].CustomEquals("0"))
                this.resourcesDict.Add(Resource.Gem, int.Parse(attrDict["gem"]));
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static ForceRewardConf() {
            ConfigureManager.Instance.LoadConfigure<ForceRewardConf>();
        }

        private static void GetAllForceReward() {
            int level = 1;
            ForceRewardConf forceRewardConf = ForceRewardConf.GetConf(level.ToString());
            while (forceRewardConf != null) {
                ForceRewardConf.AllForceConfDict.Add(level, forceRewardConf);
                ++level;
                forceRewardConf = ForceRewardConf.GetConf(level.ToString());
            }
        }


        public static int GetForceLevel(int currentForce) {
            //int currentForce = RoleManager.GetForce();
            if (AllForceConfDict.Count < 1) {
                GetAllForceReward();
            }
            int forceStageCount = AllForceConfDict.Count;
            if (currentForce < AllForceConfDict[1].force) {
                return 0;
            } else if (currentForce >= AllForceConfDict[forceStageCount].force) {
                return forceStageCount;
            } else {
                foreach (var value in ForceRewardConf.AllForceConfDict) {
                    if (value.Key > 1 &&
                        currentForce < value.Value.force &&
                        currentForce >= ForceRewardConf.AllForceConfDict[value.Key - 1].force) {
                        return (value.Key - 1);
                    }
                }

                Debug.LogError(currentForce + " not valid! " + forceStageCount);
                return -1;
            }
        }

        public static int GetThisLevelCurrentForce() {
            int level = GetForceLevel(RoleManager.GetForce());
            ForceRewardConf conf;
            level = Mathf.Max(1, level);
            level = Mathf.Min(13, level);
            if (AllForceConfDict.TryGetValue(level, out conf)) {
                return conf.force;
            } else {
                Debug.LogError("level Get Faile! " + level);
                return 0;
            }
        }

        public static int GetCurrentForceLevelForce() {
            int level = GetForceLevel(RoleManager.GetForce()) + 1;
            ForceRewardConf conf;
            level = Mathf.Max(1, level);
            level = Mathf.Min(13, level);
            if (AllForceConfDict.TryGetValue(level, out conf)) {
                return conf.force;
            } else {
                Debug.LogError("level Get Faile! " + level);
                return 0;
            }
        }

        public static ForceRewardConf GetConf(string id) {
            return ConfigureManager.GetConfById<ForceRewardConf>(id);
        }
    }
}
