using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class ContinentDisputesView : BaseView {
        private ContinentDisputesViewModel viewModel;
        private ContinentDisputesViewPreference viewPref;
        /* UI Members*/

        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<ContinentDisputesViewModel>();
        }

        protected override void OnUIInit() {
            GameObject viewHoldler = UIManager.GetUI("UICampaign.PnlCampaign.PnlContinentDisputesHoldler");
            PrefabLoader viewLoadler = viewHoldler.GetComponent<PrefabLoader>();
            //this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlContinentDisputesHoldler");
            this.ui = viewLoadler.LoadSubPrefab();
            this.viewPref = this.ui.transform.GetComponent<ContinentDisputesViewPreference>();

            this.viewPref.onlyTabsView.InitTab(3);
            this.viewPref.onlyTabsView.SetTab(0, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.campaign_detail),
                this.viewPref.continentDisputesDetails, (state) => {
                    this.OnToggleValueChange(ContinentDisputesViewType.Detail, state);
                }));
            this.viewPref.onlyTabsView.SetTab(1, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.campaign_points_rank),
                this.viewPref.continentDisputesRanking, (state) => {
                    this.OnToggleValueChange(ContinentDisputesViewType.Rank, state);
                }));
            this.viewPref.onlyTabsView.SetTab(2, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.occupy_points_detail),
                this.viewPref.pnlcontinentDisputesStatisticsCG, (state) => {
                    this.OnToggleValueChange(ContinentDisputesViewType.Statistics, state);
                }));
            this.viewPref.onlyTabsView.SetAllOff();
        }
        private void OnToggleValueChange(ContinentDisputesViewType viewType, bool state) {
            if (state && (this.viewModel.ViewType != viewType)) {
                this.viewModel.ViewType = viewType;
            }
        }
        public void SetTabsInfo() {
            this.viewPref.onlyTabsView.SetActiveTab(this.GetTabIndex());
        }
        private int GetTabIndex() {
            switch (this.viewModel.ViewType) {
                case ContinentDisputesViewType.Detail:
                    return 0;
                case ContinentDisputesViewType.Rank:
                    return 1;
                case ContinentDisputesViewType.Statistics:
                    return 2;
                default:
                    return 0;
            }
        }


        public void SetToggleInteractable(bool isEnable) {
            this.viewPref.onlyTabsView.SetToggleInteractable(isEnable);
        }
    }
}
