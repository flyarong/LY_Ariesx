using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public enum HeroRarity {
        normal = 1,
        excellent,
        superior,
        epic,
        legend
    }

    public class HeroLevelConf : BaseConf {
        public int rarity;
        public int level;
        public float levelUpPercent;
        public int fragments;
        public int levelGold;
        // to do : when change hero_level.csv, this data below must edit too.
        public static Dictionary<HeroRarity, int> HeroDiffRarityMaxLevel =
                            new Dictionary<HeroRarity, int>(){
                                    { HeroRarity.normal,    75},
                                    { HeroRarity.excellent, 65},
                                    { HeroRarity.superior,  60},
                                    { HeroRarity.epic,      45},
                                    { HeroRarity.legend,    30}};

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.rarity = int.Parse(attrDict["rarity"]);
            this.level = int.Parse(attrDict["level"]);
            this.levelUpPercent = float.Parse(attrDict["level_up_percent"]);
            this.fragments = int.Parse(attrDict["fragments"]);
            this.levelGold = int.Parse(attrDict["level_gold"]);
        }

        public override string GetId() {
            return string.Concat(this.rarity, "_", this.level);
        }

        static HeroLevelConf() {
            ConfigureManager.Instance.LoadConfigure<HeroLevelConf>();
        }

        public static HeroLevelConf GetConf(string id) {
            return ConfigureManager.GetConfById<HeroLevelConf>(id);
        }

        public static bool GetHeroReachMaxLevel(Hero hero) {
            return HeroLevelConf.GetHeroReachMaxLevel(hero.Name, hero.Level);
        }

        public static bool GetHeroReachMaxLevel(string heroName, int heroLevel) {
            return heroLevel >= HeroLevelConf.GetHeroMaxLevel(heroName);
        }

        private static int GetHeroMaxLevel(string heroName) {
            HeroAttributeConf heroConf = HeroAttributeConf.GetConf(heroName);
            return HeroLevelConf.GetHeroMaxLevel(heroConf.rarity);
        }

        public static int GetHeroMaxLevel(int rarity) {
            return HeroLevelConf.HeroDiffRarityMaxLevel[(HeroRarity)rarity];
        }

        public static int GetHeroFragments(int level, HeroAttributeConf heroConf) {
            return GetHeroFragments(heroConf.rarity, level);
        }

        public static int GetHeroUpgradCost(Hero hero) {
            HeroAttributeConf heroConf = HeroAttributeConf.GetConf(hero.GetId());
            return GetHeroLevelUpGold(heroConf.rarity, hero.Level);
        }

        public static int GetHeroUpgradFragments(Hero hero) {
            return GetHeroUpgradFragments(hero.Name, hero.Level);
        }

        public static int GetHeroUpgradFragments(string heroName, int heroLevel) {
            HeroAttributeConf heroConf = HeroAttributeConf.GetConf(heroName);
            return GetHeroFragments(heroConf.rarity, heroLevel);
        }

        public static int GetHeroFragments(int rarity, int level) {
            if (level < 1 || level > HeroLevelConf.GetHeroMaxLevel(rarity)) {
                return 0;
            }
            string heroLevelId = string.Format("{0}_{1}", rarity, level);
            HeroLevelConf heroLevelConf = HeroLevelConf.GetConf(heroLevelId);
            return heroLevelConf.fragments;
        }

        public static float GetHeroLevelUpPercent(Hero hero) {
            HeroAttributeConf heroConf = HeroAttributeConf.GetConf(hero.GetId());
            return GetHeroLevelUpPercent(heroConf.rarity, hero.Level);
        }

        public static float GetHeroLevelUpPercent(int rarity, int level) {
            if (level < 1 || level > HeroLevelConf.GetHeroMaxLevel(rarity)) {
                return 0;
            }
            string heroLevelId = string.Format("{0}_{1}", rarity, level);
            HeroLevelConf heroLevelConf = HeroLevelConf.GetConf(heroLevelId);
            return heroLevelConf.levelUpPercent;
        }

        public static bool GetCanLevelUp(Hero hero) {
            return !GetHeroReachMaxLevel(hero) &&
                hero.FragmentCount >= HeroLevelConf.GetHeroUpgradFragments(hero);
        }

        private static int GetHeroLevelUpGold(int rarity, int level) {
            if (level < 1 || level > HeroLevelConf.GetHeroMaxLevel(rarity)) {
                return 0;
            }
            string heroLevelId = string.Format("{0}_{1}", rarity, level);
            HeroLevelConf heroLevelConf = HeroLevelConf.GetConf(heroLevelId);
            return heroLevelConf.levelGold;
        }
    }
}