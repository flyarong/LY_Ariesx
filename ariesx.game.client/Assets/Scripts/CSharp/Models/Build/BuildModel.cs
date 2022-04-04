using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using System.Text.RegularExpressions;
using System;
using System.Text;
using UnityEngine.UI;

namespace Poukoute {
    public class BuildInfo {
        public Dictionary<Resource, long> resouceDict;
        public long timeCost;
        public int built;
        public int canBuild;
    }

    public class UnlockBuildingInfo {
        public string buildingKey;
        public string buildingName;
        public string buildingLevel;
        public int buildingCount;
        public bool isBuildingNew;

        public UnlockBuildingInfo(string buildingName, string buildingLevel,
            int buildingCount, bool isNew) {
            this.buildingName = buildingName;
            this.buildingLevel = buildingLevel;
            this.buildingCount = buildingCount;
            this.isBuildingNew = isNew;
            this.buildingKey = string.Format("{0}_{1}",
                this.buildingName, this.buildingLevel);
        }
    }

    public class UnlockShowBuildingInfo {
        public string buildingKey;
        public string buildingName;
        public string buildingLevel;
        public int buildingCount;
        public bool isBuildingNew;
        public string unlockShowLevel;

        public UnlockShowBuildingInfo(string buildingName, string buildingLevel,
            int buildingCount, bool isNew, string unlockShowLevel) {
            this.buildingName = buildingName;
            this.buildingLevel = buildingLevel;
            this.buildingCount = buildingCount;
            this.isBuildingNew = isNew;
            this.buildingKey = string.Format("{0}_{1}",
                this.buildingName, this.buildingLevel);
            this.unlockShowLevel = unlockShowLevel;
        }
    }

    public enum BuildViewType {
        List,
        Info,
        Upgrade
    }

    public class BuildModel : BaseModel {
        /* Add data member in this */
        // Can already build in building list 
        public Dictionary<string, UnlockBuildingInfo> canBeBuildBuildingDict =
            new Dictionary<string, UnlockBuildingInfo>(0);
        public List<ElementBuilding> brokenBuildingList =
                    new List<ElementBuilding>(0);
        public Dictionary<string, UnlockShowBuildingInfo> unlockShowBuildingDict =
            new Dictionary<string, UnlockShowBuildingInfo>(0);
        private List<string> unlockList = new List<string>(0);
        // Can unlock if you upgrade a building.
        public Dictionary<string, UnlockBuildingInfo> buildingUnlockDict =
            new Dictionary<string, UnlockBuildingInfo>(0);
        public Dictionary<string, int> buildingCountDict = new Dictionary<string, int>(0);
        public Dictionary<string, ElementBuilding> buildingDict =
            new Dictionary<string, ElementBuilding>(0);

        public List<string> avaliableList;
        public string currentBuilding;
        public string level;
        public BuildViewType buildViewType;
        public Vector2 buildCoordinate;
        /***************************/
        public void Refresh(LoginAck loginAck) {
            this.buildingDict.Clear();
            this.buildingCountDict.Clear();
            BuildingConf configure = null;
            foreach (ElementBuilding building in loginAck.Buildings) {
                this.buildingDict.Add(building.Name, building);
                if (!building.IsBroken) {
                    configure = BuildingConf.GetConf(building.GetId());
                    if (!this.buildingCountDict.ContainsKey(configure.type)) {
                        this.buildingCountDict.Add(configure.type, 1);
                    } else {
                        this.buildingCountDict[configure.type]++;
                    }
                }
            }
            this.SetUnlockAndBrokenBuildingDict();
        }

