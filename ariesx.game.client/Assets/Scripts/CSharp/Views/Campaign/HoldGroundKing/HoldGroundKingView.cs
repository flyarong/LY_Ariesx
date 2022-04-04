using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class HoldGroundKingView : BaseView {
        private HoldGroundKingViewModel viewModel;
        private HoldGroundKingViewPeference viewPref;
        /* UI Members*/

        /*************/
        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<HoldGroundKingViewModel>();
            GameObject viewHoldler = UIManager.GetUI("UICampaign.PnlCampaign.PnlHoldGroundKingHoldler");
            PrefabLoader viewLoadler = viewHoldler.GetComponent<PrefabLoader>();
            this.ui = viewLoadler.LoadSubPrefab();
            //this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlHoldGroundKing");
            this.viewPref = this.ui.transform.GetComponent<HoldGroundKingViewPeference>();

            this.viewPref.onlyTabsView.InitTab(3);
            this.viewPref.onlyTabsView.SetTab(0, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.campaign_detail),
                this.viewPref.holdGroundKingDetails, (state) => {
                    this.OnToggleValueChange(HoldGroundKingType.Detail, state);
                }));
            this.viewPref.onlyTabsView.SetTab(1, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.campaign_points_rank),
                this.viewPref.holdGroundKingRanking, (state) => {
                    this.OnToggleValueChange(HoldGroundKingType.Rank, state);
                }));
            this.viewPref.onlyTabsView.SetTab(2, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.occupy_points_detail),
                this.viewPref.holdGroundKingStatistics, (state) => {
                    this.OnToggleValueChange(HoldGroundKingType.Statistics, state);
                }));
            this.viewPref.onlyTabsView.SetAllOff();
        }
        public void SetTabInfo() {
            this.viewPref.onlyTabsView.SetActiveTab(this.GetTabIndex());
            this.viewModel.customContentSizeFitterSettle();
        }

        private int GetTabIndex() {
            switch (this.viewModel.KingType) {
                case HoldGroundKingType.Detail:
                    return 0;
                case HoldGroundKingType.Rank:
                    return 1;
                case HoldGroundKingType.Statistics:
                    return 2;
                default:
                    return 0;
            }
        }

        public void SetToggleInteractable(bool isEnable) {
            this.viewPref.onlyTabsView.SetToggleInteractable(isEnable);
        }

        /* Propert change function */
        private void OnToggleValueChange(HoldGroundKingType viewType, bool state) {
            if (state && (this.viewModel.KingType != viewType)) {
                this.viewModel.KingType = viewType;
            }
        }

    }
}
