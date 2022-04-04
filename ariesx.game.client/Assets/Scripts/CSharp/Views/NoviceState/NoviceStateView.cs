using ProtoBuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class NoviceStateView : BaseView {
        private NoviceStateViewModel viewModel;
        private NoviceStateViewPreference viewPref;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<NoviceStateViewModel>();
            //this.InitUi();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UINoviceState");
            this.viewPref = this.ui.transform.GetComponent<NoviceStateViewPreference>();
            this.viewPref.btnClose.onClick.AddListener(this.OnCloseBtnClick);
            this.viewPref.Background.onClick.AddListener(this.OnCloseBtnClick);
        }

        public void SetTime() {
            this.viewPref.timeView.SetTime();
            this.viewPref.txtTime.text = 
                GameHelper.TimeFormat(RoleManager.GetFreshProtectionFinishAt()*1000
                    -RoleManager.GetCurrentUtcTime());
        }

        private void OnCloseBtnClick() {
            this.viewModel.Hide();
        }
    }
}
