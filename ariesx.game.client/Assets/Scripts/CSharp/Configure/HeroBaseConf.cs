using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Poukoute {
    public class HeroBaseConf : BaseConf {
        public string heroId;
        public static List<string> HeroList {
            get {
                if (heroList.Count < 1) {
                    GetAllHeroId();
                }
                return heroList;
            }
        }
        private static List<string> heroList = new List<string>(30);

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.heroId = attrDict["hero_id"];
            //HeroList.Add(heroId);
        }

        public override string GetId() {
            return heroId;
        }

        static HeroBaseConf() {
            ConfigureManager.Instance.LoadConfigure<HeroBaseConf>();
        }

        public static void Clear() {
            heroList.Clear();
        }

        private static void GetAllHeroId() {
            Dictionary<string, BaseConf> heroBaseConf =
                ConfigureManager.GetConfDict<HeroBaseConf>();
            if (heroBaseConf == null) {
                Debug.LogError("heroBaseConf is null");
            }
            foreach (var conf in heroBaseConf) {
                heroList.Add((conf.Value as HeroBaseConf).heroId);
            }
        }
    }
}
