using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using System.Text;

namespace Poukoute {
    public class NPCCityConf : BaseConf {
        public string id;
        public int level;
        public string type;
        public string name;
        public string race;
        public string allianceName;
        public bool isCenter;
        public int durability;
        public string size;
        public int mapSn;
        public int zoneSn;
        public string limitGem;
        public long limitTime;

        public int tributeGoldBuff;
        public float tributeGoldBuffBonus;
        public int allianceTributeGoldBuff;
        public Dictionary<Resource, int> resourceBuff = new Dictionary<Resource, int>();
        public Dictionary<Resource, int> rewardDict = new Dictionary<Resource, int>();
        private static List<NPCCityConf> mainCityList =
            new List<NPCCityConf>();
        //private static Dictionary<int, List<NPCCityConf>> mainCityDict =
        //    new Dictionary<int, List<NPCCityConf>>();
        private static Dictionary<int, List<NPCCityConf>> allCity =
            new Dictionary<int, List<NPCCityConf>>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.isCenter = false;
            this.id = attrDict["id"];
            this.level = int.Parse(attrDict["level"]);
            this.type = attrDict["type"];
            this.name = attrDict["name"];
            this.race = attrDict["race"];
            this.size = attrDict["size"];
            this.durability = int.Parse(attrDict["durability"]);
            this.limitGem = attrDict["limit_gem"];
            this.limitTime = long.Parse(attrDict["limit_time"]);

            this.ParseBuff(Resource.Lumber, int.Parse(attrDict["lumber_buff"]));
            this.ParseBuff(Resource.Steel, int.Parse(attrDict["steel_buff"]));
            this.ParseBuff(Resource.Marble, int.Parse(attrDict["marble_buff"]));
            this.ParseBuff(Resource.Food, int.Parse(attrDict["food_buff"]));

            this.ParseResource(Resource.Lumber, int.Parse(attrDict["lumber"]));
            this.ParseResource(Resource.Steel, int.Parse(attrDict["steel"]));
            this.ParseResource(Resource.Marble, int.Parse(attrDict["marble"]));
            this.ParseResource(Resource.Food, int.Parse(attrDict["food"]));
            this.ParseResource(Resource.Gold, int.Parse(attrDict["gold"]));

            this.tributeGoldBuff = int.Parse(attrDict["tribute_gold_buff"]);
            this.tributeGoldBuffBonus = float.Parse(attrDict["tribute_gold_buff_bonus"]);
            this.allianceTributeGoldBuff = int.Parse(attrDict["alliance_tribute_gold_buff"]);

            string[] idArray = this.id.CustomSplit(',');

            this.mapSn = int.Parse(idArray[0]);
            this.zoneSn = int.Parse(idArray[1]);
            this.isCenter = idArray[3].CustomEquals("1");
            if ((this.type.CustomEquals(GameConst.REGION_CAPITAL) ||
                this.type.CustomEquals(GameConst.CAPITAL)) &&
                this.isCenter) {
                mainCityList.Add(this);
            }

            if (this.isCenter) {
                List<NPCCityConf> cityList;
                if (!NPCCityConf.allCity.TryGetValue(this.mapSn, out cityList)) {
                    cityList = new List<NPCCityConf>();
                    NPCCityConf.allCity.Add(this.mapSn, cityList);
                }
                if (!cityList.Contains(this)) {
                    cityList.Add(this);
                }
            }
        }

        public override string GetId() {
            return this.id;
        }

        static NPCCityConf() {
            ConfigureManager.Instance.LoadConfigure<NPCCityConf>();

        }

        private void ParseBuff(Resource resource, int value) {
            if (value != 0) {
                resourceBuff.Add(resource, value);
            }
        }

        private void ParseResource(Resource resource, int value) {
            if (value != 0) {
                rewardDict.Add(resource, value);
            }
        }

        public static List<NPCCityConf> GetMainCityList() {
            return NPCCityConf.mainCityList;
        }

        public static List<NPCCityConf> GetCityIn(int sn) {
            if (sn == 0) {
                return NPCCityConf.mainCityList;
            }

            List<NPCCityConf> npcCityConf;
            if (!NPCCityConf.allCity.TryGetValue(sn, out npcCityConf)) {
                npcCityConf = new List<NPCCityConf>();
            }
            return npcCityConf;
        }

