using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class AllianceSubWindowsView : BaseView {
        private AllianceSubWindowsViewModel viewModel;
        private AllianceSubWindowsViewPreference viewPref;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<AllianceSubWindowsViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIAllianceWindows");
            this.viewPref = this.ui.transform.GetComponent<AllianceSubWindowsViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
        }

        public void SetWindowsHeadVisible(bool isVisible) {
            UIManager.SetUICanvasGroupEnable(this.viewPref.windowsHeadCG, isVisible);
        }
        /* Propert change function */

        /***************************/
        public void SetTitleInfo(string title) {
            this.InitUI();
            this.viewPref.windowTitle.text = title;
        }

        /***************************/
        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
