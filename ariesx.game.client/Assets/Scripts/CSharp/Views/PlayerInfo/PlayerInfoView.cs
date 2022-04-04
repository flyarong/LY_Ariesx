using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class PlayerInfoView : BaseView {
        private PlayerInfoViewModel viewModel;
        private PlayerInfoViewPreference viewPref;

        private void Awake() {
            this.viewModel = this.transform.GetComponent<PlayerInfoViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIPlayerInfo");
            this.viewPref = this.ui.transform.GetComponent<PlayerInfoViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.tabView.InitTab(2);
            this.viewPref.tabView.SetTab(
                0, new TabInfo(LocalManager.GetValue(LocalHashConst.house_keeper_state),
                this.viewPref.pnlState, (state) => {
                    OnToggleValueChange(PlayerInfoType.State, state);
                }));
            this.viewPref.tabView.SetTab(
               1, new TabInfo(LocalManager.GetValue(LocalHashConst.force_grow),
               this.viewPref.pnlForce, (state) => {
                   OnToggleValueChange(PlayerInfoType.Force, state);
               }));
            this.viewPref.tabView.SetAllOff();
            this.viewPref.tabView.SetCloseCallBack(this.OnBtnCloseClick);
        }

        public void SetAllOff() {
            this.viewPref.tabView.SetAllOff();
        }

        public void SetTab(int index) {
            this.viewPref.tabView.SetActiveTab(index);
        }

        private void OnToggleValueChange(PlayerInfoType viewType, bool state) {
            if (state) {
                this.viewModel.Channel = viewType;
            }
        }

        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}