        public void Refresh(PlayerBuildingNtf playerBuildingNtf) {
            ElementBuilding building = playerBuildingNtf.Building;
            int level = Mathf.Max(1, building.Level);
            BuildingConf conf = BuildingConf.GetConf(building.Name + "_" + level);

            bool isMethodDel = playerBuildingNtf.Method.CustomEquals("del");
            if (!this.buildingDict.ContainsKey(building.Name) && !isMethodDel) {
                if (!this.buildingCountDict.ContainsKey(conf.type)) {
                    this.buildingCountDict.Add(conf.type, 1);
                } else {
                    this.buildingCountDict[conf.type]++;
                }
            }
            if (isMethodDel) {
                if (this.buildingCountDict.ContainsKey(conf.type)) {
                    if (this.buildingCountDict[conf.type] > 1) {
                        this.buildingCountDict[conf.type]--;
                    } else {
                        this.buildingCountDict.Remove(conf.type);
                    }
                }
                this.buildingDict.Remove(building.Name);
            } else {
                this.buildingDict[building.Name] = building;
            }
            this.SetUnlockAndBrokenBuildingDict();
        }

        public void Refresh() {
            this.SetUnlockAndBrokenBuildingDict();
        }

        public List<Resource> GetProduceResourceList() {
            List<Resource> produceBuilding = new List<Resource>(4);
            List<Resource> produceResource = new List<Resource>(4);
            Resource resource;
            foreach (var building in this.buildingDict) {
                if (building.Key.CustomStartsWith("produce")) {
                    resource = this.GetBuildingResourceType(building.Key);
                    if (!produceBuilding.Contains(resource)) {
                        produceBuilding.Add(resource);
                    }
                }
            }

            foreach (Resource item in produceBuilding) {
                if (item == Resource.Lumber) {
                    produceResource.Add(item);
                }
            }

            foreach (Resource item in produceBuilding) {
                if (item == Resource.Steel) {
                    produceResource.Add(item);
                }
            }

            foreach (Resource item in produceBuilding) {
                if (item == Resource.Marble) {
                    produceResource.Add(item);
                }
            }

            foreach (Resource item in produceBuilding) {
                if (item == Resource.Food) {
                    produceResource.Add(item);
                }
            }

            return produceResource;
        }


        private Resource GetBuildingResourceType(string buildingName) {
            if (buildingName.Contains("lumber")) {
                return Resource.Lumber;
            } else if (buildingName.Contains("food")) {
                return Resource.Food;
            } else if (buildingName.Contains("steel")) {
                return Resource.Steel;
            } else if (buildingName.Contains("marble")) {
                return Resource.Marble;
            } else {
                return Resource.None;
            }
        }

        private void SetUnlockAndBrokenBuildingDict() {
            this.canBeBuildBuildingDict.Clear();
            this.brokenBuildingList.Clear();
            this.unlockShowBuildingDict.Clear();
            Dictionary<string, BaseConf> buildingConfDict =
                ConfigureManager.GetConfDict<BuildingConf>();
            foreach (BaseConf baseConf in buildingConfDict.Values) {
                BuildingConf buildingConf = baseConf as BuildingConf;
                if (buildingConf.unlockCondition.CustomIsEmpty()) {
                    if (!this.buildingDict.ContainsKey(buildingConf.buildingName)) {
                        this.canBeBuildBuildingDict.Add(
                            buildingConf.type,
                            new UnlockBuildingInfo(buildingConf.buildingName, "1", 1, true)
                        );
                    }
                    continue;
                }

                bool isBuildExist = this.AddBrokenBuildingInfo(buildingConf);
                if (!isBuildExist) {
                    this.AddUnlockBuildingInfo(buildingConf);
                }
            }
        }

