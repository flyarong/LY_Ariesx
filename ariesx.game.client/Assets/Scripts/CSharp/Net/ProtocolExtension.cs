using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using Poukoute;
using System.Text;

namespace Protocol {
    public static class ProtocolExtentsion {
        public static string GetId(this Hero hero) {
            return hero.Name;
        }

        public static string GetId(this Battle.Hero hero) {
            return hero.Name;
        }

        public static string GetId(this ElementBuilding building) {
            //if (building == null) {
            //    return null;
            //}
            int level = Mathf.Max(1, building.Level);
            return building.Name + "_" + level;
        }

        public static string GetNextLevelId(this ElementBuilding building) {
            // to do: find the max level build.
            return building.Name + "_" + (building.Level + 1);
        }

        public static int GetHeroPosition(this List<HeroPosition> positionList, string name) {
            foreach (HeroPosition heroPosition in positionList) {
                if (heroPosition.Name == name) {
                    return heroPosition.Position;
                }
            }
            return 1;
        }

        public static string GetBattleOccureTileName(this PointInfo reportInfo) {
            if (reportInfo.NpcCity != null) {
                string confKey = reportInfo.MapSN + "," +
                          reportInfo.ZoneSN + "," +
                          reportInfo.NpcCity.SN + "," +
                          (reportInfo.NpcCity.IsCenter ? 1 : 0);
                NPCCityConf cityConf = NPCCityConf.GetConf(confKey);
                return GameHelper.GetNameAndLevel(
                    NPCCityConf.GetNpcCityLocalName(cityConf.name, cityConf.isCenter),
                    cityConf.level);
            } else if (reportInfo.Camp != null) {
                return GameHelper.GetNameAndLevel(
                    "Camp",
                    reportInfo.Camp.Level);
            } else if (reportInfo.Building != null) {
                return MapUtils.GetBuildingLocalName(reportInfo.Building);
            } else if (reportInfo.Resource != null) {
                string elementType =
                    Enum.GetName(typeof(ElementType), reportInfo.Resource.Type).ToLower();
                return GameHelper.GetNameAndLevel(
                    LocalManager.GetValue("resource_", elementType),
                    reportInfo.Resource.Level);
            } else if (reportInfo.Pass != null) {
                string passKey = reportInfo.MapSN + "_" + reportInfo.ZoneSN + "_"
                        + (int)reportInfo.Coord.X + "_" + (int)reportInfo.Coord.Y;
                return GameHelper.GetNameAndLevel(
                    LocalManager.GetValue("pass_", passKey),
                    reportInfo.Pass.Level
                );
            } else if (reportInfo.Building != null) {
                return GameHelper.GetNameAndLevel(LocalManager.GetValue("name_",
                    reportInfo.ElementType.ToString()), reportInfo.Building.Level);
            } else {
                return GameHelper.GetNameAndLevel(LocalManager.GetValue("resource_",
                    reportInfo.ElementType.ToString()), 1);
            }
        }

        public static string GetOccureTileName(this PointInfo reportInfo) {
            if (reportInfo.NpcCity != null) {
                string confKey = reportInfo.MapSN + "," +
                          reportInfo.ZoneSN + "," +
                          reportInfo.NpcCity.SN + "," +
                          (reportInfo.NpcCity.IsCenter ? 1 : 0);
                NPCCityConf cityConf = NPCCityConf.GetConf(confKey);
                return GameHelper.GetNameAndLevel(
                   NPCCityConf.GetNpcCityLocalName(cityConf.name, cityConf.isCenter),
                    cityConf.level);
            } else if (reportInfo.Camp != null) {
                return GameHelper.GetNameAndLevel(
                    "Camp",
                    reportInfo.Camp.Level);
            } else if (reportInfo.Building != null) {
                return MapUtils.GetBuildingLocalName(reportInfo.Building);
            } else if (reportInfo.Resource != null) {

                string elementType =
                    Enum.GetName(typeof(ElementType), reportInfo.Resource.Type).ToLower();
                return GameHelper.GetNameAndLevel(
                    LocalManager.GetValue("resource_", elementType),
                    reportInfo.Resource.Level);
            } else if (reportInfo.Pass != null) {
                string passKey = reportInfo.MapSN + "_" + reportInfo.ZoneSN + "_"
                        + (int)reportInfo.Coord.X + "_" + (int)reportInfo.Coord.Y;
                return GameHelper.GetNameAndLevel(
                    LocalManager.GetValue("pass_", passKey),
                    reportInfo.Pass.Level
                );
            } else if (reportInfo.Building != null) {
                return GameHelper.GetNameAndLevel(LocalManager.GetValue("name_",
                    reportInfo.ElementType.ToString()), reportInfo.Building.Level);
            } else {
                return GameHelper.GetNameAndLevel(LocalManager.GetValue("resource_",
                    reportInfo.ElementType.ToString()), 1);
            }
        }
    }



