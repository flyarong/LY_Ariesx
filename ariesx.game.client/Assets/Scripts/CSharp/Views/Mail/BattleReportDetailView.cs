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
    public class BattleReportDetailView: SRIA<BattleReportDetailViewModel, BattleReportRoundItemView> {
        private BattleReportDetailViewPreference viewPref;

        private bool battleWin;
        public Dictionary<string, Battle.Hero> attackerHeroes =
                new Dictionary<string, Battle.Hero>();
        public Dictionary<string, Battle.Hero> defenderHeroes =
            new Dictionary<string, Battle.Hero>();
        private string roleName = string.Empty;
        private List<Battle.Round> battleReportRounds = new List<Battle.Round>(4);
        /*************************************************/

        protected override void OnUIInit() {
            this.viewModel = this.GetComponent<BattleReportDetailViewModel>();
            this.ui = UIManager.GetUI("UIBattleDetail");
            this.viewPref = this.ui.transform.GetComponent<BattleReportDetailViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.SRIAInit(this.viewPref.customScrollRect,
                this.viewPref.customVerticalLayoutGroup, defaultItemSize: 371f);
        }


        #region SRIA implementation
        protected override BattleReportRoundItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlRound, this.viewPref.pnlRoundList);
            BattleReportRoundItemView itemView = itemObj.GetComponent<BattleReportRoundItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.battleReportRounds[itemIndex]);

            return itemView;
        }

        protected override void UpdateViewsHolder(BattleReportRoundItemView itemView) {
            this.OnItemContentChange(itemView,
                this.battleReportRounds[itemView.ItemIndex]);
            itemView.MarkForRebuild();
            this.ScheduleComputeVisibilityTwinPass(true);
        }
        #endregion

        private void OnItemContentChange(BattleReportRoundItemView itemView, Battle.Round round) {
            GameHelper.ClearChildren(itemView.pnlRoundList);
            if (round.Actions.Count == 0 && itemView.ItemIndex == 0) {
                itemView.txtTitle.text = string.Empty;
                return;
            }
            itemView.attackerHeroes = this.attackerHeroes;
            itemView.defenderHeroes = this.defenderHeroes;
            itemView.txtTitle.text = (itemView.ItemIndex == 0) ?
                LocalManager.GetValue(LocalHashConst.battle_ready_round) :
                string.Format(LocalManager.GetValue(LocalHashConst.battle_round),
                    itemView.ItemIndex);
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

        public void SetBattleReport(GetBattleReportAck battleReportAck, string title) {
            this.roleName = RoleManager.GetRoleName();
            this.SetBattleArmyInfo();
            this.SetRoundInfo(battleReportAck);
        }

        private void SetBattleArmyInfo() {
            Battle.Report Report = this.viewModel.BattleReport.Report;
            
            bool isDefender = this.SetBattleResultInfo(Report);
            Battle.PlayerInfo attackerInfo = Report.Attacker;
            Battle.PlayerInfo defenderInfo = Report.Defender;
            if (isDefender) {
                attackerInfo = Report.Defender;
                defenderInfo = Report.Attacker;
            }
            this.SetHeroDict(this.attackerHeroes, attackerInfo.BeforeHeroes);
            this.SetHeroDict(this.defenderHeroes, defenderInfo.BeforeHeroes);
            this.SetArmyInfo(attackerInfo, this.viewPref.pnlAttacker, this.battleWin);
            this.SetArmyInfo(defenderInfo, this.viewPref.pnlDefender, !this.battleWin);
        }

        private void SetRoundInfo(GetBattleReportAck battleReportAck) {
            this.battleReportRounds = battleReportAck.Rounds.Rounds;
            this.ResetItems(this.battleReportRounds.Count);
        }

        private bool SetBattleResultInfo(Battle.Report report) {

            this.battleWin = false;
            string defenderName = report.Defender.BasicInfo.Name;
            bool isDefender = defenderName.CustomEquals(this.roleName);

            if (report.Winner.CustomEquals("attacker") &&
                 report.Attacker.BasicInfo.Name.CustomEquals(this.roleName)) {
                this.battleWin = true;
            }
            this.viewPref.txtTitle.colorGradient = this.battleWin ?
               ArtConst.victoryVertexGradient : ArtConst.failureVertexGradient;
            this.viewPref.txtTitle.text =
                string.Concat(isDefender ? LocalManager.GetValue(LocalHashConst.mail_battle_report_defence) :
                                           LocalManager.GetValue(LocalHashConst.mail_battle_report_attack), "",
                              this.battleWin ? LocalManager.GetValue(LocalHashConst.mail_battle_victory) :
                                                    LocalManager.GetValue(LocalHashConst.mail_battle_failure));
            return isDefender;
        }

        private void ResetPositionHero(Transform pnlTroop) {
            Transform pnlTroopList = pnlTroop.Find("PnlTroops");
            for (int i = 1; i < 7; i++) {
                Transform position = pnlTroopList.Find("Position" + i);
                GameHelper.ClearChildren(position);
            }
        }

        public void ClearChildren() {
            //GameHelper.ClearChildren(this.viewPref.pnlRoundList);
            this.ClearTextLabel(this.viewPref.pnlAttacker);
            this.ClearTextLabel(this.viewPref.pnlDefender);
        }

        private void ClearTextLabel(Transform pnlTroop) {
            Transform pnlInfo = pnlTroop.Find("PnlInfo");
            Transform pnlTroopList = pnlTroop.Find("PnlTroops");
            for (int i = 1; i <= pnlTroopList.childCount; i++) {
                Transform position = pnlTroopList.Find("Position" + i);
                GameHelper.ClearChildren(position);
            }



            //TextMeshProUGUI name = pnlInfo.Find("TxtName").GetComponent<TextMeshProUGUI>();
            //TextMeshProUGUI allianceName = pnlInfo.Find("TxtAllianceName").GetComponent<TextMeshProUGUI>();
            //name.text = string.Empty; allianceName.text = string.Empty;
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
            this.SetPlayerTroopInfo(playerInfo, pnlTroop, win);
            UIManager.ShowUI(pnlTroop.gameObject);
        }

        private void SetPlayerTroopInfo(Battle.PlayerInfo playerInfo, Transform pnlTroop, bool win) {
            Transform pnlInfo = pnlTroop.Find("PnlInfo");
            //pnlInfo.gameObject.SetActiveSafe(true);
            TextMeshProUGUI name = pnlInfo.Find("TxtName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI allianceName = pnlInfo.Find("TxtAllianceName").GetComponent<TextMeshProUGUI>();
            bool isPlayerNameValiable = !playerInfo.BasicInfo.Name.CustomIsEmpty();
            bool isMonster = playerInfo.BasicInfo.Id.Contains("monster");
            bool isBoss = playerInfo.BasicInfo.Id.Contains("boss");
            if (isMonster) {
                int monsterLevel = int.Parse(playerInfo.BasicInfo.Id.CustomSplit('+')[1]);
                name.text = string.Concat(
                    CampaignModel.MonsterLocalName, string.Format(
                        LocalManager.GetValue(LocalHashConst.melee_map_level),
                        monsterLevel
                    )
                );
            } else if (isBoss) {
                int bossLevel = int.Parse(playerInfo.BasicInfo.Id.CustomSplit('+')[1]);
                name.text = string.Format(
                    LocalManager.GetValue(LocalHashConst.domination_detail_name), bossLevel);
            } else if (isPlayerNameValiable) {
                // To do : display name need change 
                if (playerInfo.BasicInfo.Name.CustomEquals(this.roleName)) {
                    name.text = LocalManager.GetValue(LocalHashConst.mail_battle_report_me);
                } else {
                    name.text = playerInfo.BasicInfo.Name;
                }
            } else {
                name.text = this.viewModel.BattleReport.Report.PointInfo.GetBattleOccureTileName();
            }
            if (isMonster || isBoss) {
                allianceName.text = string.Empty;
            } else {
                if (playerInfo.BasicInfo.AllianceName == null ||
               playerInfo.BasicInfo.AllianceName.CustomIsEmpty()) {
                    allianceName.text = LocalManager.GetValue(LocalHashConst.mail_battle_report_no_alliance);
                } else {
                    allianceName.text = playerInfo.BasicInfo.AllianceName;
                }
            }
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
