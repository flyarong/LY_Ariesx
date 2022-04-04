using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class BuildUnlockView : BaseView {
        private BuildUnlockViewModel viewModel;
        private BuildUnlockViewPreference viewPref;
        /* UI Members*/

        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<BuildUnlockViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIBuildUnlock");
            /* Cache the ui components here */
            this.viewPref = this.ui.transform.GetComponent<BuildUnlockViewPreference>();
            this.viewPref.btnUnlockClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnUnlock.onClick.AddListener(this.OnBtnUnlockClick);
            this.viewPref.btnBackClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnBuyMonthly.onClick.AddListener(this.OnBtnShowPayClick);
        }

        public override void PlayShow() {
            base.PlayShow();
            this.viewPref.txtPrice.text = GameConst.BUILD_QUEUE_COST.ToString();
            if (RoleManager.GetResource(Resource.Gem) < GameConst.BUILD_QUEUE_COST) {
                this.viewPref.txtPrice.color = Color.red;
            } else {
                this.viewPref.txtPrice.color = Color.white;
            }
        }

        private void OnBtnShowPayClick() {
            this.viewModel.ShowPayOnClick();
        }

        public void SetContent(bool needTip = false) {
            if (needTip) {
                this.viewPref.txtContent.text =
                    LocalManager.GetValue(LocalHashConst.unlock_build_queue_content_busy);
            } else {
                this.viewPref.txtContent.text =
                    LocalManager.GetValue(LocalHashConst.unlock_build_queue_content);
            }
        }

        public override void PlayHide() {
            base.PlayHide();
        }

        public void OnBtnUnlockClick() {
            this.viewPref.btnUnlock.interactable = false;
            this.viewModel.UnlockReq();
        }

        public void EnableBtnUnlockClick() {
            this.viewPref.btnUnlock.interactable = true;
        }

        public void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        /* Propert change function */

        /***************************/

    }
}
