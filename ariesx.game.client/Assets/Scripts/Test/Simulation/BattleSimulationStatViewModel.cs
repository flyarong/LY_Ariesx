using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.SceneManagement;

namespace Poukoute {
    public class BattleSimulationStatViewModel : MonoBehaviour {
        private BattleSimulationStatView view;
        private BattleSimulationModel model;

        /* Model data get set */
        public Battle.Report reportInfoStat = new Battle.Report();
        public Battle.PlayerInfo attackerInfoStat = new Battle.PlayerInfo();
        public Battle.PlayerInfo defenderInfoStat = new Battle.PlayerInfo();
        public Dictionary<string, Battle.Hero> attackerHeroes = new Dictionary<string, Battle.Hero>();
        public Dictionary<string, Battle.Hero> defenderHeroes = new Dictionary<string, Battle.Hero>();
        private Dictionary<string, List<int>> attHeroArmyAmountDic = new Dictionary<string, List<int>>();
        private Dictionary<string, List<int>> defHeroArmyAmountDic = new Dictionary<string, List<int>>();
        private Dictionary<string, Dictionary<string, int>> attackerSkillDic = new Dictionary<string, Dictionary<string, int>>();
        private Dictionary<string, Dictionary<string, int>> defenderSkillDic = new Dictionary<string, Dictionary<string, int>>();

        public Battle.Report BattleReport {
            get {
                return this.reportInfoStat;
            }
        }

        public bool NeedRefresh { get; set; }
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<BattleSimulationStatView>();
            this.model = BattleSimulationModel.Instance;
            this.NeedRefresh = true;
        }
        public void Show() {
            if (this.NeedRefresh) {
                this.RefreshData();
                this.GetBattleStatData();
                this.view.Show();
                this.NeedRefresh = false;
            }
        }

        public void Hide() {
            this.view.Hide();
        }

        public class StatInfo {
            public int winner;
            public int attack;
            public int skill;
            public int hp;
            public int maxArmyAmount;
            public int roundSum;
        }
        public StatInfo attStatInfo = new StatInfo();
        public StatInfo defStatInfo = new StatInfo();
        public int battleCount = 0;
        private void GetBattleStatData() {
            for (int i = 0; i < this.model.battleReportList.Count; i++) {
                this.reportInfoStat = this.model.battleReportList[i].Report;
                this.attackerInfoStat = this.reportInfoStat.Attacker;
                this.defenderInfoStat = this.reportInfoStat.Defender;
                this.GetBattleStatArmyAmount(this.attackerInfoStat, this.attHeroArmyAmountDic);
                this.GetBattleStatArmyAmount(this.defenderInfoStat, this.defHeroArmyAmountDic);

                this.SetHeroDict(this.attackerHeroes, this.attackerInfoStat.BeforeHeroes);
                this.SetHeroDict(this.defenderHeroes, this.defenderInfoStat.BeforeHeroes);

                if (this.reportInfoStat.Winner.CustomEquals("attacker")) this.attStatInfo.winner++;
                else if (this.reportInfoStat.Winner.CustomEquals("defender")) this.defStatInfo.winner++;
            }
            this.attStatInfo.roundSum = this.GetRound(this.model.battleReportRounds);
            this.defStatInfo.roundSum = this.GetRound(this.model.battleReportRounds);

            this.reportInfoStat.Attacker = this.SetPlayerInfoStat(this.attackerInfoStat, this.attHeroArmyAmountDic, this.attStatInfo);
            this.reportInfoStat.Defender = this.SetPlayerInfoStat(this.defenderInfoStat, this.defHeroArmyAmountDic, this.defStatInfo);

            Battle.Action action = new Battle.Action();
            for (int i = 0; i < this.model.battleReportRounds.Count; i++) {
                for (int j = 0; j < this.model.battleReportRounds[i].Rounds.Count; j++) {
                    for (int k = 0; k < this.model.battleReportRounds[i].Rounds[j].Actions.Count; k++) {
                        action = this.model.battleReportRounds[i].Rounds[j].Actions[k];
                        PrintAction(0, action, null);
                    }
                }
            }
        }

        private void RefreshData() {
            this.battleCount = 0;
            ClearStatInfo(this.attStatInfo);
            ClearStatInfo(this.defStatInfo);
            this.attackerHeroes.Clear();
            this.defenderHeroes.Clear();
            this.attHeroArmyAmountDic.Clear();
            this.defHeroArmyAmountDic.Clear();
            this.attackerSkillDic.Clear();
            this.defenderSkillDic.Clear();
            this.battleCount = this.model.battleReportList.Count;
        }

        private void ClearStatInfo(StatInfo statInfo) {
            statInfo.winner = 0;
            statInfo.hp = 0;
            statInfo.skill = 0;
            statInfo.attack = 0;
            statInfo.maxArmyAmount = 0;
        }