    public partial class Coord {
        public Coord(Coord other) {
            this.X = other.X;
            this.Y = other.Y;
        }


        public Coord(int x, int y) {
            this.X = x;
            this.Y = y;
        }

        public override string ToString() {
            StringBuilder s = new StringBuilder();
            return s.AppendFormat("({0},{1})", this.X, this.Y).ToString();
        }

        public override bool Equals(System.Object obj) {
            if (obj == null) {
                return false;
            }
            Coord p = obj as Coord;
            if ((System.Object)p == null) {
                return false;
            }

            return (this.X == p.X) && (this.Y == p.Y);
        }

        public bool Equals(Coord p) {
            if ((object)p == null) {
                return false;
            }

            return (this.X == p.X) && (this.Y == p.Y);
        }

        public override int GetHashCode() {
            return this.X ^ this.Y;
        }

        public static bool operator ==(Coord a, Coord b) {
            if ((object)a == null && (object)b == null) {
                return true;
            }
            if (((object)a != null && (object)b == null) ||
                (object)a == null && (object)b != null) {
                return false;
            }
            return (a.X == b.X && a.Y == b.Y);
        }

        public static bool operator !=(Coord a, Coord b) {
            if ((object)a == null && (object)b == null) {
                return false;
            }
            if (((object)a != null && (object)b == null) ||
                (object)a == null && (object)b != null) {
                return true;
            }
            return (a.X != b.X || a.Y != b.Y);
        }

        public static implicit operator Vector2(Coord coord) {
            return new Vector2(coord.X, coord.Y);
        }

        public static implicit operator Coord(Vector2 coord) {
            return new Coord(Mathf.RoundToInt(coord.x), Mathf.RoundToInt(coord.y));
        }
    }

    public partial class Point {
        public bool isCaculate = false;
        public bool isVisible = true;
        public bool isTroopVisible = true;
        public bool isSightBuilding = false;

        public bool IsSelf {
            get {
                return this.PlayerId == RoleManager.GetRoleId();
            }
        }

        public bool IsAlly {
            get {
                return !this.AllianceId.CustomIsEmpty() &&
                    this.AllianceId == RoleManager.GetAllianceId();
            }
        }

        public bool IsSlave {
            get {
                return !this.BelongsAllianceId.CustomIsEmpty() &&
                    this.BelongsAllianceId == RoleManager.GetAllianceId();
            }
        }

        public bool IsMaster {
            get {
                return !this.AllianceId.CustomIsEmpty() &&
                    this.AllianceId.CustomEquals(RoleManager.GetMasterAllianceId());

            }
        }

        public int GetLevel() {
            if (this.Resource != null) {
                return this.Resource.Level;
            } else if (this.Pass != null) {
                return this.Pass.Level;
            } else if (this.NpcCity != null) {
                return NPCCityConf.GetConf(this).level;
            } else if (this.Building != null) {
                return this.Building.Level;
            } else {
                return 0;
            }
        }

        public int GetMinLevel() {
            int minLevel = 0;
            if (this.Resource != null) {
                minLevel = minLevel > this.Resource.Level ? this.Resource.Level : minLevel;
            }

            if (this.Pass != null) {
                minLevel = minLevel > this.Pass.Level ? this.Pass.Level : minLevel;
            }

            if (this.NpcCity != null) {
                int cityLevel = NPCCityConf.GetConf(this).level;
                minLevel = minLevel > cityLevel ? cityLevel : minLevel;
            }

            if (this.Building != null) {
                minLevel = minLevel > this.Building.Level ? this.Building.Level : minLevel;
            }

            return minLevel;
        }

        public int GetMaxDuration() {
            int maxDurance = GameConst.MAX_ENDURANCE;
            ElementType pointType = (ElementType)this.ElementType;
            if ((int)pointType >= (int)Poukoute.ElementType.townhall &&
                   (int)pointType <= (int)Poukoute.ElementType.durability_up) {
                int level = Mathf.Max(1, this.Building.Level);
                maxDurance = BuildingConf.GetDurability(
                    string.Concat(this.Building.Name, "_", level)
                );
            } else if (pointType == Poukoute.ElementType.npc_city) {
                string cityKey = NPCCityConf.GetCityKey(this);
                NPCCityConf cityConf = NPCCityConf.GetConf(cityKey);
                maxDurance = cityConf.durability;
            } else if (pointType == Poukoute.ElementType.pass) {
                string passKey = this.MapSN + "," + this.ZoneSN + ","
                    + this.Coord.X + "," + this.Coord.Y;
                MiniMapPassConf passConf = MiniMapPassConf.GetConf(passKey);
                string passType = GameConst.PASS_BRIDGE;
                if (MiniMapPassConf.GetMainPassList().Contains(passConf)) {
                    passType = GameConst.PASS_PASS;
                }
                maxDurance = PassConf.GetPassDurability(
                    this.Pass.Level,
                    passType
                );
            }

            return maxDurance;
        }
    }