        private void AddUnlockBuildingInfo(BuildingConf buildingConf) {
            string[] unlockConditionArray = buildingConf.unlockCondition.CustomSplit(',');
            string conditionBuildName = unlockConditionArray[0];
            int conditonLevel = int.Parse(unlockConditionArray[1]);
            ElementBuilding building;

            if ((buildingConf.buildingLevel == 1) &&
                this.buildingDict.TryGetValue(conditionBuildName, out building)
                && building.Level >= conditonLevel) {
                //已解锁建筑
                UnlockBuildingInfo unlockBuildingInfo;
                if (this.canBeBuildBuildingDict.TryGetValue(buildingConf.type, out unlockBuildingInfo)) {
                    unlockBuildingInfo.buildingCount++;
                } else {
                    unlockBuildingInfo =
                        new UnlockBuildingInfo(buildingConf.buildingName, "1", 1, true);
                    this.canBeBuildBuildingDict.Add(buildingConf.type, unlockBuildingInfo);
                }
            } else if (!buildingConf.unlockShow.CustomIsEmpty()) { //待解锁建筑
                string[] unlockShowArray = buildingConf.unlockShow.CustomSplit(',');
                string unlockShowbuildingName = unlockShowArray[0];
                int unlockShowLevel = int.Parse(unlockShowArray[1]);
                this.buildingDict.TryGetValue(unlockShowbuildingName, out building);
                if (building.Level >= unlockShowLevel && !building.IsBroken) {
                    UnlockShowBuildingInfo unlockShowBuildingInfo;
                    if (this.unlockShowBuildingDict.TryGetValue(buildingConf.type, out unlockShowBuildingInfo)) {
                        unlockShowBuildingInfo.buildingCount++;
                    } else {
                        unlockShowBuildingInfo =
                            new UnlockShowBuildingInfo(buildingConf.buildingName, "1", 1, true, unlockShowLevel.ToString());
                        this.unlockShowBuildingDict.Add(buildingConf.type, unlockShowBuildingInfo);
                    }
                }
            }

            if ((buildingConf.buildingLevel == 1) &&
               this.buildingDict.TryGetValue(conditionBuildName, out building) && true) {

            }
        }


        private bool AddBrokenBuildingInfo(BuildingConf buildingConf) {
            bool isBuildExist = false;
            ElementBuilding buildedBuilding;
            if (this.buildingDict.TryGetValue(buildingConf.buildingName, out buildedBuilding)) {
                isBuildExist = true;
                if (buildedBuilding.IsBroken && buildedBuilding.Level > 0) {
                    this.brokenBuildingList.TryAdd(buildedBuilding);
                }
            }
            return isBuildExist;
        }

        //public string GetMaxLevelBuildingWithType(string type) {
        //    int level = 0;
        //    string buildingName = string.Empty;
        //    BuildingConf buildingConf;
        //    foreach (var pair in this.buildingDict) {
        //        if (pair.Value.Level == 0) {
        //            continue;
        //        }
        //        buildingConf = BuildingConf.GetConf(pair.Value.GetId());
        //        if (buildingConf.type == type &&
        //            pair.Value.Level > level &&
        //            !pair.Value.IsBroken) {
        //            level = pair.Value.Level;
        //            buildingName = pair.Key;
        //        }
        //    }
        //    return buildingName;
        //}

        public int GetTownhallLevel() {
            foreach (var build in buildingDict) {
                if (build.Key.CustomEquals(ElementName.townhall)) {
                    return build.Value.Level;
                }
            }
            return 1;
        }

        public int GetHavedBuilding(string type) {
            int count;
            if (!this.buildingCountDict.TryGetValue(type, out count)) {
                count = 0;
            }
            return count;
        }

        public int GetAllBuildableAmount(string type) {
            UnlockBuildingInfo unlockBuilding;
            if (this.canBeBuildBuildingDict.TryGetValue(type, out unlockBuilding)) {
                return unlockBuilding.buildingCount;
            }
            return 0;
        }

        public string GetBuildingNumber(string type) {
            //return this.GetHavedBuilding(type) + "/" + BuildingConf.buildingCountDict[type];
            return this.GetHavedBuilding(type).ToString();
        }

        public string GetUnlockBuildingTips(BuildingConf unlockBuildConf) {
            if (!this.buildingDict.ContainsKey(unlockBuildConf.buildingName)) {
                return LocalManager.GetValue(LocalHashConst.unlock_build_new);
            }
            return GameHelper.GetLevelLocal(unlockBuildConf.buildingLevel);
        }


