using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceDetailView: BaseView {
        private AllianceDetailViewModel viewModel;
        private AllianceDetailViewPreference viewPref;
        //private Vector2 tabBtnClose = new Vector2(340, -60);
        //public class TabEvent: UnityEvent<bool> { }
        /*****************************************************/

        //void Awake() {
        //    this.viewModel = this.gameObject.GetComponent<AllianceDetailViewModel>();
        //}

        //public override void Show(bool needHideBack = false, UnityAction callback = null) {
        //    base.Show();
        //}

        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<AllianceDetailViewModel>();
            this.ui = UIManager.GetUI("UIAlliance.PnlAlliance");
            this.viewPref = this.ui.transform.GetComponent<AllianceDetailViewPreference>();
            this.viewPref.tabView.InitTab(3);
            this.viewPref.tabView.SetTab(0, new TabInfo(
                LocalManager.GetValue(LocalHashConst.alliance_subtitle_alliance),
                null, (state) => {
                    OnToggleValueChange(AllianceViewType.Alliance, state);
                }));
            this.viewPref.tabView.SetTab(1, new TabInfo(
                LocalManager.GetValue(LocalHashConst.alliance_subtitle_city),
                null, (state) => {
                    OnToggleValueChange(AllianceViewType.City, state);
                }));

            this.viewPref.tabView.SetTab(2, new TabInfo(
                LocalManager.GetValue(LocalHashConst.alliance_subtitle_subordinate),
                null, (state) => {
                    OnToggleValueChange(AllianceViewType.Subordinate, state);
                }));
            this.viewPref.tabView.SetAllOff();
        }

        public void SetAllianceDetailView() {
            this.viewPref.tabView.SetCloseCallBack(this.OnBtnCloseClick);
            this.viewPref.tabView.SetActiveTab(this.GetChannelTabIndex());
        }

        public void SetReturn() {
            this.viewPref.tabView.SetAllOff();
        }

        private int GetChannelTabIndex() {
            switch (this.viewModel.Channel) {
                case AllianceViewType.Alliance:
                    return 0;
                case AllianceViewType.City:
                    return 1;
                case AllianceViewType.Subordinate:
                    return 2;
                default:
                    return 0;
            }
        }

        private void OnToggleValueChange(AllianceViewType allianceViewType, bool state) {
            if (state) {
                this.viewModel.Channel = allianceViewType;
            }
        }

        protected void OnBtnCloseClick() {
            this.viewModel.HideAllianceView();
        }

        private void OnAllianceInfoClose() {
            this.viewModel.HideAllianceDetail();
        }

        private void HideDetailViewPanels() {
            this.viewModel.Channel = AllianceViewType.None;
            this.viewPref.tabView.SetAllOff();
        }
    }
}
