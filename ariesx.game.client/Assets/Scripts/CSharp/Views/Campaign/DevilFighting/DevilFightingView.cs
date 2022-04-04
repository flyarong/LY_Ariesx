using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;

namespace Poukoute {
    public class DevilFightingView : BaseView {
        private DevilFightingViewModel viewModel;
        private DevilFightingViewPreference viewPref;


        /*************/

        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<DevilFightingViewModel>();
            GameObject viewHoldler = UIManager.GetUI("UICampaign.PnlCampaign.PnlDevilFightingHoldler");
            PrefabLoader viewLoadler = viewHoldler.GetComponent<PrefabLoader>();
            this.ui = viewLoadler.LoadSubPrefab();
            //this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlDevilFighting");
            this.viewPref = this.ui.transform.GetComponent<DevilFightingViewPreference>();
            this.viewPref.onlyTabsView.SetTab(0, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.campaign_detail),
                this.viewPref.devilFightingDetailsCG, (state) => {
                    this.OnToggleValueChange(DevilFightingViewType.Detail, state);
                }));

            this.viewPref.onlyTabsView.SetTab(1, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.campaign_points_rank),
                this.viewPref.devilFightingRankCG, (state) => {
                    this.OnToggleValueChange(DevilFightingViewType.Rank, state);
                }));
            this.viewPref.onlyTabsView.SetAllOff();
        }

        public void SetContent() {
            this.viewPref.onlyTabsView.SetActiveTab(this.GetTabIndex());
        }

        private void OnToggleValueChange(DevilFightingViewType viewType, bool state) {
            if (state && (this.viewModel.ChannelType != viewType)) {
                this.viewModel.ChannelType = viewType;
            }
        }

        public void SetToggleInteractable(bool isEnable) {
            this.viewPref.onlyTabsView.SetToggleInteractable(isEnable);
        }

        private int GetTabIndex() {
            switch (this.viewModel.ChannelType) {
                case DevilFightingViewType.Detail:
                    return 0;
                case DevilFightingViewType.Rank:
                    return 1;
                default:
                    return 0;
            }
        }

    }
}