    public partial class Hero {
        public int NewEnergy = 0;
        public float armyCoeff = 1;

        public bool IsUpgradeable {
            get {
                if (this.Level < 1) {
                    return false;
                }
                int heroUpgradeNeedsFragments = HeroLevelConf.GetHeroUpgradFragments(this);

                if (HeroLevelConf.GetHeroReachMaxLevel(this.Name, this.Level)) {
                    return false;
                }
                return FragmentCount >= heroUpgradeNeedsFragments;
            }
        }
    }


    public partial class ChapterTask {
        public bool unlocked = false;
    }


    public enum ItemType {
        chest,
        fragment,
        gem,
        gold,
        resource
    }

    public partial class Resources {
        private Dictionary<Resource, int> resourceDict =
            new Dictionary<Resource, int>(5);

        public Dictionary<Resource, int> GetResourceDict() {
            this.resourceDict.Clear();
            if (this.Lumber != 0) {
                this.resourceDict.Add(Resource.Lumber, this.Lumber);
            }

            if (this.Steel != 0) {
                this.resourceDict.Add(Resource.Steel, this.Steel);
            }

            if (this.Marble != 0) {
                this.resourceDict.Add(Resource.Marble, this.Marble);
            }

            if (this.Food != 0) {
                this.resourceDict.Add(Resource.Food, this.Food);
            }

            if (this.Crystal != 0) {
                this.resourceDict.Add(Resource.Crystal, this.Crystal);
            }

            return this.resourceDict;
        }
    }

    public partial class BattleReport {
        private Dictionary<Resource, int> attachmentsDict =
            new Dictionary<Resource, int>(5);

        public Dictionary<Resource, int> GetAttachmentsDict() {
            this.attachmentsDict.Clear();

            this.attachmentsDict = this.Resources.GetResourceDict();
            if (this.Currency.Gold > 0) {
                this.attachmentsDict.Add(Resource.Gold, this.Currency.Gold);
            }
            if (this.Currency.Gem > 0) {
                this.attachmentsDict.Add(Resource.Gem, this.Currency.Gem);
            }

            if (!this.ChestName.CustomIsEmpty()) {
                this.attachmentsDict.Add(Resource.Chest, 1);
            }

            return this.attachmentsDict;
        }
    }

    public partial class CommonReward {
        private Dictionary<Resource, int> rewardsDict =
            new Dictionary<Resource, int>(10);
        private Dictionary<Resource, int> speicialDict =
            new Dictionary<Resource, int>();

        public Dictionary<Resource, int> GetRewardsDict(bool containFragments = true, bool containChests = true) {
            this.rewardsDict = this.Resources.GetResourceDict();

            if (this.Chests != null && this.Chests.Count != 0) {
                this.rewardsDict.Add(Resource.Chest, this.Chests.Count);
            }

            if (this.Currency.Gem != 0 && containChests) {
                this.rewardsDict.Add(Resource.Gem, this.Currency.Gem);
            }

            if (this.Currency.Gold != 0) {
                this.rewardsDict.Add(Resource.Gold, this.Currency.Gold);
            }

            if (containFragments && this.Fragments != null) {
                int fragmentAmount = 0;
                foreach (HeroFragment fragment in this.Fragments) {
                    fragmentAmount += fragment.Count;
                }
                if (fragmentAmount != 0) {
                    this.rewardsDict.Add(Resource.Fragment, fragmentAmount);
                }
            }
            return this.rewardsDict;
        }

        public Dictionary<Resource, int> GetNormalDict() {
            return this.Resources.GetResourceDict();
        }

        public Dictionary<Resource, int> GetSpecialDict() {
            this.speicialDict.Clear();

            if (this.Resources.Crystal != 0) {
                this.speicialDict.Add(Resource.Crystal, this.Resources.Crystal);
            }

            if (this.Currency.Gold != 0) {
                this.speicialDict.Add(Resource.Gold, this.Currency.Gold);
            }

            if (this.Currency.Gem != 0) {
                this.speicialDict.Add(Resource.Gem, this.Currency.Gem);
            }

            if (this.Chests != null && this.Chests.Count != 0) {
                this.speicialDict.Add(Resource.Chest, this.Chests.Count);
            }

            if (this.Fragments != null) {
                int fragmentAmount = 0;
                foreach (HeroFragment fragment in this.Fragments) {
                    fragmentAmount += fragment.Count;
                }
                if (fragmentAmount != 0) {
                    this.speicialDict.Add(Resource.Fragment, fragmentAmount);
                }
            }
            return this.speicialDict;
        }


