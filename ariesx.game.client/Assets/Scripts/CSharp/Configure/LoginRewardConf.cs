using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
namespace Poukoute {
    public class LoginRewardConf: BaseConf {
        public static Dictionary<int, LoginRewardConf> AllLoginRewardDict =
            new Dictionary<int, LoginRewardConf>();
        public int day;
        public int gem;
        public int gold;
        public int lumber;
        public int marble;
        public int steel;
        public int food;
        public bool special = false;
        public string viewName;
        public string heroFragments;
        public string[] heroName;
        private static int totalDays;
        public Dictionary<Resource, int> resourceDict =
            new Dictionary<Resource, int>();
        public Dictionary<string, int> heroDict =
            new Dictionary<string, int>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.day = int.Parse(attrDict["day"]);
            this.gem = int.Parse(attrDict["gem"]);
            this.gold = int.Parse(attrDict["gold"]);
            this.special = int.Parse(attrDict["special"]) == 1;
            this.viewName = attrDict["view"];
            if (!attrDict["lumber"].CustomEquals("0"))
                this.resourceDict.Add(Resource.Lumber, int.Parse(attrDict["lumber"]));
            if (!attrDict["marble"].CustomEquals("0"))
                this.resourceDict.Add(Resource.Marble, int.Parse(attrDict["marble"]));
            if (!attrDict["steel"].CustomEquals("0"))
                this.resourceDict.Add(Resource.Steel, int.Parse(attrDict["steel"]));
            if (!attrDict["food"].CustomEquals("0"))
                this.resourceDict.Add(Resource.Food, int.Parse(attrDict["food"]));
            if (!attrDict["gem"].CustomEquals("0"))
                this.resourceDict.Add(Resource.Gem, int.Parse(attrDict["gem"]));
            if (!attrDict["gold"].CustomEquals("0"))
                this.resourceDict.Add(Resource.Gold, int.Parse(attrDict["gold"]));

            this.heroFragments = attrDict["hero_fragments"];
            this.heroName = heroFragments.Split(',');
            if (this.heroName[0] != "") {
                this.heroDict.Add(this.heroName[0], int.Parse(this.heroName[1]));
            } else {
                this.heroDict.Add(string.Empty, 0);
            }
        }

        private static void GetAllLoginReward() {
            int day = 1;
            LoginRewardConf loginRewardConf = LoginRewardConf.GetConf(day.ToString());
            while (loginRewardConf != null) {
                LoginRewardConf.AllLoginRewardDict.Add(day, loginRewardConf);
                ++day;
                loginRewardConf = LoginRewardConf.GetConf(day.ToString());
            }
        }

        public static int GetAllLoginRewardDey() {
            if (LoginRewardConf.AllLoginRewardDict.Count < 1) {
                LoginRewardConf.GetAllLoginReward();
            }
            return LoginRewardConf.AllLoginRewardDict.Count;
        }

        public override string GetId() {
            return this.day.ToString();
        }

        static LoginRewardConf() {
                ConfigureManager.Instance.LoadConfigure<LoginRewardConf>();
        }

        public static LoginRewardConf GetConf(string id) {
            return ConfigureManager.GetConfById<LoginRewardConf>(id);
        }

    }
}
