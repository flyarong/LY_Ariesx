using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;
using System.Text;

namespace Poukoute {
    public class BattleReportTipView : BaseView {
        private BattleReportTipViewModel viewModel;
        private BattleReportTipViewPreference viewPref;
        /*************/
        //private string reportId;
        private Coroutine coroutine;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<BattleReportTipViewModel>();
            FteManager.SetStartCallback(GameConst.NORMAL, 61, this.OnFteStep61Start);
            FteManager.SetEndCallback(GameConst.NORMAL, 71, this.OnFteStep71End);
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIBattleReportTip");
            GameHelper.SetCanvasCamera(this.ui.GetComponent<Canvas>());
            this.viewPref = this.ui.transform.GetComponent<BattleReportTipViewPreference>();
            this.viewPref.btnPlayBattleReport.onClick.AddListener(OnBtnPlayBattleReportClick);
        }

        public void SetContent(TroopArrivedNtf message) {
            //Debug.Log("SetContent");
            this.InitUI();
            this.viewPref.pnlArrive.gameObject.SetActiveSafe(false);
            this.viewPref.pnlAttack.gameObject.SetActiveSafe(false);
            this.viewPref.pnlDefend.gameObject.SetActiveSafe(false);

            this.viewPref.txtLeftName.text = TroopModel.GetTroopName(message.TroopName);
            Troop troop = this.viewModel.GetTroopByArmyCampName(message.TroopName);
            this.viewPref.imgLeftHead.sprite =
                ArtPrefabConf.GetSprite(string.Format("{0}_s", troop.Positions[0].Name));
            //message.PointInfo.Monster
            if (message.HasBattle) {
                this.viewPref.pnlAttack.gameObject.SetActiveSafe(true);
                if (!message.DefenderName.CustomIsEmpty()) {
                    this.viewPref.pnlRight.gameObject.SetActiveSafe(true);
                    this.viewPref.pnlTile.gameObject.SetActiveSafe(false);
                    string avatarName = PlayerHeroAvatarConf.GetAvatarName(message.DefenderAvatar.ToString());
                    if (avatarName.CustomIsEmpty()) avatarName = "hero_106";
                    this.viewPref.imgRightHead.sprite = ArtPrefabConf.GetRoleAvatarSprite(avatarName);
                    this.viewPref.txtRightName.text = message.DefenderName;
                    this.viewPref.pnlResource.gameObject.SetActiveSafe(false);
                    if (message.IsOccupied && (message.PointInfo.Resource != null)) {
                        this.SetResource(message.PointInfo.Resource.Type, message.PointInfo.Resource.Level);
                    }
                } else {
                    this.viewPref.pnlRight.gameObject.SetActiveSafe(false);
                    this.viewPref.pnlTile.gameObject.SetActiveSafe(true);
                    this.viewPref.txtTile.text = message.PointInfo.GetOccureTileName();
                    this.viewPref.pnlResource.gameObject.SetActiveSafe(false);
                    if (message.PointInfo.Monster != null) {
                        this.viewPref.txtTile.text = string.Concat(
                            CampaignModel.MonsterLocalName,
                            string.Format(
                                LocalManager.GetValue(LocalHashConst.level),
                                message.PointInfo.Monster.Level
                            )
                        );
                    }
                    Debug.Log(message.PointInfo.Monster);
                    if (message.IsOccupied && (message.PointInfo.Resource != null)) {
                        this.SetResource(message.PointInfo.Resource.Type, message.PointInfo.Resource.Level);
                    }
                }
            } else {
                this.viewPref.pnlArrive.gameObject.SetActiveSafe(true);
                this.viewPref.pnlRight.gameObject.SetActiveSafe(false);
                this.viewPref.pnlTile.gameObject.SetActiveSafe(true);
                this.viewPref.pnlResource.gameObject.SetActiveSafe(false);
                this.viewPref.txtTile.text = message.PointInfo.GetOccureTileName();
            }
            this.viewPref.txtArrive.gameObject.SetActiveSafe(!message.HasBattle);
            //Debug.Log(message.IsWin);
            this.viewPref.txtVictory.gameObject.SetActiveSafe(message.IsWin && message.HasBattle);
            this.viewPref.txtFailure.gameObject.SetActiveSafe(!message.IsWin && message.HasBattle);
            StartCoroutine(DelayShowBanner());
            if (!this.IsVisible) {
                this.PlayShow();
            }

            if (this.coroutine != null) {
                this.StopCoroutine(this.coroutine);
            }
            this.coroutine = this.StartCoroutine(this.HideReportTip());
        }

