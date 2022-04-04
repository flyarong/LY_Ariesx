using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Protocol;
using TMPro;
using UnityEngine.Events;
using System.Text;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class BattleSimulationReportDetailView : MonoBehaviour {
        private BattleSimulationReportDetailViewPreference viewPref;
        private BattleSimulationReportDetailViewModel viewModel;

        private GameObject ui;

        private bool battleWin;
        public Dictionary<string, Battle.Hero> attackerHeroes =
                new Dictionary<string, Battle.Hero>();
        public Dictionary<string, Battle.Hero> defenderHeroes =
            new Dictionary<string, Battle.Hero>();
        private string roleName = string.Empty;
        private List<Battle.Round> battleReportRounds = new List<Battle.Round>(4);
        /*************************************************/

        protected void OnUIInit() {
            this.viewModel = this.GetComponent<BattleSimulationReportDetailViewModel>();
            this.ui = GameObject.Find("UI").transform.
                Find("UIDetailReport").gameObject;
            this.viewPref = this.ui.transform.GetComponent<BattleSimulationReportDetailViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
        }

        private void Awake() {
            OnUIInit();
        }

        public void PlayHide() {
            this.ui.gameObject.SetActive(false);
        }

        public void PlayShow(Action callback) {
            this.ui.gameObject.SetActive(true);
            callback.Invoke();
            CreateViewsHolder();
        }


        public void CreateViewsHolder() {
            int itemIndex;
            GameHelper.ClearChildren(this.viewPref.pnlRoundList);
            for (int i = 0; i < this.battleReportRounds.Count; i++) {
                itemIndex = i;
                if (this.battleReportRounds[itemIndex].Actions.Count == 0) continue;
                GameObject itemObj =
               PoolManager.GetObject(PrefabPath.pnlRound, this.viewPref.pnlRoundList);
                BattleReportRoundItemView itemView = itemObj.GetComponent<BattleReportRoundItemView>();
                CanvasGroup itemCG = itemObj.GetComponent<CanvasGroup>();
                itemCG.interactable = true;
                itemCG.alpha = 1;
                itemCG.blocksRaycasts = true;
                itemView.ItemIndex = itemIndex;
                this.OnItemContentChange(itemView,
                    this.battleReportRounds[itemIndex]);
            }
        }

        private void OnItemContentChange(BattleReportRoundItemView itemView, Battle.Round round) {
            if (round.Actions.Count == 0) {
                return;
            }
            itemView.attackerHeroes = this.attackerHeroes;
            itemView.defenderHeroes = this.defenderHeroes;
            itemView.txtTitle.text = (itemView.ItemIndex == 0) ?
                LocalManager.GetValue(LocalHashConst.battle_ready_round) :
                string.Format(LocalManager.GetValue(LocalHashConst.battle_round),
                    itemView.ItemIndex);
            GameHelper.ClearChildren(itemView.pnlRoundList);
            foreach (Battle.Action action in round.Actions) {
                itemView.PrintAction(0, action, null, true);
            }
        }


        public void ClearInfo() {
            this.ResetPositionHero(this.viewPref.pnlAttacker);
            this.ResetPositionHero(this.viewPref.pnlDefender);
            UIManager.HideUICanvasGroup(this.viewPref.defenderCG);
            UIManager.HideUICanvasGroup(this.viewPref.attackerCG);
        }

        public void SetBattleReport(List<Battle.Round> battleRounds) {
            this.SetBattleArmyInfo();
            this.SetRoundInfo(battleRounds);
        }

        private void SetBattleArmyInfo() {
            Battle.Report Report = this.viewModel.BattleReport.Report;
            Battle.PlayerInfo attackerInfo = Report.Attacker;
            Battle.PlayerInfo defenderInfo = Report.Defender;
            
            this.SetHeroDict(this.attackerHeroes, attackerInfo.BeforeHeroes);
            this.SetHeroDict(this.defenderHeroes, defenderInfo.BeforeHeroes);
            this.SetArmyInfo(attackerInfo, this.viewPref.pnlAttacker, this.battleWin);
            this.SetArmyInfo(defenderInfo, this.viewPref.pnlDefender, !this.battleWin);
        }

        private void SetRoundInfo(List<Battle.Round> battleRounds) {
            this.battleReportRounds = battleRounds;
        }

        private void ResetPositionHero(Transform pnlTroop) {
            Transform pnlTroopList = pnlTroop.Find("PnlTroops");
            for (int i = 1; i < 7; i++) {
                Transform position = pnlTroopList.Find("Position" + i);
                GameHelper.ClearChildren(position);
            }
        }

        private void SetArmyInfo(Battle.PlayerInfo playerInfo, Transform pnlTroop, bool win) {
            Transform pnlTroopList = pnlTroop.Find("PnlTroops");
            playerInfo.AfterHeroes.Sort((a, b) => {
                return a.Position.CompareTo(b.Position);
            });
            foreach (Battle.Hero hero in playerInfo.AfterHeroes) {
                Transform position = pnlTroopList.Find("Position" + hero.Position);
                GameObject heroHead = PoolManager.GetObject(
                    PrefabPath.pnlHeroBig,
                    position
                );
                HeroHeadView heroHeadView = heroHead.GetComponent<HeroHeadView>();
                heroHeadView.SetHero(hero, showArmyAmount: true, showPower: false);
                heroHeadView.OnHeroClick.RemoveAllListeners();
                heroHeadView.IsEnable = (hero.ArmyAmount > 0);
                // Test : Find a way get every thing done;
                heroHeadView.IsShowUnlockChest = false;
            }
            UIManager.ShowUI(pnlTroop.gameObject);
        }

        private void SetHeroDict(Dictionary<string, Battle.Hero> heroDict, List<Battle.Hero> heroList) {
            heroDict.Clear();
            foreach (Battle.Hero hero in heroList) {
                heroDict.Add(hero.Id, hero);
            }
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
