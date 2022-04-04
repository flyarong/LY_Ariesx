using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class CampaignModel: BaseModel {
        public List<Activity> allActivities = new List<Activity>(4);
        public Activity chosenActivity = null;
        // DevilFighting
        public static string monsterType = string.Empty;
        public const string nian = "monster_nian";
        public static string MonsterLocalName {
            get {
                if (monsterType == nian) {
                    return LocalManager.GetValue(LocalHashConst.melee_evil_nian);
                } else {
                    return LocalManager.GetValue(LocalHashConst.campaign_monster);
                }
            }
        }

        // HoldGrounKing
        public int holdGroundKingLogPage = 0;
        public bool holdGroundKingLogIsLoadAll = false;

        //ContinentDisputes
        public int continentDisputesLogPage = 0;
        public bool continentDisputesLogIsLoadAll = false;

        //DemonShadow
        public int dominationHistoryPage = 0;
        public bool dominationHistoryIsLoadAll = false;

        public List<Domination> dominationRewardList = new List<Domination>();
        public List<NPCCityConf> NPCCityConfList = new List<NPCCityConf>();

        public long GetDevilFightingLeftTime() {
            foreach (Activity activity in this.allActivities) {
                if (activity.CampaignType == CampaignType.melee &&
                    activity.Status == Activity.ActivityStatus.Started) {
                    return activity.ActivityRemainTime;
                }
            }
            return 0;
        }

        public int GetUsableActivityCount() {
            int count = 0;
            foreach (Activity activity in this.allActivities) {
                Activity.ActivityStatus status = activity.Status;
                if (status != Activity.ActivityStatus.None) {
                    count++;
                }
            }
            return count;
        }

        //LoginReward
        public LoginRewardAck loginRewardAck = null;
        public int OpenServiceActivityCout = 0;
    }
}
