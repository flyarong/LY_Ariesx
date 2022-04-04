using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public struct GachaHero {
        public int minFragment;
        public int maxFragment;
        public string[] heroes;
    }


    public struct SpecialHero {
        public int fragment;
        public int weight;
        public string heroName;
    }

    public enum HeroQuality {
        T1,
        T2,
        T3,
        T4,
        T5
    }

    public class GachaGroupConf : BaseConf {
        private static Dictionary<string, BaseConf> allGachaConf = null;

        public string chest;
        public int price;
        public string material;

        public List<string> gachaAllHeroes = new List<string>(50);
        public List<SpecialHero> gachaSpecialHeroes = new List<SpecialHero>(30);
        public List<GachaHero> gachaHeroes = new List<GachaHero>(30);
        public List<string> diffQualityNum = new List<string>(5);
        public List<string> joinAllHeroes = new List<string>();
        public List<GachaHero> joinHeroes = new List<GachaHero>();
        private readonly int CARDS_TYPE_NUM = 5;

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.chest = attrDict["chest"];
            this.price = int.Parse(attrDict["price"]);
            this.material = this.chest.CustomSplit('_')[0];
            string cardNum;
            for (int i = 1; i <= CARDS_TYPE_NUM; i++) {
                cardNum = attrDict[string.Concat("T", i)];
                if (!cardNum.CustomEquals("0~0")) {
                    this.diffQualityNum.Add(
                        string.Format("T{0},{1}", i, cardNum));
                }
            }

            string heroes = attrDict["heroes"];
            string[] results = heroes.CustomSplit('|');
            int count = results.Length;
            for (int i = 0; i < count; i++) {
                string[] heroInfo = results[i].CustomSplit(';');
                GachaHero containDetail = new GachaHero() {
                    minFragment = int.Parse(heroInfo[0]),
                    maxFragment = int.Parse(heroInfo[1]),
                    heroes = heroInfo[2].CustomSplit(',')
                };
                this.gachaHeroes.Add(containDetail);
                this.gachaAllHeroes.AddRange(containDetail.heroes);
            }

            string joinHeroes = attrDict["join_heroes"];
            string[] joinResults = joinHeroes.CustomSplit('|');
            int joinCount = joinResults.Length;
            for (int i = 0; i < joinCount; i++) {
                string[] joinHeroInfo = joinResults[i].CustomSplit(';');
                if (string.IsNullOrEmpty(joinHeroInfo[0])) {
                    continue;
                }
                GachaHero containDetail = new GachaHero() {
                    minFragment = int.Parse(joinHeroInfo[0]),
                    maxFragment = int.Parse(joinHeroInfo[1]),
                    heroes = joinHeroInfo[2].CustomSplit(',')
                };
                this.joinHeroes.Add(containDetail);
                this.joinAllHeroes.AddRange(containDetail.heroes);
            }

            string specialHeroes = attrDict["special_heroes"];
            if (!specialHeroes.CustomIsEmpty()) {
                results = specialHeroes.CustomSplit(';');
                count = results.Length;
                for (int i = 0; i < count; i++) {
                    string[] heroInfo = results[i].CustomSplit(',');
                    SpecialHero containDetail = new SpecialHero() {
                        heroName = heroInfo[0],
                        weight = int.Parse(heroInfo[1]),
                        fragment = int.Parse(heroInfo[2])
                    };
                    this.gachaSpecialHeroes.Add(containDetail);
                    this.gachaAllHeroes.Add(containDetail.heroName);
                }
            }
        }

        public override string GetId() {
            return this.chest;
        }

        static GachaGroupConf() {
            ConfigureManager.Instance.LoadConfigure<GachaGroupConf>();
        }

        public static GachaGroupConf GetConf(string id) {
            return ConfigureManager.GetConfById<GachaGroupConf>(id);
        }

        public static int GetUnlockChestIndex(string heroName) {
            if (allGachaConf == null) {
                allGachaConf =
                    ConfigureManager.GetConfDict<GachaGroupConf>();
            }

            foreach (GachaGroupConf gacha in allGachaConf.Values) {
                foreach (SpecialHero specialHero in gacha.gachaSpecialHeroes) {
                    if (heroName.CustomEquals(specialHero.heroName)) {
                        return gacha.chest.GetNumber();
                    }
                }

                foreach (GachaHero gachaHero in gacha.gachaHeroes) {
                    int heroesCount = gachaHero.heroes.Length;
                    for (int i = 0; i < heroesCount; i++) {
                        if (heroName.CustomEquals(gachaHero.heroes[i])) {
                            return gacha.chest.GetNumber();
                        }
                    }
                }
            }

            return -1;
        }
    }
}