        public List<ItemType> GetRewardItemTypes() {
            List<ItemType> rewardItemType = new List<ItemType>(5);

            if (this.Chests != null) {
                int chestAmount = 0;
                foreach (Chest chest in this.Chests) {
                    chestAmount += chest.Count;
                }
                if (chestAmount > 0) {
                    rewardItemType.Add(ItemType.chest);
                }
            }

            if (this.Currency.Gem != 0) {
                rewardItemType.Add(ItemType.gem);
            }

            if (this.Currency.Gold != 0) {
                rewardItemType.Add(ItemType.gold);
            }

            if (this.Fragments != null) {
                int fragmentAmount = 0;
                foreach (HeroFragment fragment in this.Fragments) {
                    fragmentAmount += fragment.Count;
                }
                if (fragmentAmount > 0) {
                    rewardItemType.Add(ItemType.fragment);
                }
            }

            if (this.Resources.Lumber != 0 ||
                this.Resources.Steel != 0 ||
                this.Resources.Marble != 0 ||
                this.Resources.Food != 0 ||
                this.Resources.Crystal != 0) {
                rewardItemType.Add(ItemType.resource);
            }

            return rewardItemType;
        }
    }

    public partial class Activity {
        public enum ActivityStatus {
            Started,
            Preheat,
            Finish,
            None
        }

        public ActivityStatus Status {
            get {
                return this.GetActivityStatus();
            }
        }


        private long activityRemainTime = -1;
        public long ActivityRemainTime {
            get {
                if (activityRemainTime == -1) {
                    this.GetActivityStatus();
                }
                return this.activityRemainTime;

            }
        }

        public CampaignType CampaignType {
            get {
                if (CampaignType.IsDefined(typeof(CampaignType), this.Base.Type)) {
                    return (CampaignType)Enum.Parse(typeof(CampaignType), this.Base.Type);
                } else {
                    return CampaignType.none;
                }
            }
        }

        public List<RankReward> RankRewards {
            get {
                switch (this.CampaignType) {
                    case CampaignType.melee:
                        return this.Melee.RankReward;
                    case CampaignType.occupy:
                        return this.Occupy.RankReward;
                    case CampaignType.capture:
                        return this.Capture.RankReward;
                    case CampaignType.domination:
                        return null;
                    case CampaignType.none:
                    default:
                        return null;
                }
            }
        }

        public List<IntegralReward> IntegralReward {
            get {
                switch (this.CampaignType) {
                    case CampaignType.melee:
                        return this.Melee.IntegralReward;
                    case CampaignType.occupy:
                        return this.Occupy.IntegralReward;
                    case CampaignType.capture:
                        return this.Capture.IntegralReward;
                    case CampaignType.domination:
                        return null;
                    case CampaignType.none:
                    default:
                        return null;
                }
            }
        }



        public string GetActivitySubject() {
            int activityInfosCount = this.Base.ActivityInfo.Count;
            for (int i = 0; i < activityInfosCount; i++) {
                if (this.Base.ActivityInfo[i].Language.CustomEquals(VersionConst.language) ||
                    ((this.Base.ActivityInfo[i].Language == "zh-CN") &&
                    (VersionConst.language == "cn"))) {
                    return this.Base.ActivityInfo[i].Subject;
                }
            }
            return string.Empty;
        }

        public string GetActivityBody() {
            int activityInfosCount = this.Base.ActivityInfo.Count;
            for (int i = 0; i < activityInfosCount; i++) {
                //Debug.LogError(this.Base.ActivityInfo[i].Language + " " + VersionConst.language);
                if (this.Base.ActivityInfo[i].Language.CustomEquals(VersionConst.language) ||
                    ((this.Base.ActivityInfo[i].Language == "zh-CN") &&
                    (VersionConst.language == "cn"))) {
                    return this.Base.ActivityInfo[i].Body;
                }
            }

            return string.Empty;
        }

        private ActivityStatus GetActivityStatus() {
            long currentTime = RoleManager.GetCurrentUtcTime() / 1000;
            //Debug.LogError(currentTime + ", " + this.Base.PrepareTime + ", " + Base.StartTime);
            if (currentTime >= this.Base.PrepareTime && currentTime < this.Base.StartTime) {
                this.activityRemainTime = this.Base.StartTime - currentTime;
                return ActivityStatus.Preheat;
            } else if (currentTime >= this.Base.StartTime && currentTime < this.Base.EndTime) {
                this.activityRemainTime = this.Base.EndTime - currentTime;
                return ActivityStatus.Started;
            } else if (currentTime >= this.Base.EndTime) {
                this.activityRemainTime = -1;
                return ActivityStatus.Finish;
            } else {
                this.activityRemainTime = this.Base.PrepareTime - currentTime;
                return ActivityStatus.None;
            }

        }
    }
}
