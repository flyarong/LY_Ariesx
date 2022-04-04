using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public enum HeroAttribute {
        ArmyAmount,
        Attack,
        Defense,
        Speed,
        Siege,
        MarchingTime
    }
    public class HeroAttributeConf : BaseConf {
        public string name;
        public int rarity;
        public float rarityPercent;
        public int attack;
        public int defense;
        public int speed;
        public int marchingTime;
        public int troopAmount;
        public int siege;

        public string race;
        public string role;
        public string row;
        public string skills;

        public string RoleIcon {
            get {
                return "hero_role_icon_" + this.role;
            }
        }

        public string RarityBorder {
            get {
                return "hero_rarity_border_" + this.rarity;
            }
        }

        public override void SetProperty(Dictionary<string, string> attrDict) {
            this.name = attrDict["hero_id"];
            this.rarity = int.Parse(attrDict["rarity"]);
            this.rarityPercent = float.Parse(attrDict["rarity_percent"]);
            this.attack = int.Parse(attrDict["attack"]);
            this.defense = int.Parse(attrDict["defense"]);
            this.speed = int.Parse(attrDict["speed"]);
            this.marchingTime = int.Parse(attrDict["marching_time"]);
            this.troopAmount = int.Parse(attrDict["troop_amount"]);
            this.siege = int.Parse(attrDict["siege"]);

            this.race = attrDict["race"];
            this.role = attrDict["role"];
            this.row = attrDict["row"];
            this.skills = attrDict["skills"];
        }

        public override string GetId() {
            //return this.name + "_" + this.tiers;
            return this.name;
        }

        static HeroAttributeConf() {
            ConfigureManager.Instance.LoadConfigure<HeroAttributeConf>();
        }

        public int GetAttribute(int level, HeroAttribute attribute, float armyCoeff = 1,
            bool afterBonus = true) {
            float value = 0;
            bool isArmyAmount = false;
            switch (attribute) {
                case HeroAttribute.ArmyAmount:
                    //value = this.troopAmount * armyCoeff;
                    value = this.troopAmount;
                    isArmyAmount = true;
                    break;
                case HeroAttribute.Attack:
                    value = this.attack;
                    break;
                case HeroAttribute.Defense:
                    value = this.defense;
                    break;
                case HeroAttribute.Siege:
                    if (afterBonus) {
                        return this.siege + Mathf.RoundToInt(
                            siege * ModelManager.GetModelData<BuildModel>().GetSiegeBonus()
                        );
                    } else {
                        return this.siege;
                    }
                case HeroAttribute.Speed:
                    return this.speed;
                case HeroAttribute.MarchingTime:
                    return this.marchingTime;
                default:
                    return 0;
            }
            float levelUpPercent = HeroLevelConf.GetHeroLevelUpPercent(this.rarity, level);
            float result = value * (1 + levelUpPercent + this.rarityPercent);
            result = (isArmyAmount ? result * armyCoeff : result);
            if (afterBonus) {
                if (attribute == HeroAttribute.Attack) {
                    return Mathf.CeilToInt(result - 0.01f) + ModelManager.GetModelData<BuildModel>().GetAttackBonus();
                } else if (attribute == HeroAttribute.Defense) {
                    return Mathf.CeilToInt(result - 0.01f) + ModelManager.GetModelData<BuildModel>().GetDefenceBonus();
                }
            }
            return Mathf.CeilToInt(result - 0.01f);
        }

        public static int GetHeroArmyAmount(string heroName, int level, float armyCoeff) {
            return HeroAttributeConf.GetConf(heroName).GetAttribute(
                level, HeroAttribute.ArmyAmount, armyCoeff
            );
        }

        public static int GetBossArmyAmount(string heroName, int level) {
            return HeroAttributeConf.GetConf(heroName).GetAttribute(level, HeroAttribute.ArmyAmount);
        }

        public static float GetMarchTime(float speed, float distance) {
            float unit = 0;
            if (speed <= 30) {
                unit = 30;
            } else if (speed <= 80) {
                unit = 20;
            } else {
                unit = 10;
            }
            return distance * unit;
        }

        public static float GetMarchTime(string heroName, float distance) {
            return distance * HeroAttributeConf.GetConf(heroName).marchingTime;
        }

        public static HeroAttributeConf GetConf(string id) {
            return ConfigureManager.GetConfById<HeroAttributeConf>(id);
        }

        public static string GetLocalName(string heroName) {
            return LocalManager.GetValue(heroName);
        }

        public static string GetSkillName(string heroName) {
            return LocalManager.GetValue(heroName);
        }

        public static string GetSpeed(float speed) {
            if (speed < 30) {
                return LocalManager.GetValue(LocalHashConst.troop_speed_slow);
            } else if (speed < 80) {
                return LocalManager.GetValue(LocalHashConst.troop_speed_normal);
            } else {
                return LocalManager.GetValue(LocalHashConst.troop_speed_fast);
            }
        }



        public int GetPower(int level) {
            int attack = this.GetAttribute(level, HeroAttribute.Attack);
            int defense = this.GetAttribute(level, HeroAttribute.Defense);
            int troopAmount = this.GetAttribute(level, HeroAttribute.ArmyAmount);
            int siege = this.GetAttribute(level, HeroAttribute.Siege);
            int speed = this.GetAttribute(level, HeroAttribute.Speed);

            return Mathf.CeilToInt(attack + defense + troopAmount +
                siege + speed / 100f * attack - 0.01f);
        }

        public int GetAttack(int level) {
            return this.GetAttribute(level, HeroAttribute.Attack);
        }

        public int GetDefense(int level) {
            return this.GetAttribute(level, HeroAttribute.Defense);
        }

        public static int GetPower(string id, int level) {
            return HeroAttributeConf.GetConf(id).GetPower(level);
        }

        public static int GetDefense(string id, int level) {
            return HeroAttributeConf.GetConf(id).GetDefense(level);
        }

        public static int GetAttack(string id, int level) {
            return HeroAttributeConf.GetConf(id).GetAttack(level);
        }

        public static int GetHeroesCount() {
            return ConfigureManager.GetConfDict<HeroAttributeConf>().Count;
        }


        #region DemonShadow boss logic

        /// <summary>
        /// 得到魔影入侵Boss队伍总战斗力
        /// </summary>
        /// <param name="bossTroop"></param>
        /// <returns></returns>
        public static int GetBossPower(BossTroop bossTroop) {
            int bossCount = bossTroop.Heroes.Count;
            int maxPower = 0;
            for (int i = 0; i < bossCount; i++) {
                maxPower += GetPower(bossTroop.Heroes[i].Name, bossTroop.Heroes[i].Level);
            }
            return maxPower;
        }

        /// <summary>
        /// 得到魔影入侵Boss队伍的最大血量
        /// </summary>
        /// <param name="bossTroop"></param>
        /// <returns></returns>
        public static int GetBossMaxHP(BossTroop bossTroop) {
            int bossCount = bossTroop.Heroes.Count;
            int maxArmy = 0;
            for (int i = 0; i < bossCount; i++) {
                maxArmy += HeroAttributeConf.GetBossArmyAmount(
                    bossTroop.Heroes[i].Name, bossTroop.Heroes[i].Level);
            }
            return maxArmy;
        }

        /// <summary>
        /// Get This Boss Troop ArmyAmount
        /// </summary>
        /// <param name="bossTroop"></param>
        /// <returns></returns>
        public static int GetBossTroopThisHP(BossTroop bossTroop) {
            int bossCount = bossTroop.Heroes.Count;
            int bossThisHp = 0;
            for (int i = 0; i < bossCount; i++) {
                bossThisHp += bossTroop.Heroes[i].ArmyAmount;
            }
            return bossThisHp;
        }


        /// <summary>
        /// Get Boss Name
        /// </summary>
        /// <param name="boss"></param>
        /// <returns></returns>
        public static string GetBossName(BossTroop boss) {
            int bossPositionList = boss.Positions.Count;
            for (int i = 0; i < bossPositionList; i++) {
                if (boss.Positions[i].Position == 1) {
                    return boss.Positions[i].Name;
                }
            }
            return string.Empty;
        }

        public static int GetBossThisHP(BossTroop boss) {
            int bossCount = boss.Heroes.Count;
            string bossName = HeroAttributeConf.GetBossName(boss);
            for (int i = 0; i < bossCount; i++) {
                if (boss.Heroes[i].Name == bossName) {
                    return boss.Heroes[i].ArmyAmount;
                }
            }
            return 0;
        }

        public static int GetBossLevelInfo(BossTroop boss) {
            int bossList = boss.Heroes.Count;
            string bossName = HeroAttributeConf.GetBossName(boss);
            for (int i = 0; i < bossList; i++) {
                if (boss.Heroes[i].Name == bossName) {
                    return boss.Heroes[i].Level;
                }
            }
            return 0;
        }

        #endregion



    }
}
