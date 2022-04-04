using UnityEngine;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Poukoute {
    public class BattleSimulationStatView : MonoBehaviour {
        private BattleSimulationStatViewModel viewModel;
        private BattleSimulationStatViewPreference viewPref;

        private GameObject ui;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<BattleSimulationStatViewModel>();
            InitUi();
        }

        public void Show() {
            this.ClearInfo();
            this.SetBattleArmyInfo();
        }

        public void Hide() {
            this.ui.gameObject.SetActive(false);
        }

        private void InitUi() {
            this.ui = GameObject.Find("UI").transform.Find("UIStatReport").gameObject;
            this.viewPref = this.ui.GetComponent<BattleSimulationStatViewPreference>();
            this.viewPref.btnClose.onClick.AddListener(this.Hide);
            this.viewPref.BG.onClick.AddListener(this.Hide);

        }

        public void SetBattleArmyInfo() {
            Battle.Report Report = this.viewModel.BattleReport;
            Battle.PlayerInfo attackerInfo = Report.Attacker;
            Battle.PlayerInfo defenderInfo = Report.Defender;
            this.SetArmyInfo(attackerInfo, this.viewPref.pnlAttTroops,this.viewPref.txtAttTroops, true);
            this.SetArmyInfo(defenderInfo, this.viewPref.pnlDefTroops, this.viewPref.txtDefTroops, false);
        }

        private void SetArmyInfo(Battle.PlayerInfo playerInfo, Transform[] pnlTroop, Text[] txtTroop, bool isAttacker) {
            playerInfo.AfterHeroes.Sort((a, b) => {
                return a.Position.CompareTo(b.Position);
            });
            foreach (Battle.Hero hero in playerInfo.AfterHeroes) {
                GameObject heroHead = PoolManager.GetObject(
                    PrefabPath.pnlHeroBig,
                    pnlTroop[hero.Position - 1]
                );
                HeroHeadView heroHeadView = heroHead.GetComponent<HeroHeadView>();
                heroHeadView.SetHero(hero, showArmyAmount: true, showPower: false);
                heroHeadView.OnHeroClick.RemoveAllListeners();
                heroHeadView.IsEnable = (hero.ArmyAmount > 0);
                txtTroop[hero.Position - 1].text = this.viewModel.GetHeroStatInfo(hero.Id, isAttacker);
                heroHeadView.IsShowUnlockChest = false;
            }
            this.SetPlayerTroopInfo(isAttacker);
        }

        private void SetPlayerTroopInfo(bool isAttacker) {
            Text txtStat = isAttacker ? this.viewPref.txtAttStat : this.viewPref.txtDefStat;
            txtStat.text = this.viewModel.GetStatInfo(isAttacker);
        }

        public void ClearInfo() {
            this.ResetPositionHero(this.viewPref.pnlAttTroops,this.viewPref.txtAttTroops);
            this.ResetPositionHero(this.viewPref.pnlDefTroops,this.viewPref.txtDefTroops);
        }
        private void ResetPositionHero(Transform[] pnlTroop,Text[] txtTroop) {
            for (int i = 0; i < 6; i++) {
                GameHelper.ClearChildren(pnlTroop[i]);
                txtTroop[i].text = null;
            }
        }
    }
}
