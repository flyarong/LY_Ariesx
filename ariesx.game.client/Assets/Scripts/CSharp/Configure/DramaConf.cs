using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class DramaConf : BaseConf {
        public string id;
        public int chapter;
        public string type;
        public int resourceLevelType;
        public string description;
        public string buildingName;
        public string buildingType;
        public bool isBuildingSpecial = false;
        public int unlockId;
        public List<string> dailyTaskTypes = null;
        public int pointsLimit;
        // target
        public int level;
        public int amount;
        public int times;
        public int rarity;
        // reward
        public int gem;
        public Dictionary<Resource, int> resourcesDict = new Dictionary<Resource, int>();
        public Dictionary<string, int> otherRewardDict = new Dictionary<string, int>();
        public Dictionary<string, int> heroDict = new Dictionary<string, int>();
        public int totalHeroAmount = 0;
        public static Dictionary<int, List<int>> unlockDict = new Dictionary<int, List<int>>();


        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.id = attrDict["id"];
            this.chapter = int.Parse(attrDict["chapter"]);
            this.type = attrDict["type"];
            this.buildingName = attrDict["name"];
            this.buildingType = this.buildingName;
            this.description = attrDict["locale_desc"];
            this.unlockId = string.IsNullOrEmpty(attrDict["unlock_id"]) ? 0 : int.Parse(attrDict["unlock_id"]);

            string switchDaily = attrDict["switch_daily"];
            if (!switchDaily.CustomIsEmpty()) {
                string[] dailyTaskTypes = switchDaily.CustomSplit(',');
                int length = dailyTaskTypes.Length;
                if (length > 0) {
                    this.dailyTaskTypes = new List<string>(length);
                    for (int i = 0; i < length; i++) {
                        this.dailyTaskTypes.Add(dailyTaskTypes[i]);
                    }
                }
            }

            this.pointsLimit = int.Parse(attrDict["points_limit"]);
            if (this.pointsLimit != 0) {
                this.otherRewardDict.Add("tile_limit", this.pointsLimit);
            }
            // target
            this.level = string.IsNullOrEmpty(attrDict["level"]) ? 0 : int.Parse(attrDict["level"]);
            this.amount = string.IsNullOrEmpty(attrDict["amount"]) ? 0 : int.Parse(attrDict["amount"]);

            if (this.type.CustomEquals(GameConst.SPECIAL_BUILDING_LEVEL)) {
                this.type = GameConst.BUILDING_LEVEL;
                this.isBuildingSpecial = true;
                //this.buildingName = this.buildingName.Split('_')[0];
            }
            if (this.type.CustomEquals(GameConst.STRONGHOLD_AMOUNT)) {
                this.type = GameConst.BUILDING_LEVEL;
                this.buildingType =
                this.buildingName = ElementName.stronghold;
                this.level = this.amount;
            }
            if (this.type == GameConst.BUILDING_LEVEL && this.level > 1) {
                this.type = GameConst.BUILDING_UPGRADE;
            }
            if (this.type == GameConst.RESOURCE_OCCUPY) {
                this.type = GameConst.RESOURCE_LEVEL;
            }
            if (this.type == GameConst.RESOURCE_LEVEL) {
                string typeName = attrDict["name"];
                if (typeName.CustomIsEmpty()) {
                    this.resourceLevelType = 0;
                } else {
                    this.resourceLevelType = (int)System.Enum.Parse(
                        typeof(ElementType),
                        GameHelper.LowerFirstCase(attrDict["name"])
                    );
                }
            }



            this.times = string.IsNullOrEmpty(attrDict["times"]) ? 0 : int.Parse(attrDict["times"]);
            this.rarity = string.IsNullOrEmpty(attrDict["rarity"]) ? 0 : int.Parse(attrDict["rarity"]);

            if (!attrDict["lumber"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["lumber"]))
                this.resourcesDict.Add(Resource.Lumber, int.Parse(attrDict["lumber"]));

            if (!attrDict["steel"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["steel"]))
                this.resourcesDict.Add(Resource.Steel, int.Parse(attrDict["steel"]));

            if (!attrDict["marble"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["marble"]))
                this.resourcesDict.Add(Resource.Marble, int.Parse(attrDict["marble"]));

            if (!attrDict["food"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["food"]))
                this.resourcesDict.Add(Resource.Food, int.Parse(attrDict["food"]));

            if (!attrDict["gold"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["gold"]))
                this.resourcesDict.Add(Resource.Gold, int.Parse(attrDict["gold"]));
            this.gem = string.IsNullOrEmpty(attrDict["gem"]) ? 0 : int.Parse(attrDict["gem"]);

            string heroesStr = attrDict["hero_fragments"];
            if (!string.IsNullOrEmpty(heroesStr) || this.id == "1") {
                this.resourcesDict.Add(Resource.Fragment, 1);
            }
        }

        public override string GetId() {
            return this.id;
        }

        static DramaConf() {
            ConfigureManager.Instance.LoadConfigure<DramaConf>();
        }

        public int GetTarget() {
            switch (type) {
                case GameConst.RESOURCE_LEVEL:
                case GameConst.RESOURCE_OCCUPY:
                    return this.times;
                case GameConst.BUILDING_LEVELUP_TIMES:
                case GameConst.BUILDING_UPGRADE:
                case GameConst.HERO_LEVELUP_TIMES:
                case GameConst.SPECIAL_BUILDING_LEVEL:
                case GameConst.BUILDING_LEVEL:
                case GameConst.HERO_LEVEL:
                    return this.level;
                case GameConst.RESOURCE_AMOUNT:
                case GameConst.STRONGHOLD_AMOUNT:
                    return this.amount;
                case GameConst.PVP_TIMES:
                case GameConst.ATTACK_NPCCITY:
                case GameConst.RECRUIT:
                    return this.times;
                case GameConst.TROOP_ADD_HERO:
                    return this.times;
                case GameConst.JOIN_ALLIANCE:
                case GameConst.ALLIANCE_CHAT:
                    return 1;
                default:
                    Debug.LogErrorf("No such type: {0}", type);
                    return 1;
            }
        }

        public int GetDramaUnlockValue() {
            switch (type) {
                case GameConst.BUILDING_UPGRADE:
                case GameConst.SPECIAL_BUILDING_LEVEL:
                case GameConst.BUILDING_LEVEL:
                    BuildModel buildModel = ModelManager.GetModelData<BuildModel>();
                    return buildModel.GetBuildLevelByName(this.buildingName);
                case GameConst.HERO_LEVEL:
                    HeroModel heroModel = ModelManager.GetModelData<HeroModel>();
                    Hero hero = heroModel.GetMaxLevelHero();
                    return hero != null ? hero.Level : 0;
                default:
                    return 0;
            }
        }

        public string GetTitle() {
            return LocalManager.GetValue("chapter_", this.chapter.ToString(), "_title");
        }

        public string GetDescription() {
            return LocalManager.GetValue("chapter_", this.chapter.ToString(), "_desc");
        }

        public string GetName() {
            return LocalManager.GetValue("chapter_task_", this.id, "_desc");
        }

        public List<int> GetUnlockDrama() {
            List<int> idList;
            if (unlockDict.TryGetValue(int.Parse(this.id), out idList)) {
                return idList;
            } else {
                return null;
            }
        }

        public static DramaConf GetConf(string id) {
            return ConfigureManager.GetConfById<DramaConf>(id);
        }

        public static string GetConfName(string id) {
            DramaConf conf = GetConf(id);
            return (conf != null) ? conf.GetName() : string.Empty;
        }

        public static DramaConf GetConfByFull(string index) {
            string[] indexArray = index.CustomSplit('_');
            string step = indexArray[indexArray.Length - 1];
            return DramaConf.GetConf(step);
        }

        public static DramaConf GetDramaByIndex(string index) {
            string[] indexArray = index.CustomSplit('_');
            return GetConf(indexArray[indexArray.Length - 1]);
        }

        public static void AfterRead() {
            unlockDict.Clear();
            foreach (BaseConf conf in ConfigureManager.GetConfDict<DramaConf>().Values) {
                DramaConf dramaConf = conf as DramaConf;
                if (dramaConf.unlockId != 0) {
                    List<int> idList;
                    if (unlockDict.TryGetValue(dramaConf.unlockId, out idList)) {
                        idList.Add(int.Parse(dramaConf.id));
                    } else {
                        unlockDict.Add(dramaConf.unlockId, new List<int> { int.Parse(dramaConf.id) });
                    }
                }
            }
        }
    }
}