        public static Vector2 GetNearestNPCCityCoord(Vector2 startCoord, int mapSN) {
            Vector2 npcCityCoord = Vector2.zero;
            List<NPCCityConf> cityList = NPCCityConf.GetCityIn(mapSN);
            int cityListCount = cityList.Count;
            NPCCityConf npcCityConf = null;
            MiniMapCityConf cityConf = null;
            float distance = 0;
            float mixDistance = 0;
            for (int i = 0; i < cityListCount; i++) {
                npcCityConf = cityList[i];
                cityConf = MiniMapCityConf.GetConf(npcCityConf.id);
                if (cityConf != null) {
                    distance = Vector2.Distance(startCoord, cityConf.coordinate);
                    if (mixDistance < distance) {
                        mixDistance = distance;
                        npcCityCoord = cityConf.coordinate;
                    }
                }
            }

            return npcCityCoord;
        }

        public string GetNpcCityMapZoneLocal() {
            return string.Concat(GetMapSNLocalName(this.mapSn), ",",
                GetZoneSnLocalName(this.mapSn, this.zoneSn));
        }

        public static string GetNpcCityMapZoneLocal(string id) {
            NPCCityConf cityConf = NPCCityConf.GetConf(id);
            return cityConf.GetNpcCityMapZoneLocal();
        }

        //public static string GetNpcCityLocalName(string npcName) {
        //    int subIndex = npcName.LastIndexOf("_");
        //    return LocalManager.GetValue(npcName.Substring(0, subIndex));
        //}

        public static string GetNpcCityLocalName(string npcName, bool isCenter) {
            int subIndex = npcName.LastIndexOf("_");
            string cityName = LocalManager.GetValue(npcName.Substring(0, subIndex));
            if (!isCenter) {
                cityName = string.Format(
                    LocalManager.GetValue(LocalHashConst.npc_city_outside), cityName);
            }
            return cityName;
        }

        public static string GetMapSNLocalName(int mapSN) {
            return LocalManager.GetValue("map_", mapSN.ToString());
        }

        public static string GetMapSNLocalDesc(int mapSN) {
            return LocalManager.GetValue("map_desc_", mapSN.ToString());
        }

        public static string GetZoneSnLocalName(int mapSN, int zoneSN) {
            return LocalManager.GetValue("map_", mapSN, "_zone_", zoneSN);
        }

        public static NPCCityConf GetConf(string id) {
            return ConfigureManager.GetConfById<NPCCityConf>(id);
        }

        public static string GetCityKey(PointInfo point) {
            StringBuilder s = new StringBuilder();
            return s.AppendFormat("{0},{1},{2},{3}",
                point.MapSN, point.ZoneSN, point.NpcCity.SN, (point.NpcCity.IsCenter ? 1 : 0)).ToString();
        }

        public static NPCCityConf GetConf(Point point) {
            string id = GetCityKey(point);
            return ConfigureManager.GetConfById<NPCCityConf>(id);
        }

        public static NPCCityConf GetConf(PointInfo point) {
            string id = GetCityKey(point);
            return ConfigureManager.GetConfById<NPCCityConf>(id);
        }

        public static float GetDurability(string key) {
            NPCCityConf npcCityConf = NPCCityConf.GetConf(key);
            return npcCityConf.durability;
        }

        public static void AfterRead() {
            foreach (List<NPCCityConf> cityList in allCity.Values) {
                cityList.Sort((a, b) => { return -a.level.CompareTo(b.level); });
            }
        }

        public static void BeforeRead() {
            mainCityList.Clear();
            //allCity.Clear();
            //mainCityDict.Clear();
        }

        public static string GetCityKey(Point point) {
            StringBuilder s = new StringBuilder();
            return s.AppendFormat("{0},{1},{2},{3}",
                point.MapSN, point.ZoneSN, point.NpcCity.SN, (point.NpcCity.IsCenter ? 1 : 0)).ToString();
        }

        public static string GetCityKey(uint point) {
            int level = (int)(point & 255);
            int SN = GameConst.NPC_CITY_SN + level % GameConst.CITY_ENTRANCE_NS;
            StringBuilder s = new StringBuilder();
            return s.AppendFormat("{0},{1},{2},{3}",
                (point >> 24), ((point >> 16) & 255), SN, level / GameConst.CITY_CENTER).ToString();
        }

        public static string GetCityKey(FallenTarget target) {
            StringBuilder s = new StringBuilder();
            return s.AppendFormat("{0},{1},{2},1",
                target.MapSN, target.ZoneSN, target.SN).ToString();
        }

        public bool IsLimitRewardValid(out long remainTime) {
            if (this.isCenter) {
                long limitFinisAt = RoleManager.GetWorldOpenAt() + this.limitTime;
                remainTime = limitFinisAt * 1000 - RoleManager.GetCurrentUtcTime();
                return (remainTime > 0);
            } else {
                remainTime = 0;
                return false;
            }
        }
    }
}