        public bool GetUpgradeConditionConf(out string upgradeDescription, BuildingConf buildingConf,
            out bool showImg, out string unlockBuildName, out string unlockBuildType, out string level) {

            string unlockBuildingId = string.Empty;
            // To do: May provide the more detail of build event.
            if (EventManager.IsBuildEventMaxFull()) {
                upgradeDescription = LocalManager.GetValue(LocalHashConst.building_queue_full);
                showImg = false;
                unlockBuildName = string.Empty;
                unlockBuildType = string.Empty;
                level = string.Empty;
                return false;
            }
            if (!buildingConf.unlockCondition.CustomIsEmpty()) {
                string[] unlockValue = buildingConf.unlockCondition.CustomSplit(',');
                string relyBuildingName = unlockValue[0];
                int relyBuildingLevel = int.Parse(unlockValue[1]);
                BuildingConf unlockConf = BuildingConf.GetConf(string.Concat(
                    relyBuildingName,
                    "_",
                    relyBuildingLevel
                ));
                string[] unlockBuilding = MapUtils.GetUnlockBuildingName(relyBuildingName).CustomSplit(',');
                unlockBuildType = unlockConf.type;
                if (unlockBuilding.Length == 1) {
                    unlockBuildingId = string.Empty;
                } else {
                    unlockBuildingId = unlockBuilding[1];
                }
                if (this.GetBuildMaxLevelByName(relyBuildingName) < relyBuildingLevel) {
                    upgradeDescription = string.Format(
                       LocalManager.GetValue(LocalHashConst.building_list_locked),
                      ((LocalManager.GetValue("name_", unlockBuildType)) + unlockBuildingId),
                       relyBuildingLevel);
                    unlockBuildName = relyBuildingName;
                    showImg = true;
                    level = relyBuildingLevel.ToString();
                    return false;
                }
            }

            upgradeDescription = string.Empty;
            unlockBuildName = string.Empty;
            unlockBuildType = string.Empty;
            level = string.Empty;
            showImg = false;
            return true;
        }

        public bool GetUpgradeForceConf(out string upgradeDescrip, BuildingConf buildingConf) {
            int forceLevel = ForceRewardConf.GetForceLevel
                (RoleManager.GetForce());
            int unlockTownhallLevel;
            if (forceLevel != 0) {
                unlockTownhallLevel = ForceRewardConf
                    .GetConf(forceLevel.ToString()).unlockTownhallLevel;
            } else {
                unlockTownhallLevel = 1;
            }
            if (!buildingConf.buildingName.CustomEquals(ElementName.townhall)) {
                upgradeDescrip = string.Empty;
                return true;
            }
            if (unlockTownhallLevel < buildingConf.buildingLevel) {
                upgradeDescrip = string.Format(LocalManager.GetValue(LocalHashConst.townhall_power_require),
                    buildingConf.buildingLevel - 1);
                return false;
            } else {
                upgradeDescrip = string.Empty;
                return true;
            }
        }

        public bool IsBuildingRechMaxLevel(string building) {
            int maxLevel = BuildingConf.GetBuildMaxLevelByName(building);
            return (this.buildingDict[building].Level >= maxLevel);
        }

        public Vector2 GetUpgradeableBuildingCoord() {
            foreach (ElementBuilding build in this.buildingDict.Values) {
                if (this.GetBuildingUpgradeable(build.Name, build.Level)) {
                    return build.Coord;
                }
            }
            return Vector2.zero;
        }

