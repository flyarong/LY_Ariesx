using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class ContinentDisputesDetailsView : BaseView {
        private ContinentDisputesDetailsViewModel viewModel;
        private ContinentDisputesDetailsViewPeference viewPref;

        /******************************************************/

        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<ContinentDisputesDetailsViewModel>();
            this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlContinentDisputes.PnlContinentDisputesDetails");
            this.viewPref = this.ui.transform.GetComponent<ContinentDisputesDetailsViewPeference>();
        }

        /* Propert change function */
        public void SetContinentDisputesOwnInfo(int ownPoints) {
            this.viewPref.panelStageRewardsView.SetStageInfo(
                this.viewModel.ChosenActivity.Capture.IntegralReward,
                ownPoints, showCanmpaignTips: true);

            //this.viewPref.txtCampaignTitle.text =
            //    this.viewModel.ChosenActivity.GetActivitySubject();

            this.SetDevilFightingGuidVisible(
                this.viewModel.ChosenActivity.Status != Activity.ActivityStatus.Finish);
        }

        private void SetDevilFightingGuidVisible(bool isVisible) {
            UIManager.SetUICanvasGroupVisible(
                this.viewPref.ContinentDisputesGuidLabelCG, isVisible);
            UIManager.SetUICanvasGroupEnable(
                this.viewPref.ContinentDisputesGuidContentCG, isVisible);
        }

        public void HideStateRewardDetail() {
            this.viewPref.panelStageRewardsView.HideStateRewardDetail();
        }

        public void SetContent() {
            Dictionary<CapturePointType, List<CapturePointsConf>> OccupyPointDict =
                CapturePointsConf.GetapturePointDict();
            foreach (var pair in OccupyPointDict) {
                this.SetCapturePointList(pair.Key, pair.Value);
            }
        }

        private void SetCapturePointList(
            CapturePointType pointType, List<CapturePointsConf> pointList) {
            Transform root = null;
            switch (pointType) {
                case CapturePointType.Resoure:
                    root = this.viewPref.pnlLandPointsList;
                    break;
                case CapturePointType.Pass:
                    root = this.viewPref.pnlPassPointsList;
                    break;
                default:
                    return;
            }
            //GameHelper.ClearChildren(root);
            GameHelper.ResizeChildreCount(root, pointList.Count,
                PrefabPath.pnlContinentDisputesIntegralItem);
            DisputesIntegralItemView itemView;
            int index = 0;
            foreach (CapturePointsConf item in pointList) {
                itemView = root.GetChild(index++).
                    GetComponent<DisputesIntegralItemView>();
                itemView.SetContent(item.level.ToString(), item.point);
            }
        }
    }
}

/***************************/
