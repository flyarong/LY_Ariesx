using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class HoldGroundKingDetailsView : BaseView {
        private HoldGroundKingDetailsViewModel viewModel;
        private HoldGroundKingDetailsViewPeference viewPref;
        /* UI Members*/
        //private List<IntegralReward> AllRewards = new List<IntegralReward>(12);
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<HoldGroundKingDetailsViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlHoldGroundKingHoldler.PnlHoldGroundKingDetails");
            this.viewPref = this.ui.transform.GetComponent<HoldGroundKingDetailsViewPeference>();
        }

        public void CustomContentSizeFitterSettle() {
            if (this.IsUIInit) {
                this.viewPref.customContentSizeFitter.enabled = false;
                this.viewPref.customContentSizeFitter.enabled = true;
            }
        }

        public void HideStateRewardDetail() {
            this.viewPref.stageRewardView.HideStateRewardDetail();
        }

        public void SetHoldGroundKingOwnInfo(int ownPoints) {
            if (this.viewModel.ChosenActivity.Occupy != null) {
                this.viewPref.stageRewardView.SetStageInfo(
                this.viewModel.ChosenActivity.Occupy.IntegralReward,
                ownPoints, showCanmpaignTips: true);
            }

            this.SetDevilFightingGuidVisible(
                this.viewModel.ChosenActivity.Status != Activity.ActivityStatus.Finish);
        }

        private void SetDevilFightingGuidVisible(bool isVisible) {
            UIManager.SetUICanvasGroupVisible(this.viewPref.HoldGroundKingGuidLabelCG, isVisible);
            UIManager.SetUICanvasGroupEnable(this.viewPref.HoldGroundKingGuidContentCG, isVisible);
        }

        private void SetOccupyPointList(OccupyPointType type,
            List<OccupyPointsConf> pointList) {
            Transform root = null;
            switch (type) {
                case OccupyPointType.Resoure:
                    root = this.viewPref.pnlGroundIntegralGuide;
                    break;
                case OccupyPointType.Bridge:
                    root = this.viewPref.pnlBridgeGuide;
                    break;
                case OccupyPointType.NpcCity:
                    root = this.viewPref.pnlMarginCityGuide;
                    break;
                default:
                    return;
            }
            //GameHelper.ClearChildren(root);
            int pointListCount = pointList.Count;
            GameHelper.ResizeChildreCount(root, pointListCount,
                PrefabPath.pnlContinentDisputesIntegralItem);
            DisputesIntegralItemView itemView;
            OccupyPointsConf item;
            // Debug.LogError(root.name+" "+ pointListCount);
            for (int index = 0; index < pointListCount; index++) {
                itemView = root.GetChild(index).GetComponent<DisputesIntegralItemView>();
                item = pointList[index];
                itemView.SetContent(item.level.ToString(), item.point);
            }
        }


        public void SetContent() {
            Dictionary<OccupyPointType, List<OccupyPointsConf>> occupyPointConfDict =
                OccupyPointsConf.GetOccupyPointDict();
            foreach (var pair in occupyPointConfDict) {
                this.SetOccupyPointList(pair.Key, pair.Value);
            }
        }

    }
}