        public bool GetBuildingUpgradeable(string buildingName, int buildingLevel) {
            StringBuilder s = new StringBuilder();
            BuildingConf buildingConf = BuildingConf.GetConf(
                s.AppendFormat("{0}_{1}", buildingName, buildingLevel).ToString());
            if (buildingConf == null) {
                return false;
            }

            if (!buildingConf.unlockCondition.CustomIsEmpty()) {
                string[] unlockValue = buildingConf.unlockCondition.CustomSplit(',');
                string relyBuildingName = unlockValue[0];
                int relyBuildingLevel = int.Parse(unlockValue[1]);
                if (this.GetBuildMaxLevelByName(relyBuildingName) < relyBuildingLevel) {
                    return false;
                }

                foreach (var pair in buildingConf.resourceDict) {
                    if (pair.Value > RoleManager.GetResource(pair.Key)) {
                        return false;
                    }
                }
            }

            if (buildingConf.type.CustomEquals(ElementName.townhall)) {
                string desc;
                return this.GetUpgradeForceConf(out desc, buildingConf);
            }

            return true;
        }

        public Vector2 GetMaxLevelBuildCoordOfType(string type) {
            int level = 0;
            Vector2 target = Vector2.one * -1;
            foreach (ElementBuilding building in this.buildingDict.Values) {
                BuildingConf buildingConf = BuildingConf.GetConf(building.GetId());
                if (buildingConf.type == type && building.Level >= level) {
                    level = building.Level;
                    target = building.Coord;
                }
            }
            return target;
        }

        public Vector2 GetBuildCoordinateByName(string buildName) {
            foreach (ElementBuilding building in this.buildingDict.Values) {
                BuildingConf buildingConf = BuildingConf.GetConf(building.GetId());
                if (buildingConf.buildingName.CustomEquals(buildName)) {
                    //Debug.LogError(buildName + " " + building.IsBroken);
                    return building.IsBroken ? GameConst.LeftUp : (Vector2)building.Coord;
                }
            }
            return GameConst.LeftDown;
        }

        public int GetBuildLevelByName(string buildName) {
            foreach (var build in this.buildingDict.Values) {
                if (buildName == build.Name) {
                    return build.Level;
                }
            }

            return 0;
        }

        public ElementType GetBuildTypeWithCoord(Coord coord) {
            foreach (ElementBuilding building in this.buildingDict.Values) {
                if (building.Coord == coord) {
                    return (ElementType)building.Type;
                }
            }
            return ElementType.none;
        }

        public ElementBuilding GetBuildingByCoord(Coord coord) {
            foreach (ElementBuilding building in this.buildingDict.Values) {
                if (building.Coord == coord) {
                    return building;
                }
            }
            return null;
        }


        private int GetBuildMaxLevelByName(string buildingName) {
            int level = 0;
            foreach (ElementBuilding building in this.buildingDict.Values) {
                BuildingConf buildingConf = BuildingConf.GetConf(building.GetId());
                if (buildingConf.buildingName.CustomEquals(buildingName) &&
                    building.Level >= level) {
                    level = building.Level;
                }
            }
            return level;
        }

        public Dictionary<string, UnlockBuildingInfo> GetBuildingUnlockDict(string buildingName, int level) {
            this.GetUnlockList(buildingName, level);
            string insertStronghold = string.Empty;
            this.buildingUnlockDict.Clear();

            foreach (string buildingKey in this.unlockList) {
                if (this.IsStrongholdExist(out insertStronghold)) {
                    this.buildingUnlockDict[insertStronghold].buildingCount++;
                } else {
                    bool isNew = false;
                    string[] unlockBuildID = buildingKey.CustomSplit(',');
                    if (!this.buildingDict.ContainsKey(unlockBuildID[0]) &&
                        unlockBuildID[1] == "1") {
                        isNew = true;
                        this.buildingUnlockDict.Add(buildingKey,
                            new UnlockBuildingInfo(unlockBuildID[0], unlockBuildID[1], 1, isNew));
                    }
                }
            }

            return buildingUnlockDict;
        }

        public List<ElementBuilding> GetBuildingByType(ElementType type) {
            List<ElementBuilding> buildingList = new List<ElementBuilding>();
            foreach (ElementBuilding building in this.buildingDict.Values) {
                if (building.Type == (int)type) {
                    buildingList.Add(building);
                }
            }
            return buildingList;
        }

