using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class DemonShadowView : BaseView {
        private DemonShadowViewModel viewModel;
        private DemonShadowViewPeference viewPref;
        /* UI Members*/

        /*************/

        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<DemonShadowViewModel>();
            GameObject viewHoldler = UIManager.GetUI("UICampaign.PnlCampaign.PnlDemonShadowHoldler");
            PrefabLoader viewLoadler = viewHoldler.GetComponent<PrefabLoader>();
            this.ui = viewLoadler.LoadSubPrefab();
            //this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlDemonShadow");
            /* Cache the ui components here */
            this.viewPref = this.ui.transform.GetComponent<DemonShadowViewPeference>();
            //Tabs设置
            this.viewPref.onlyTabsView.SetTab(0, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.campaign_detail),
                this.viewPref.pnlDemonShadowDetails, (state) => {
                    this.OnToggleValueChange(DemonShadownType.Detail, state);
                }));
            this.viewPref.onlyTabsView.SetTab(1, new TabTogsInfo(
                LocalManager.GetValue(LocalHashConst.domination_history),
                this.viewPref.pnlDemonShadowHistory, (state) => {
                    this.OnToggleValueChange(DemonShadownType.History, state);
                }));
            this.viewPref.onlyTabsView.SetAllOff();
        }

        /* Propert change function */

        /***************************/

        public void SetTabInfo() {
            this.viewPref.onlyTabsView.SetActiveTab(this.GetTabIndex());
        }

        private int GetTabIndex() {
            switch (this.viewModel.DemonType) {
                case DemonShadownType.Detail:
                    return 0;
                case DemonShadownType.History:
                    return 1;
                default:
                    return 0;
            }
        }

        public void SetToggleInteractable(bool isEnable) {
            this.viewPref.onlyTabsView.SetToggleInteractable(isEnable);
        }

        private void OnToggleValueChange(DemonShadownType viewType, bool state) {
            if (state && (this.viewModel.DemonType != viewType)) {
                this.viewModel.DemonType = viewType;
            }
        }
    }
}