        private void GetBattleStatArmyAmount(Battle.PlayerInfo playerInfo, Dictionary<string, List<int>> heroArmyAmountDic) {
            Battle.Hero hero = new Battle.Hero();
            for (int k = 0; k < playerInfo.AfterHeroes.Count; k++) {
                hero = playerInfo.AfterHeroes[k];
                if (hero.ArmyAmount < 0) hero.ArmyAmount = 0;
                if (!heroArmyAmountDic.ContainsKey(hero.Name))
                    heroArmyAmountDic.Add(hero.Name, new List<int>());
                heroArmyAmountDic[hero.Name].Add(hero.ArmyAmount);
            }
        }

        public void PrintAction(int offset, Battle.Action action, Battle.Action parent) {
            string description = string.Empty;
            bool isAttacker = false;
            isAttacker = this.GetHeroRelation(action.HeroId);
            switch (action.Type) {
                case SkillConst.TypeAttack:
                case SkillConst.TypeCastSkill:
                    if (isAttacker) {
                        SetHeroSkills(this.attackerSkillDic, action);
                    }
                    else {
                        SetHeroSkills(this.defenderSkillDic, action);
                    }
                    break;
                case SkillConst.TypeBuffActing:
                case SkillConst.TypeDelaySkill:
                case SkillConst.TypeImmune:
                case SkillConst.TypeBuffLose:
                case SkillConst.TypeGetBuff:
                case SkillConst.TypeDisableAttack:
                case SkillConst.TypeDisableSkill:
                case SkillConst.TypeStuned:
                case SkillConst.TypeDispel:
                case SkillConst.TypeHeal:
                case SkillConst.TypeInjured:
                case SkillConst.TypeReflectAttack:
                case SkillConst.TypeIntervene:
                case SkillConst.TypeShieldInjured:
                case SkillConst.TypeTakeEffect:
                case SkillConst.TypeDead:
                    break;
                default:
                    Debug.LogErrorf("No such type {0} in battle.", action.Type);
                    break;
            }

            if (!description.CustomIsEmpty()) {
                offset++;
            }

            foreach (Battle.Action child in action.Actions) {
                this.PrintAction(offset, child, action);
            }
        }

        private void SetHeroSkills(Dictionary<string, Dictionary<string, int>> heroSkill, Battle.Action action) {
            string heroId = this.GetHeroId(action.HeroId);
            if (!heroSkill.ContainsKey(heroId)) {
                heroSkill.Add(heroId, new Dictionary<string, int>());
                if (!heroSkill[heroId].ContainsKey(action.Name)) {
                    heroSkill[heroId].Add(action.Name, 1);
                }
            }
            else {
                if (!heroSkill[heroId].ContainsKey(action.Name)) {
                    heroSkill[heroId].Add(action.Name, 1);
                }
                else {
                    heroSkill[heroId][action.Name]++;
                }
            }
        }

        private string GetHeroId(string id) {
            Battle.Hero attackHero;
            Battle.Hero defenceHero;
            if (this.attackerHeroes.TryGetValue(id, out attackHero)) {
                return attackHero.GetId();
            }
            else if (this.defenderHeroes.TryGetValue(id, out defenceHero)) {
                return defenceHero.GetId();
            }
            else {
                Debug.LogError("No such hero");
                return string.Empty;
            }
        }

        private bool GetHeroRelation(string id) {
            if (this.attackerHeroes.ContainsKey(id)) {
                return true;
            }
            else if (this.defenderHeroes.ContainsKey(id)) {
                return false;
            }
            else {
                return true;
            }
        }

        private void SetHeroDict(Dictionary<string, Battle.Hero> heroDict, List<Battle.Hero> heroList) {
            for (int i = 0; i < heroList.Count; i++) {
                heroDict.Add(heroList[i].Id, heroList[i]);
            }
        }

        private Battle.PlayerInfo SetPlayerInfoStat(Battle.PlayerInfo playerInfo, Dictionary<string, List<int>> heroArmyAmountDic, StatInfo statInfo) {
            foreach (Battle.Hero afterHero in playerInfo.AfterHeroes) {
                statInfo.hp += afterHero.ArmyAmount = GetHeroArmyStat(afterHero, heroArmyAmountDic);
                statInfo.maxArmyAmount += this.GetHeroMaxArmyAmount(afterHero);
            }
            return playerInfo;
        }

        private int GetHeroMaxArmyAmount(Battle.Hero battleHero) {
            Hero hero = new Hero {
                Name = battleHero.Name,
                Level = battleHero.Level,
                ArmyAmount = battleHero.ArmyAmount,
                armyCoeff = GameHelper.NearlyEqual(battleHero.ArmyAmountBonus, 0d, 0.001d) ? 1 : battleHero.ArmyAmountBonus
            };
            HeroAttributeConf heroAttributeConf = HeroAttributeConf.GetConf(hero.GetId());
            int maxArmyAmount = heroAttributeConf != null ?
                heroAttributeConf.GetAttribute(hero.Level, HeroAttribute.ArmyAmount, hero.armyCoeff) : 0;
            return maxArmyAmount;
        }