        private bool IsStrongholdExist(out string strongholdKey) {
            strongholdKey = string.Empty;

            foreach (string buildingName in this.buildingUnlockDict.Keys) {
                if (buildingName.CustomStartsWith(ElementName.stronghold)) {
                    strongholdKey = buildingName;
                    return true;
                }
            }
            return false;
        }

        private List<string> GetUnlockList(string buildingName, int buildingLevel) {
            Dictionary<string, BaseConf> buildingConfDict =
                ConfigureManager.GetConfDict<BuildingConf>();
            this.unlockList.Clear();
            foreach (BaseConf baseConf in buildingConfDict.Values) {
                BuildingConf buildingConf = baseConf as BuildingConf;
                string buildingKey = string.Format(
                    GameConst.TWO_PART_WITH_COMMA, buildingName, buildingLevel);
                if (buildingConf.unlockCondition.CustomEquals(buildingKey)) {
                    this.AddBuildConfToUnlockList(buildingConf);
                }
            }
            return this.unlockList;
        }

        private void AddBuildConfToUnlockList(BuildingConf buildingConf) {
            string unlockBuildId = string.Format(
                GameConst.TWO_PART_WITH_COMMA,
                buildingConf.buildingName,
                buildingConf.buildingLevel
            );
            if (buildingConf.buildingLevel == 1) {
                this.unlockList.Add(unlockBuildId);
            }
        }

        public float GetSpeedBonus() {
            ElementBuilding speedUpBuilding;
            if (this.buildingDict.TryGetValue("march_speed_up_building", out speedUpBuilding) &&
                speedUpBuilding.Level > 0 && !speedUpBuilding.IsBroken) {
                return MarchSpeedUpConf.GetConf(speedUpBuilding.Level.ToString()).percent;
            } else {
                return 0;
            }
        }

        public float GetSiegeBonus() {
            ElementBuilding siegeUpBuilding;
            if (this.buildingDict.TryGetValue("siege_up_building", out siegeUpBuilding) &&
                siegeUpBuilding.Level > 0 && !siegeUpBuilding.IsBroken) {
                string bonus = SiegeUpConf.GetConf(siegeUpBuilding.Level.ToString()).bonus.ToString();
                return float.Parse(bonus);
            } else {
                return 0;
            }
        }

        public int GetAttackBonus() {
            ElementBuilding attackUpBuilding;
            if (this.buildingDict.TryGetValue("hero_attack_up_building", out attackUpBuilding) &&
                attackUpBuilding.Level > 0 && !attackUpBuilding.IsBroken) {
                string bonus = HeroAttackUpConf.GetConf(attackUpBuilding.Level.ToString()).attack_up.ToString();
                return int.Parse(bonus);
            } else {
                return 0;
            }
        }

        public int GetDefenceBonus() {
            ElementBuilding defenceUpBuilding;
            if (this.buildingDict.TryGetValue("hero_defence_up_building", out defenceUpBuilding) &&
                defenceUpBuilding.Level > 0 && !defenceUpBuilding.IsBroken) {
                string bonus = HeroDefenceUpConf.GetConf(defenceUpBuilding.Level.ToString()).defence_up.ToString();
                return int.Parse(bonus);
            } else {
                return 0;
            }
        }

        public float GetDurabilityBonus() {
            ElementBuilding durabilityUpBuilding;
            if (this.buildingDict.TryGetValue("durability_up_building", out durabilityUpBuilding) &&
                durabilityUpBuilding.Level > 0 && !durabilityUpBuilding.IsBroken) {
                string bonus = DurabilityUpConf.GetConf(durabilityUpBuilding.Level.ToString()).defence_up.ToString();
                return float.Parse(bonus);
            } else {
                return 0;
            }
        }
    }
}
