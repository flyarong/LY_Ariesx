using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using Protocol;
using TMPro;
using System.Text;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class BattleSimulationReportItemView : BaseItemViewsHolder {
        private BattleReport battleReport;
        public BattleReport BattleReport {
            get {
                return this.battleReport;
            }
            set {
                if (this.battleReport != value) {
                    this.battleReport = value;
                    this.SetArmyInfo();
                    this.OnReadStatusChange();
                }
            }
        }

        public string AttackerName { get; set; }
        public string DefenderName { get; set; }

        public UnityEvent OnBtnDetailClick {
            get {
                this.btnDetail.onClick.RemoveAllListeners();
                return this.btnDetail.onClick;
            }
        }

        public UnityEvent OnBtnPlayClick {
            get {
                this.btnPlay.onClick.RemoveAllListeners();
                return this.btnPlay.onClick;
            }
        }

        public UnityEvent OnBtnItemClick {
            get {
                this.btnItem.onClick.RemoveAllListeners();
                return this.btnItem.onClick;
            }
        }

        public bool IsRead {
            get {
                return this.battleReport.IsRead;
            }
            set {
                this.battleReport.IsRead = value;
                this.OnReadStatusChange();
            }
        }

        #region UI component cache
        [SerializeField]
        private Image imgBG;
        [SerializeField]
        private Transform imgNewMark;
        [SerializeField]
        private Transform pnlAttacker;
        [SerializeField]
        private Transform pnlDefender;
        [SerializeField]
        private TextMeshProUGUI txtResult;
        [SerializeField]
        private Button btnPlay;
        [SerializeField]
        private Button btnDetail;
        [SerializeField]
        private Button btnItem;
        [SerializeField]

        #endregion
        private bool isAnyResource = false;
        private bool isDefender = false;
        private bool isRaid = false;
        private bool isMonster = false;
        private int monsterLevel = 0;
        private bool isBoss = false;
        private int BossLevel = 0;
        private bool isNeutralBattle = false;
        private bool isWin = false;
        private readonly int NEXT_TROOP_TYPE_DEFENDER = 1;
        private readonly int NEXT_TROOP_TYPE_ENDURANCE_DEFENDER = 2;
        // Methods
        public override void MarkForRebuild() {
            base.MarkForRebuild();
            //contentSizeFitter.enabled = true;
        }

        private void SetArmyInfo() {
            Battle.PlayerInfo attackerInfo;
            Battle.PlayerInfo defenderInfo;
            this.isWin = this.SetResultInfo(out attackerInfo, out defenderInfo);
            Battle.Result result = this.battleReport.Report.Result;
            this.SetFormat(defenderInfo, defenderInfo.BeforeHeroes, this.pnlDefender,
                            defenderInfo.BasicInfo, result, !isWin, isDefender);
            this.SetFormat(attackerInfo, attackerInfo.BeforeHeroes, this.pnlAttacker,
                            attackerInfo.BasicInfo, result, isWin, !isDefender);
            this.imgBG.sprite =
                ArtPrefabConf.GetSprite(SpritePath.battleReportBGPrefix,
                isWin ? "victory" : "failure");
        }

        private bool SetResultInfo(out Battle.PlayerInfo attackerInfo,
                                   out Battle.PlayerInfo defenderInfo) {
            string roleOwnId = this.battleReport.Report.Attacker.BasicInfo.Id;

            string defenderId = this.battleReport.Report.Defender.BasicInfo.Id;
            string attackerId = this.battleReport.Report.Attacker.BasicInfo.Id;
            this.isDefender = (defenderId.CustomEquals(roleOwnId) && !attackerId.CustomEquals(roleOwnId));
            this.isRaid = (defenderId.CustomEquals(roleOwnId) && attackerId.CustomEquals(roleOwnId));
            this.isMonster = defenderId.Contains("monster");
            this.isBoss = defenderId.Contains("boss");
            if (this.isBoss) {
                this.BossLevel = int.Parse(defenderId.CustomSplit('+')[1]);
            }
            if (this.isMonster) {
                this.monsterLevel = int.Parse(defenderId.CustomSplit('+')[1]);
            }
            if (this.isDefender) {
                attackerInfo = this.battleReport.Report.Defender;
                defenderInfo = this.battleReport.Report.Attacker;
            }
            else {
                attackerInfo = this.battleReport.Report.Attacker;
                defenderInfo = this.battleReport.Report.Defender;
            }
            this.isNeutralBattle = string.IsNullOrEmpty(defenderInfo.BasicInfo.Name);
            string battleWinner = this.battleReport.Report.Winner;
            bool win = (battleWinner.CustomEquals("attacker") && !this.isDefender) ||
                       (battleWinner.CustomEquals("defender") && this.isDefender);

            this.txtResult.colorGradient = win ?
               ArtConst.victoryVertexGradient : ArtConst.failureVertexGradient;
            this.txtResult.text = string.Concat(
                this.isDefender ? LocalManager.GetValue(LocalHashConst.mail_battle_report_defence) :
                                  LocalManager.GetValue(LocalHashConst.mail_battle_report_attack),
                              " ", win ? LocalManager.GetValue(LocalHashConst.mail_battle_victory) :
                                  LocalManager.GetValue(LocalHashConst.mail_battle_failure));
            return win;
        }

        private void SetFormat(Battle.PlayerInfo amountInfo, List<Battle.Hero> heroList, Transform participants,
           Battle.BasicInfo playerInfo, Battle.Result result, bool win, bool isAttacker) {

            Slider sliderTroop = participants.Find("PnlTroop").Find("SliTroop").GetComponent<Slider>();
            TextMeshProUGUI txtSliderAmount = participants.Find("PnlTroop").Find("TxtAmount").GetComponent<TextMeshProUGUI>();
            txtSliderAmount.text = this.GetAfterHeroesAmount(amountInfo).ToString() + "/" + this.GetBattleDeadAmount(amountInfo).ToString();
            sliderTroop.maxValue = this.GetBattleDeadAmount(amountInfo);
            sliderTroop.value = this.GetAfterHeroesAmount(amountInfo);
        }

        private int GetAfterHeroesAmount(Battle.PlayerInfo AfterHeroes) {
            int count = AfterHeroes.AfterHeroes.Count;
            int deadAmount = 0;
            int maxdeadAmount = 0;
            for (int i = 0; i < count; i++) {
                deadAmount = AfterHeroes.AfterHeroes[i].ArmyAmount;
                maxdeadAmount = maxdeadAmount + deadAmount;
            }
            return maxdeadAmount;
        }

        private int GetBattleDeadAmount(Battle.PlayerInfo BeforeHeroes) {
            int count = BeforeHeroes.BeforeHeroes.Count;
            int deadAmount = 0;
            int maxdeadAmount = 0;
            for (int i = 0; i < count; i++) {
                deadAmount = BeforeHeroes.BeforeHeroes[i].ArmyAmount;
                maxdeadAmount = maxdeadAmount + deadAmount;
            }
            return maxdeadAmount;
        }
        private void OnReadStatusChange() {
            this.imgNewMark.gameObject.SetActiveSafe(!this.IsRead);
        }
    }
}
