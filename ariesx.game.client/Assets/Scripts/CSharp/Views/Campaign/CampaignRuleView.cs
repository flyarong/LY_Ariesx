using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;

namespace Poukoute {
    public class CampaignRuleView : BaseView {
        private CampaignRuleViewModel viewModel;
        private CampaignRuleViewPreference viewPref;


        /*************/
        void Awake() {
            this.viewModel = this.gameObject.GetComponent<CampaignRuleViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UICampaignRule");
            this.viewPref = this.ui.transform.GetComponent<CampaignRuleViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
        }  

        public void SetContent(string content) {
            this.viewPref.txtContent.text = content;
        }
        
        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