        private int GetHeroArmyStat(Battle.Hero hero, Dictionary<string, List<int>> heroArmyAmountDic) {
            int sumArmy = 0;
            for (int i = 0; i < heroArmyAmountDic[hero.Name].Count; i++) {
                sumArmy += heroArmyAmountDic[hero.Name][i];
            }
            sumArmy /= heroArmyAmountDic[hero.Name].Count;
            return sumArmy;
        }

        public struct HeroStat {
            public int Winner;
            public int MinHp;
            public int MaxHp;
        }


        public string GetHeroStatInfo(string heroId, bool isAttacker) {
            return isAttacker ?
                GetHeroStatInfo(heroId, this.attackerSkillDic, this.attHeroArmyAmountDic, this.attStatInfo) :
                GetHeroStatInfo(heroId, this.defenderSkillDic, this.defHeroArmyAmountDic, this.defStatInfo);
        }

        public string GetHeroStatInfo(string heroId, Dictionary<string, Dictionary<string, int>> heroSkillDic,
            Dictionary<string, List<int>> heroArmyAmountDic, StatInfo statInfo) {
            HeroStat heroStat = new HeroStat();
            heroId = this.GetHeroId(heroId);
            int attackNum = 0;
            string str = null;
            if (heroSkillDic.ContainsKey(heroId)) {
                foreach (var kv in heroSkillDic[heroId]) {
                    if (kv.Key.CustomIsEmpty()) {
                        attackNum = kv.Value;
                        statInfo.attack += kv.Value;
                    }
                    else {
                        str = string.Format("{0} <color=#0000FFFF>{1}</color>:<color=#FF00FFFF>{2}</color>", str, kv.Key.Substring(kv.Key.Length - 1), kv.Value);
                        statInfo.skill += kv.Value;
                    }
                }
            }
            heroStat.Winner = 0;
            for (int i = 0; i < heroArmyAmountDic[heroId].Count; i++) {
                if (i == 0)
                    heroStat.MinHp = heroStat.MaxHp = heroArmyAmountDic[heroId][0];
                else {
                    if (heroStat.MinHp >= heroArmyAmountDic[heroId][i])
                        heroStat.MinHp = heroArmyAmountDic[heroId][i];
                    if (heroStat.MaxHp <= heroArmyAmountDic[heroId][i])
                        heroStat.MaxHp = heroArmyAmountDic[heroId][i];
                }
                if (heroArmyAmountDic[heroId][i] > 0) heroStat.Winner++;
            }
            str = string.Format(
                "<color=#FF0000FF>攻击:</color> {5}\t" +
                "<color=#FF0000FF>技能:</color> {0}\n" +
                "<color=#FF0000FF>存活:</color> <color=#00FF00FF>活 </color> {1} <color=#FF00FFFF>死 </color>{2}\n" +
                "<color=#FF0000FF>剩余血量:</color> <color=#00FF00FF>最大</color> {3} <color=#FF00FFFF>最小</color>{4}",
                str, heroStat.Winner, this.battleCount - heroStat.Winner, heroStat.MaxHp, heroStat.MinHp, attackNum);
            return str;
        }

        public string GetStatInfo(bool isAttacker) {
            return isAttacker ? GetStatInfo(this.attStatInfo) : GetStatInfo(this.defStatInfo);
        }

        private string GetStatInfo(StatInfo statInfo) {
            return string.Format(
                   "<color=#FF0000FF>攻击:</color> {0}\t" +
                   "<color=#FF0000FF>技能:</color> {1}\n" +
                   "<color=#FF0000FF>胜率:</color> {2:P}\t<color=#00FF00FF>胜</color>{3} <color=#FF00FFFF>败</color>{4}\n" +
                   //"<color=#FF0000FF>血量:</color> <color=#FF00FFFF>剩余总血量{5}</color> <color=#00FF00FF>总血量</color>{6}\n" +
                   "<color=#FF0000FF>血量比:</color> {5:P}\n" +
                   "<color=#FF0000FF>平均回合:</color> {6}\n",
                   statInfo.attack, statInfo.skill,
                   statInfo.winner * 1.0 / this.battleCount, statInfo.winner, this.battleCount - statInfo.winner,
                    //statInfo.hp * this.battleCount,statInfo.maxArmyAmount * this.battleCount,
                    statInfo.hp * 1.0 / statInfo.maxArmyAmount,
                   statInfo.roundSum * 1.0f / this.battleCount
                   );
        }

        private int GetRound(Dictionary<int, Battle.ReportRounds> reportRounds) {
            int roundSum = 0;
            for (int i = 0; i < reportRounds.Count; i++) {
                roundSum += reportRounds[i].Rounds.Count;
            }
            return roundSum;
        }
    }
}