        public void SetDefendContent(TroopHasBeenAttackedNtf message) {
            this.InitUI();
            this.viewPref.pnlArrive.gameObject.SetActiveSafe(false);
            this.viewPref.pnlAttack.gameObject.SetActiveSafe(false);
            this.viewPref.pnlDefend.gameObject.SetActiveSafe(true);
            this.viewPref.txtLeftName.text = TroopModel.GetTroopName(message.TroopName);
            Troop troop = this.viewModel.GetTroopByArmyCampName(message.TroopName);
            this.viewPref.imgLeftHead.sprite =
                ArtPrefabConf.GetSprite(string.Format("{0}_s", troop.Positions[0].Name));
            this.viewPref.pnlRight.gameObject.SetActiveSafe(true);
            this.viewPref.pnlTile.gameObject.SetActiveSafe(false);
            this.viewPref.pnlResource.gameObject.SetActiveSafe(false);

            string avatarName = PlayerHeroAvatarConf.GetAvatarName(message.AttackerAvatar.ToString());
            //Debug.Log(avatarName);
            if (avatarName.CustomIsEmpty()) avatarName = "hero_106";
            this.viewPref.imgRightHead.sprite = ArtPrefabConf.GetRoleAvatarSprite(avatarName);
            this.viewPref.txtRightName.text = message.AttackerName;

            this.viewPref.txtArrive.gameObject.SetActiveSafe(false);
            this.viewPref.txtVictory.gameObject.SetActiveSafe(message.DefenderWin);
            this.viewPref.txtFailure.gameObject.SetActiveSafe(!message.DefenderWin);
            StartCoroutine(DelayShowBanner());
            if (!this.IsVisible) {
                this.PlayShow();
            }
            if (this.coroutine != null) {
                this.StopCoroutine(this.coroutine);
            }
            this.coroutine = this.StartCoroutine(this.HideReportTip());
        }

        private IEnumerator HideReportTip() {
            yield return YieldManager.GetWaitForSeconds(5f);
            if (this.IsVisible) {
                this.PlayHide();
                this.viewModel.ShowTopHUD();
            }
        }

        private void SetResource(int type, int level) {
            string pointType = MapBasicTypeConf.GetConf(type.ToString()).type;
            this.viewPref.txtResource.text = string.Empty;
            Dictionary<Resource, int> resource = MapResourceProductionConf.GetConf(pointType + level.ToString()).productionDict;
            if (resource == null) {
                this.viewPref.pnlResource.gameObject.SetActiveSafe(false);
                return;
            }
            this.viewPref.pnlResource.gameObject.SetActiveSafe(true);
            foreach (var item in resource) {
                if (item.Value != 0) {
                    this.viewPref.txtResource.text += LocalManager.GetValue("resource_" + item.Key.ToString())
                        + LocalManager.GetValue("produce") + "\n"
                        + MapResourceProductionConf.GetProduction(item.Value) + "\n\n";
                }
            }
        }

        private IEnumerator DelayShowBanner() {
            this.viewPref.imgRight.fillOrigin = 1;
            this.viewPref.imgLeft.fillOrigin = 0;
            for (int i = 0; i <= 13; i++) {
                this.viewPref.imgLeft.fillAmount =
                    this.viewPref.showCurve.Evaluate(this.viewPref.showCurve.keys[1].time / 13 * i) * 2;
                if (i == 13) {
                    AnimationManager.Animate(this.viewPref.pnlDetail.gameObject, "Show");
                }
                yield return YieldManager.GetWaitForSeconds(this.viewPref.showCurve.keys[1].time / 13);
            }
            for (int i = 0; i <= 13; i++) {
                this.viewPref.imgRight.fillAmount =
                    (this.viewPref.showCurve.Evaluate((this.viewPref.showCurve.keys[2].time -
                        this.viewPref.showCurve.keys[1].time)
                            / 13 * i + this.viewPref.showCurve.keys[1].time) - 0.5f) * 2f;

                yield return YieldManager.GetWaitForSeconds
                    ((this.viewPref.showCurve.keys[2].time
                        - this.viewPref.showCurve.keys[1].time) / 13);
            }
            yield return YieldManager.GetWaitForSeconds(1f);
            this.viewPref.imgRight.fillOrigin = 0;
            this.viewPref.imgLeft.fillOrigin = 1;
            for (int i = 0; i <= 13; i++) {
                this.viewPref.imgLeft.fillAmount =
                    1 - this.viewPref.hideCurve.Evaluate(
                        this.viewPref.hideCurve.keys[1].time / 13 * i) * 2;
                yield return YieldManager.GetWaitForSeconds(
                    this.viewPref.hideCurve.keys[1].time / 13);
                if (i == 0) {
                    AnimationManager.Animate(this.viewPref.pnlDetail.gameObject, "Hide");
                }
            }
            for (int i = 0; i <= 13; i++) {
                this.viewPref.imgRight.fillAmount = 1 -
                    (this.viewPref.hideCurve.Evaluate((this.viewPref.hideCurve.keys[2].time
                         - this.viewPref.hideCurve.keys[1].time) / 13 * i
                            + this.viewPref.hideCurve.keys[1].time) - 0.5f) * 2f;
                yield return YieldManager.GetWaitForSeconds((
                    this.viewPref.hideCurve.keys[2].time
                        - this.viewPref.hideCurve.keys[1].time) / 13);
            }
            yield return YieldManager.GetWaitForSeconds(0.2f);
            base.Hide();
        }

        private void OnBtnPlayBattleReportClick() {
            if (this.IsVisible) {
                this.Hide();
            }
            this.viewModel.PlayReport();
        }

        #region Fte
        private void OnFteStep61Start(string index) {
            //this.viewPref.gameObject.SetActive(false);
        }

        private void OnFteStep71End() {
            //this.viewPref.gameObject.SetActive(true);
        }
        #endregion
    }
}
