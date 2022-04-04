using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class ContinentDisputesRankView: BaseView {
        private ContinentDisputesRankViewModel viewModel;
        private ContinentDisputesRankViewPeference viewPref;
        /* UI Members*/

        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<ContinentDisputesRankViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlContinentDisputes.PnlContinentDisputesRank");
            this.viewPref = this.ui.transform.GetComponent<ContinentDisputesRankViewPeference>();
            this.viewPref.pnlBtnShowDetail.onClick.AddListener(this.OnBtnShowRankReward);
            this.viewPref.btnShowDetail.onClick.AddListener(this.OnBtnShowRankReward);
            /* Cache the ui components here */
        }

        private void OnBtnShowRankReward() {
            this.viewModel.OnRewardsDetailClick();
        }

        public void SetRewardBasicContent() {
            int count = this.viewModel.RewardBasicContent.RankRewards.Count;
            if (count > 0) {
                this.viewPref.firstRankReward.OnHeroClick = this.viewModel.ShowHeroInfo;
                UIManager.SetUICanvasGroupEnable(
                    this.viewPref.firstRankRewardCanvasGroup, true);
                this.viewPref.firstRankReward.SetContent(
                    this.viewModel.RewardBasicContent.Capture.RankReward[0].Reward);                
            } else {
                UIManager.SetUICanvasGroupEnable(
                    this.viewPref.firstRankRewardCanvasGroup, false);
            }
            if (count > 1) {
                this.viewPref.secondRankReward.OnHeroClick = this.viewModel.ShowHeroInfo;
                UIManager.SetUICanvasGroupEnable(
                    this.viewPref.secondRankRewardCanvasGroup, true);
                this.viewPref.secondRankReward.SetContent(
                    this.viewModel.RewardBasicContent.Capture.RankReward[1].Reward);                
            } else {
                UIManager.SetUICanvasGroupEnable(
                    this.viewPref.secondRankRewardCanvasGroup, false);
            }
            if (count > 2) {
                this.viewPref.thirdRankReward.OnHeroClick = this.viewModel.ShowHeroInfo;
                UIManager.SetUICanvasGroupEnable(
                    this.viewPref.thirdRankRewardCanvasGroup, true);
                this.viewPref.thirdRankReward.SetContent(
                    this.viewModel.RewardBasicContent.Capture.RankReward[2].Reward);
                
            } else {
                UIManager.SetUICanvasGroupEnable(
                    this.viewPref.thirdRankRewardCanvasGroup, false);
            }
            this.SetRankInfo();
        }

        /* Propert change function */
        private void SetRankInfo() {
            int rankListCount = this.viewModel.rankCaptureList.Count;
            GameHelper.ClearChildren(this.viewPref.pnlOwnAllianceRankList);
            GameHelper.ResizeChildreCount(this.viewPref.pnlOwnAllianceRankList,
                rankListCount, PrefabPath.pnlAllianceRankingBase);
            AllianceRankingBaseView rankItemView = null;
            RankCapture rankData = null;
            for (int i = 0; i < rankListCount; i++) {
                int index = i;
                rankData = this.viewModel.rankCaptureList[index];
                int log = rankData.MapSn;
                string allianceId = rankData.Id;
                rankItemView =
                    this.viewPref.pnlOwnAllianceRankList.GetChild(index).GetComponent<AllianceRankingBaseView>();
                
                rankItemView.OnItemClick.AddListener(() => {
                    this.ShowCampaignAllianceInfoClick(allianceId);
                });
                rankItemView.SetContent(rankData.Rank, rankData.Name, rankData.Points
                    , rankData.Emblem, log);
            }

            Alliance alliance = RoleManager.GetAlliance();
            bool inAlliance = (alliance != null) && !alliance.Id.CustomIsEmpty();
            this.viewPref.imgAllianceBG.gameObject.SetActiveSafe(inAlliance);
            this.viewPref.txtstateName.gameObject.SetActiveSafe(inAlliance);
            if (inAlliance) {
                if (this.viewModel.selfRank.Rank != 0) {
                    this.viewPref.txtOwnAllianceRank.text = string.Format(
                        LocalManager.GetValue(LocalHashConst.campaign_rank_label), this.viewModel.selfRank.Rank);
                }
                this.viewPref.txtPoint.text = GameHelper.GetFormatNum(this.viewModel.selfRank.Points);
                this.viewPref.txtOwnAllianceName.text = this.viewModel.selfRank.Name;
                int logoId = this.viewModel.selfRank.Emblem;
                int log = this.viewModel.selfRank.MapSn;
                this.viewPref.imgAlliance.sprite = ArtPrefabConf.GetAliEmblemSprite(logoId);
                this.viewPref.txtstateName.text = NPCCityConf.GetMapSNLocalName(log);
            } else {
                this.viewPref.txtOwnAllianceName.text = LocalManager.GetValue(LocalHashConst.not_in_alliance);
            }
        }
        private void ShowCampaignAllianceInfoClick(string allianceId) {
            this.viewModel.ShowCampaignAllianceInfoClick(allianceId);
        }
        /***************************/

    }
}
