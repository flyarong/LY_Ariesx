using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class ChapterConf : BaseConf {
        public int chapter;
        public int gem;
        public Dictionary<Resource, int> resourcesDict = new Dictionary<Resource, int>();
        public Dictionary<string, int> heroDict = new Dictionary<string, int>();

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.chapter = int.Parse(attrDict["chapter"]);
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

            if (!attrDict["crystal"].CustomEquals("0") && !string.IsNullOrEmpty(attrDict["crystal"]))
                this.resourcesDict.Add(Resource.Crystal, int.Parse(attrDict["crystal"]));
            this.gem = string.IsNullOrEmpty(attrDict["gem"]) ? 0 : int.Parse(attrDict["gem"]);

            string heroesStr = attrDict["hero_fragments"];
            if (!string.IsNullOrEmpty(heroesStr)) {
                string[] heroArray = heroesStr.CustomSplit(';');
                foreach (string heroStr in heroArray) {
                    string[] heroAttr = heroStr.CustomSplit(',');
                    this.heroDict.Add(heroAttr[0], int.Parse(heroAttr[1]));
                }
            }
        }

        public override string GetId() {
            return this.chapter.ToString();
        }

        static ChapterConf() {
            ConfigureManager.Instance.LoadConfigure<ChapterConf>();
        }

        public static ChapterConf GetConf(string id) {
            return ConfigureManager.GetConfById<ChapterConf>(id);
        }
    }
}
