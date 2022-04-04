using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceView : BaseView {
        private AllianceViewModel viewModel;
        private AllianceViewPreference viewPref;

        /******************/
        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<AllianceViewModel>();
            this.ui = UIManager.GetUI("UIAlliance");
            this.viewPref = this.ui.transform.GetComponent<AllianceViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.HideCurrentPanel);
        }

        public void HideCurrentPanel() {
            this.viewModel.Hide();
        }
    }
}
