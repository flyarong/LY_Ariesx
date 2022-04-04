using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Poukoute {
    public class FirstPayConf : BaseConf {
        public int level;
        public int threshold;
        public int food;
        public int lumber;
        public int marble;
        public int steel;
        public int gem;
        public int gold;

        public string imgUrl;
        public int bgType = 1;

        public string chest;
        public string hero;
        public int fragment;

        public Dictionary<Resource, int> resourceDict = new Dictionary<Resource, int>();
        public static int maxLevel = 0;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.level = int.Parse(attrDict["first_time_pay_id"]);
            if (this.level > maxLevel) {
                maxLevel = this.level;
            }
            this.threshold = int.Parse(attrDict["pay_amount"]);
            this.food = int.Parse(attrDict["food"]);
            this.lumber = int.Parse(attrDict["lumber"]);
            this.marble = int.Parse(attrDict["marble"]);
            this.steel = int.Parse(attrDict["steel"]);
            this.gem = int.Parse(attrDict["gem"]);
            this.gold = int.Parse(attrDict["gold"]);
            this.chest = attrDict["chest"];
            this.bgType = int.Parse(attrDict["type"]);
            if (!string.IsNullOrEmpty(attrDict["hero_fragments"])) {
                string[] heroFragment = attrDict["hero_fragments"].Split(',');
                this.hero = heroFragment[0];
                this.fragment = int.Parse(heroFragment[1]);
            }
            //  this.imgUrl = attrDict["image"];

            if (this.lumber != 0) {
                resourceDict.Add(Resource.Lumber, this.lumber);
            }
            if (this.steel != 0) {
                resourceDict.Add(Resource.Steel, this.steel);
            }
            if (this.marble != 0) {
                resourceDict.Add(Resource.Marble, this.marble);
            }
            if (this.food != 0) {
                resourceDict.Add(Resource.Food, this.food);
            }
            if (this.gold != 0) {
                resourceDict.Add(Resource.Gold, this.gold);
            }
            if (this.gem != 0) {
                resourceDict.Add(Resource.Gem, this.gem);
            }
            if (!string.IsNullOrEmpty(this.chest)) {
                resourceDict.Add(Resource.Chest, 1);
            }
        }

        public override string GetId() {
            return this.level.ToString();
        }

        static FirstPayConf() {
            ConfigureManager.Instance.LoadConfigure<FirstPayConf>();
        }

        public static FirstPayConf GetConf(string id) {
            return ConfigureManager.GetConfById<FirstPayConf>(id);
        }

        public static FirstPayConf GetCurrentLevel(int level) {
            return ConfigureManager.GetConfById<FirstPayConf>(level.ToString());
        }

        //public static 
    }
}
