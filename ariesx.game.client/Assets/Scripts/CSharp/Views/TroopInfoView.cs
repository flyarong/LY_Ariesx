using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using System;
using TMPro;

namespace Poukoute {
    public class TroopInfoView : BaseView {
        private TroopInfoViewModel viewModel;
        private TroopInfoViewPreference viewPref;

        private bool isDetailVisible = false;
        private bool isShowTip = false;
        public bool IsShowTip {
            get {
                return this.isShowTip;
            }
            set {
                this.SetTimeCostTipVisible(value);
            }
        }
        private ArmyCampConf armyCampConf = null;

        private readonly List<string> marchTipsLocal = new List<string>() {
            LocalManager.GetValue(LocalHashConst.field_low_level_no_lottery),
            LocalManager.GetValue(LocalHashConst.battle_resource_upper_limit),
            LocalManager.GetValue(LocalHashConst.free_lottery_upper_limit),
            LocalManager.GetValue(LocalHashConst.land_upper_limit),
        };

        private const int TROOPS_COUNT_UPPER = 6;
        private Vector2 sizeDelta = Vector2.zero;
        private GameObject marchLine;

        void Awake() {
            this.viewModel = this.GetComponent<TroopInfoViewModel>();
        }

        private Vector2 detailSize = Vector2.zero;
        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UITroopInfo");
            this.viewPref = this.ui.transform.GetComponent<TroopInfoViewPreference>();

            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnBackgroundClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnBackgroundClick);
            this.viewPref.btnSend.onClick.AddListener(this.OnBtnSendClick);
            this.viewPref.btnPnl.onClick.RemoveAllListeners();
            this.viewPref.btnPnl.onClick.AddListener(this.OnBtnPnlClick);
            this.viewPref.btnReturnImmediately.
                onClick.AddListener(this.OnBtnReturnImmediatelyClick);
            this.detailSize = this.viewPref.detailRectTransform.sizeDelta;
        }

        public override void PlayShow(UnityAction action) {
            if (string.IsNullOrEmpty(this.viewModel.CurrentTroop)) {
                return;
            }
            base.StartCoroutine(this.ShowTroopInfo(action));
            this.viewPref.btnBackground.gameObject.SetActiveSafe(true);
        }

        public override void PlayHide(UnityAction callback) {
            if (this.isDetailVisible) {
                this.HideDetailPnl(callback);
            } else {
                this.Hide();
                callback.InvokeSafe();
            }
        }

        public void HideImmediatly() {
            if (this.isDetailVisible) {
                this.HideDetailImmediatly();
            }
        }

        // To do: need troop label.
        private IEnumerator ShowTroopInfo(UnityAction action) {
            this.ShowDetailPnl(action);
            yield return YieldManager.EndOfFrame;
            this.SetTroopInfo();
            if ((this.viewModel.ViewType & (TroopViewType.Move | TroopViewType.Return)) == 0) {
                this.SetBattleSimulateInfo();
            } else {
                this.ResetBattleSimulateInfo();
            }
            this.SetTipInfo();
            this.ShowMarchLine();
        }

        private void ShowMarchLine() {
            if (this.marchLine == null) {
                this.marchLine = PoolManager.GetObject(PrefabPath.march, null);
            }
            MarchLineView marchLineView = this.marchLine.GetComponent<MarchLineView>();
            Troop troop = this.viewModel.TroopDict[this.viewModel.CurrentTroop];
            Vector2 start = new Vector2(
                troop.Coord.X,
                troop.Coord.Y
            );
            Vector2 target = Vector2.zero;
            if (this.viewModel.ViewType == TroopViewType.Return) {
                target = RoleManager.GetRoleCoordinate();
            } else {
                target = this.viewModel.Target;
            }
            marchLineView.SetLine(start, target + new Vector2(-0.14f, 0.14f),
                Vector3.forward * TileView.marchLineHighLayerOffset);
            marchLineView.ShowMarch = false;
            this.marchLine.transform.position += Vector3.forward * -3;
        }

        private void ShowDetailPnl(UnityAction action) {
            if (!this.isDetailVisible) {
                base.PlayShow(action);
                this.isDetailVisible = true;
            }
        }

        private void HideDetailPnl(UnityAction callback) {
            if (this.isDetailVisible) {
                base.PlayHide(() => {
                    callback.InvokeSafe();
                    this.isDetailVisible = false;
                    this.HideMarchLine();
                });
            }
        }

        private void HideDetailImmediatly() {
            if (this.isDetailVisible) {
                UIManager.SetUIVisible(this.viewPref.pnlDetail.gameObject, false);
                this.isDetailVisible = false;
                this.HideMarchLine();
            }
        }

        private void HideMarchLine() {
            if (this.marchLine != null) {
                PoolManager.RemoveObject(this.marchLine);
                this.marchLine = null;
            }
        }

        private void SetTroopInfo() {
            Troop troop = this.viewModel.TroopDict[this.viewModel.CurrentTroop];
            this.armyCampConf = ArmyCampConf.GetConf(
               this.viewModel.BuildingDict[troop.ArmyCamp].Level.ToString()
           );
            MarchAttributes marchAttributes =
                this.viewModel.GetMarchAttributes(this.viewModel.CurrentTroop);
            this.ShowTroopGrid();
            this.viewPref.pnlTroopInfo.GetChild(0).Find("TxtNumber").GetComponent<TextMeshProUGUI>().text =
                GameHelper.GetFormatNum(marchAttributes.army, decLength: 1);
            this.viewPref.pnlTroopInfo.GetChild(1).Find("TxtNumber").GetComponent<TextMeshProUGUI>().text =
                HeroAttributeConf.GetSpeed(marchAttributes.speed);
            this.viewPref.pnlTroopInfo.GetChild(2).Find("TxtNumber").GetComponent<TextMeshProUGUI>().text =
                GameHelper.GetFormatNum((long)marchAttributes.siege, decLength: 1);

            this.SetTroopMarhInfo(marchAttributes);
            this.SetButton();
        }

        private void SetTroopMarhInfo(MarchAttributes marchAttributes) {
            Troop troop = this.viewModel.TroopDict[this.viewModel.CurrentTroop];
            this.viewPref.txtEnergy.text = marchAttributes.energy.ToString();
            if (marchAttributes.energy < 2) {
                this.viewPref.txtEnergy.color = Color.red;
            } else {
                this.viewPref.txtEnergy.color = Color.white;
            }

            long afterProtection = FteManager.CheckDrama(1) ? (marchAttributes.timeCost /
                (RoleManager.IsUnderProtection() ? 2 : 1)) : 5000;

            Point start;
            Point target;
            if (RoleManager.GetPointDict().TryGetValue(troop.Coord, out start) &&
                RoleManager.GetPointDict().TryGetValue(this.viewModel.Target, out target) &&
                start.Building != null && target.Building != null && 
                (start.Building.Type == (int)ElementType.stronghold || 
                    start.Building.Type == (int)ElementType.townhall) &&
                (target.Building.Type == (int)ElementType.stronghold ||
                    target.Building.Type == (int)ElementType.townhall)
            ) {
                Debug.LogError(afterProtection);
                afterProtection = (long)(afterProtection * 0.7f);
            }

            float speedUp = this.viewModel.GetSpeedBonus();
            long seconds = (long)(speedUp * afterProtection);
            if ((seconds / 1000) < 1) {
                this.viewPref.txtTimeCost.text = GameHelper.TimeFormat(afterProtection);
            } else {
                this.viewPref.btnTimeCost.onClick.RemoveAllListeners();
                this.viewPref.btnTimeCost.onClick.AddListener(() => {
                    this.IsShowTip = !this.IsShowTip;
                });
                this.viewPref.txtTimeTip.text = string.Format(
                    LocalManager.GetValue(LocalHashConst.march_bonus_speed),
                    speedUp * 100
                );

                this.viewPref.txtTimeCost.text = string.Concat(
                    " <color=#73EE67FF>(",
                    GameHelper.TimeFormat(afterProtection- seconds)
                    ,")</color>"
                 ); 
            }
            this.viewPref.txtTimeArrive.text =
                GameHelper.DateFormat(marchAttributes.timeArrive / 1000, "HH:mm:ss");
        }

        private void SetTimeCostTipVisible(bool visible) {
            if (this.isShowTip != visible) {
                this.isShowTip = visible;
                if (this.isShowTip) {
                    this.viewPref.pnlTimeTip.gameObject.SetActive(true);
                    AnimationManager.Animate(this.viewPref.pnlTimeTip.gameObject,
                        "Show", Vector2.right * 130,
                        new Vector2(130, 45), null);
                } else {
                    AnimationManager.Animate(this.viewPref.pnlTimeTip.gameObject, "Hide",
                        new Vector2(130, 45), Vector2.right * 130, () => {
                            this.viewPref.pnlTimeTip.gameObject.SetActive(true);
                        });
                }
            }
        }

        private void SetBattleSimulateInfo() {
            this.EnableBtnSend();
            this.viewPref.btnSend.Grayable = false;
            this.viewPref.btnSend.onClick.RemoveAllListeners();
            this.viewPref.btnSend.onClick.AddListener(this.OnBtnSendClick);
            this.viewPref.TipsRT.gameObject.SetActive(true);

            BattleSimulationResult result = this.viewModel.BattleSimulation();
            switch (result) {
                case BattleSimulationResult.Hard:
                case BattleSimulationResult.HardWeak:
                    this.SetBattleSimulationHard(result);
                    break;
                //case BattleSimulationResult.Normal:
                //    this.viewPref.txtResult.text = LocalManager.GetValue(LocalHashConst.battle_difficulty_normal);
                //    this.viewPref.txtResult.color = ArtConst.Orange;
                //    break;
                //case BattleSimulationResult.Easy:
                //    this.viewPref.txtResult.text = LocalManager.GetValue(LocalHashConst.battle_difficulty_easy);
                //    this.viewPref.txtResult.color = new Color(35 / 255f, 153 / 255f, 28 / 255f);
                //    break;
                case BattleSimulationResult.NoTroop:
                    this.SetBattlSimulationNotroop();
                    break;
                default:
                    this.viewPref.txtResult.text = string.Empty;
                    break;
            }
            if (this.viewModel.IsAlliacneTarget()) {
                this.viewPref.txtResult.text = LocalManager.GetValue(LocalHashConst.battle_difficulty_alliance);
                this.viewPref.txtResult.color = ArtConst.Red;
            }
            if (this.viewModel.GetMonsterInfo() != null) {
                this.viewPref.txtResult.gameObject.SetActive(false);
            } else {
                this.viewPref.txtResult.gameObject.SetActive(true);
            }
        }

        private void ResetBattleSimulateInfo() {
            this.viewPref.TipsRT.gameObject.SetActive(false);
            this.viewPref.txtResult.text = string.Empty;
            this.viewPref.btnSend.onClick.RemoveAllListeners();
            this.viewPref.btnSend.onClick.AddListener(this.OnBtnSendClick);
        }

        private void SetBattleSimulationHard(BattleSimulationResult result) {
            string battleHard = string.Empty;
            if (result == BattleSimulationResult.Hard) {
                battleHard = LocalManager.GetValue(LocalHashConst.battle_difficulty_hard);
            } else {
                battleHard = LocalManager.GetValue(LocalHashConst.lands_level_high_tips);
            }
            this.viewPref.txtResult.text = battleHard;
            this.viewPref.txtResult.color = ArtConst.Red;
            this.viewPref.btnSend.onClick.RemoveAllListeners();
            this.viewPref.btnSend.onClick.AddListener(
                () => {
                    this.HideDetailImmediatly();
                    this.viewModel.ShowMarhcConfirm(result);
                }
            );
        }

        private void SetBattlSimulationNotroop() {
            // to do : battle simulation need do more.
            this.viewPref.txtResult.text = LocalManager.GetValue(LocalHashConst.battle_difficulty_no_troop);
            this.viewPref.txtResult.color = ArtConst.Red;
            this.viewPref.btnSend.Grayable = true;
            this.viewPref.btnSend.onClick.RemoveAllListeners();
            this.viewPref.btnSend.onClick.AddListener(
                () => {
                    this.HideDetailImmediatly();
                    this.viewModel.ShowMarhcConfirm(BattleSimulationResult.Hard);
                }
            );
        }

        private void ShowTroopGrid() {
            Troop troop = this.viewModel.TroopDict[this.viewModel.CurrentTroop];
            bool unlocked = false;
            for (int index = 0; index < TROOPS_COUNT_UPPER; index++) {
                GameHelper.ClearChildren(this.viewPref.heroTransform[index]);
                unlocked = armyCampConf.unlockPositionList.Contains(index + 1);
                if (index == 0) {
                    unlocked = true;
                }
                this.viewPref.imgEmpty[index].SetActiveSafe(unlocked);
                this.viewPref.imgLocked[index].SetActiveSafe(!unlocked);
                this.viewPref.imgEmptyClick[index].onClick.RemoveAllListeners();
                this.viewPref.imgEmptyClick[index].onClick.AddListener(() => {
                    UIManager.ShowTip(LocalManager.GetValue(LocalHashConst.upgrade_army_camp_tip),
                        TipType.Warn);
                });
            }

            foreach (HeroPosition hero in troop.Positions) {
                this.viewPref.imgEmpty[hero.Position - 1].SetActiveSafe(false);
                this.viewPref.imgLocked[hero.Position - 1].SetActiveSafe(false);
                this.SetHero(hero.Position, hero);
            }
        }

        private void SetHero(int position, HeroPosition hero) {
            GameHelper.ClearChildren(this.viewPref.heroTransform[position - 1]);
            GameObject heroObj = PoolManager.GetObject(
                    PrefabPath.pnlHeroBig,
                    this.viewPref.heroTransform[position - 1]);
            HeroHeadView heroHead = heroObj.GetComponent<HeroHeadView>();
            heroHead.SetHero(this.viewModel.HeroDict[hero.Name],
                false, showEnergy: true, showArmyAmount: true, showHeroStatus: false);
            heroHead.OnHeroClick.RemoveAllListeners();
            heroHead.gameObject.SetActiveSafe(true);
        }

        private void SetTipInfo() {
            GameHelper.ClearChildren(this.viewPref.pnlMarchTips);
            GameObject marchItemObject = null;
            if (this.viewModel.LowChanceGetLottery()) {
                marchItemObject = PoolManager.GetObject(
                    PrefabPath.pnlMarchTipItem, this.viewPref.pnlMarchTips);
                this.SetMarchItemData(marchItemObject, this.marchTipsLocal[0]);
            }

            if (this.viewModel.IsResourcsReachUpperLimit()) {
                marchItemObject = PoolManager.GetObject(
                    PrefabPath.pnlMarchTipItem, this.viewPref.pnlMarchTips);

                this.SetMarchItemData(marchItemObject,
                    string.Format(this.marchTipsLocal[1], GameHelper.GetToEarlyMorningTimeInterval()));
            }

            if (this.viewModel.IsChestsReachUpperLimit()) {
                marchItemObject = PoolManager.GetObject(
                    PrefabPath.pnlMarchTipItem, this.viewPref.pnlMarchTips);
                this.SetMarchItemData(marchItemObject, this.marchTipsLocal[2]);
            }

            bool reachLandUpperLimit =
                RoleManager.GetPointDict().Count >= RoleManager.GetPointsLimit();
            if (reachLandUpperLimit) {
                marchItemObject = PoolManager.GetObject(
                    PrefabPath.pnlMarchTipItem, this.viewPref.pnlMarchTips);
                this.SetMarchItemData(marchItemObject, this.marchTipsLocal[3]);
            }

            sizeDelta = this.detailSize;
            int marchTipsCount = this.viewPref.pnlMarchTips.childCount;
            if (marchTipsCount > 3) {
                sizeDelta.y += 50;
            }

            this.viewPref.detailRectTransform.sizeDelta = sizeDelta;
            Vector2 tipsSizeDelta = this.viewPref.TipsRT.sizeDelta;
            tipsSizeDelta.y = this.viewPref.resultRT.rect.height +
                (marchTipsCount * 50 - (marchTipsCount - 2) * 10) + 10;
            this.viewPref.TipsRT.sizeDelta = tipsSizeDelta;
        }

        private void SetButton() {
            this.viewPref.btnReturnImmediately.
                gameObject.SetActiveSafe(this.viewModel.IsTownhall());
        }

        public void InvokeBtnSend() {
            this.viewPref.btnSend.onClick.Invoke();
        }

        private void SetMarchItemData(GameObject marchTipObject, string localTips) {
            marchTipObject.transform.Find("TxtResult").GetComponent<TextMeshProUGUI>().text =
                localTips;
        }

        private void OnBtnBackgroundClick() {
            this.viewModel.Hide(false);
        }

        public void DisableBtnSend() {
            this.viewPref.btnSend.interactable = false;
        }

        public void EnableBtnSend() {
            this.viewPref.btnSend.interactable = true;
        }

        private void OnBtnPnlClick() {
            this.IsShowTip = false;
        }

        private void OnBtnSendClick() {
            bool isUnderFreshProtect = RoleManager.IsUnderProtection();
            int remainEnergy = this.viewModel.CurrentMarchAttributes.energy;
            if (((isUnderFreshProtect && remainEnergy < 1) ||
                (!isUnderFreshProtect && remainEnergy < GameConst.HERO_ENERGY_COST)) &&
                this.viewModel.Target != RoleManager.GetRoleCoordinate()) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.server_hero_energy_not_enough), TipType.Info);
                return;
            }
            this.viewModel.MarchReq();
        }

        private void OnBtnReturnImmediatelyClick() {
            if (RoleManager.GetResource(Resource.Gem) < 20) {
                UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.gem_short),
                    LocalManager.GetValue(LocalHashConst.server_gem_not_enough), 
                    () => { this.viewModel.ShowPay(); }, 
                    txtYes: LocalManager.GetValue(LocalHashConst.button_enter_shop));
                return;
            }
            this.viewModel.CompleteTroopMarchReq();  
        }

        #region FTE

        public void OnResourceStep4Start(bool isEnforce) {
            this.afterShowCallback = () => {
                FteManager.SetMask(this.viewPref.btnSend.pnlContent,
                    rotation: 0, offset: Vector2.up * 50, isEnforce: isEnforce);
            };
        }

        #endregion

        protected override void OnInvisible() {
            this.IsShowTip = false;
        }
    }
}
