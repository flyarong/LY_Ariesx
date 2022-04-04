using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceCreateOrJoinView : BaseView {
        private AllianceCreateOrJoinViewModel viewModel;
        private AllianceCreateOrJoinViewPreference viewPref;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<AllianceCreateOrJoinViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIAllianceCreateOrJoin");
            this.viewPref = this.ui.transform.GetComponent<AllianceCreateOrJoinViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnUnlockedClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.tabView.InitTab(2);
            this.viewPref.tabView.SetTab(0, new TabInfo(
                LocalManager.GetValue(LocalHashConst.search_alliance_title), this.viewPref.pnlSearch, (state) => {
                    OnToggleValueChange(AllianceViewType.Search, state);
                }));
            
            this.viewPref.tabView.SetTab(1, new TabInfo(
                LocalManager.GetValue(LocalHashConst.create_alliance_title), this.viewPref.pnlCreate, (state) => {
                    OnToggleValueChange(AllianceViewType.Create, state);
                }));
            this.viewPref.tabView.SetAllOff();
            this.viewPref.tabView.SetCloseCallBack(this.OnBtnCloseClick);
        }


        public override void PlayShow(UnityAction callback) {
            base.PlayShow(callback, true);
        }

        public override void PlayHide(UnityAction callback) {
            base.PlayHide(callback);
        }

        public void SetCreateOrJoinAllianceView(bool allianceUnlocked) {
            base.InitUI();
            UIManager.SetUICanvasGroupEnable(this.viewPref.allianceUnlockedCG, !allianceUnlocked);
            UIManager.SetUICanvasGroupEnable(this.viewPref.createOrJoinCG, allianceUnlocked);
            this.viewPref.showObj = 
                allianceUnlocked ? this.viewPref.createOrJoin : this.viewPref.unlockInfo;
            if (allianceUnlocked) {
                this.viewPref.tabView.SetAllOff();
                this.viewPref.tabView.SetActiveTab(0);
                this.viewModel.Channel = AllianceViewType.Search;
            } else {
                this.viewModel.Channel = AllianceViewType.Tips;
            }
        }


        public Transform GetSearchHead() {
            return this.viewPref.pnlSearchHead;
        }

        public void SetReturn() {
            this.viewPref.tabView.SetAllOff();
        }

        public void OnAllianceViewTypeChange() {
            this.viewPref.pnlSearchHead.gameObject.SetActiveSafe(
                this.viewModel.Channel == AllianceViewType.Search);
        }

        private void SetSearchHeadVisible(bool visible) {
            this.viewPref.pnlSearchHead.gameObject.SetActiveSafe(visible);
        }

        private void OnToggleValueChange(AllianceViewType allianceViewType, bool state) {
            if (state) {
                this.viewModel.Channel = allianceViewType;
            }
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        protected override void OnInvisible() {
            this.viewPref.tabView.SetAllOff();
            this.viewModel.Channel = AllianceViewType.None;
        }
    }
}
