using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class MarchConfirmView : BaseView {
        private MarchConfirmViewModel viewModel;
        private MarchConfirmViewPreference viewPref;
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<MarchConfirmViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UITroopInfo.PnlMarchConfirm");
            this.viewPref = this.ui.transform.GetComponent<MarchConfirmViewPreference>();
            
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnRecruit.onClick.AddListener(this.OnBtnRecruitClick);
            this.viewPref.btnStillGo.onClick.AddListener(this.OnBtnStillGo);
        }

        public override void PlayShow() {
            base.PlayShow();
        }

        public void SetMarchConfirm() {
            if (this.viewModel.Result == BattleSimulationResult.Hard) {
                this.viewPref.txtWarning.text = LocalManager.GetValue(LocalHashConst.battle_difficulty_hard_with_color);
                this.viewPref.txtContent.text = LocalManager.GetValue(LocalHashConst.battle_difficulty_hard_detail);
                this.viewPref.txtBtnRecruit.text = LocalManager.GetValue(LocalHashConst.get_back_to_heal);
                this.viewPref.imgTip.sprite = ArtPrefabConf.GetSprite("march_confirm_tip_heal");
            } else {
                this.viewPref.txtWarning.text = LocalManager.GetValue(LocalHashConst.hero_need_levelup);
                this.viewPref.txtContent.text = LocalManager.GetValue(LocalHashConst.hero_need_fragment);
                this.viewPref.txtBtnRecruit.text = LocalManager.GetValue(LocalHashConst.heroe_levelup);
                this.viewPref.imgTip.sprite = ArtPrefabConf.GetSprite("march_confirm_tip_upgrade");
            }
        }

        private void OnBtnStillGo() {
            this.viewModel.MarchReq();
        }

        private void OnBtnRecruitClick() {
            this.viewModel.Recruit();
        }

        protected void OnBtnCloseClick() {
            this.viewModel.HideTroop(false);
        }

        protected override void OnVisible() {
            this.viewPref.animBtnEffect.enabled = true;
        }

        protected override void OnInvisible() {
            this.viewPref.animBtnEffect.enabled = false;
        }
        /* Propert change function */

        /***************************/
    }
}
