using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public class HouseKeeperView : BaseView {
        private HouseKeeperViewModel viewModel;
        private HouseKeeperViewPreference viewPref;

        void Awake() {
            this.viewModel = this.GetComponent<HouseKeeperViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIHouseKeeper");
            this.viewPref = this.ui.transform.GetComponent<HouseKeeperViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.tabView.InitTab(2);
            this.viewPref.tabView.SetTab(
               0, new TabInfo(LocalManager.GetValue(LocalHashConst.house_keeper_daily),
               this.viewPref.pnlDaily, (state) => {
                   OnToggleValueChange(HouseKeeperInfoType.Daily, state);
               }));
            this.viewPref.tabView.SetTab(
                1, new TabInfo(LocalManager.GetValue(LocalHashConst.house_keeper_build),
                this.viewPref.pnlBuild, (state) => {
                    OnToggleValueChange(HouseKeeperInfoType.Build, state);
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

        private void OnToggleValueChange(HouseKeeperInfoType HouseKType, bool state) {
            if (state) {
                this.viewModel.Channel = HouseKType;
            }
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
