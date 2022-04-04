using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class HoldGroundKingRankingView: BaseView {
        private HoldGroundKingRankingViewModel viewModel;
        private HoldGroundKingRankingViewPeference viewPref;
        /* UI Members*/

        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<HoldGroundKingRankingViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlHoldGroundKingHoldler.PnlHoldGroundKingRank");
            this.viewPref = this.ui.transform.GetComponent<HoldGroundKingRankingViewPeference>();
            this.viewPref.pnlBtnShowDetail.onClick.AddListener(this.OnBtnHlodCampaignRewardsClick);
            this.viewPref.btnShowDetail.onClick.AddListener(this.OnBtnHlodCampaignRewardsClick);

        }

        public void SetRewardBasicContent() {
            int count = this.viewModel.RewardBasicContent.Occupy.RankReward.Count;
            if (count > 0) {
                this.viewPref.firstRankReward.OnHeroClick = this.viewModel.ShowHeroInfo;
                UIManager.SetUICanvasGroupEnable(
                this.viewPref.firstRankRewardCanvasGroup, true);
                this.viewPref.firstRankReward.SetContent(
                    this.viewModel.RewardBasicContent.Occupy.RankReward[0].Reward);

            } else {
                UIManager.SetUICanvasGroupEnable(
                this.viewPref.firstRankRewardCanvasGroup, false);
            }
            if (count > 1) {
                this.viewPref.secondRankReward.OnHeroClick = this.viewModel.ShowHeroInfo;
                UIManager.SetUICanvasGroupEnable(
                this.viewPref.secondRankRewardCanvasGroup, true);
                this.viewPref.secondRankReward.SetContent(
                    this.viewModel.RewardBasicContent.Occupy.RankReward[1].Reward);

            } else {
                UIManager.SetUICanvasGroupEnable(
                this.viewPref.secondRankRewardCanvasGroup, false);
            }
            if (count > 2) {
                this.viewPref.thirdRankReward.OnHeroClick = this.viewModel.ShowHeroInfo;
                UIManager.SetUICanvasGroupEnable(
                this.viewPref.thirdRankRewardCanvasGroup, true);
                this.viewPref.thirdRankReward.SetContent(
                    this.viewModel.RewardBasicContent.Occupy.RankReward[2].Reward);
            } else {
                UIManager.SetUICanvasGroupEnable(
                this.viewPref.thirdRankRewardCanvasGroup, false);
            }
            this.SetRankInfo();
        }

        /* Propert change function */
        private void OnBtnHlodCampaignRewardsClick() {
            this.viewModel.OnRewardsDetailClick();
        }

        /***************************/
        private void SetRankInfo() {
            GameHelper.ClearChildren(this.viewPref.pnlOwnRankList);
            int rankListCount = this.viewModel.rankOccupyList.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlOwnRankList,
                rankListCount, PrefabPath.pnlCommonActivityRankItem);
            DevilFightingRankItemView rankItemView = null;
            RankOccupy rankData = null;
            for (int i = 0; i < rankListCount; i++) {
                int index = i;
                rankData = this.viewModel.rankOccupyList[index];
                string playerId = rankData.Id;
                rankItemView =
                    this.viewPref.pnlOwnRankList.GetChild(index).GetComponent<DevilFightingRankItemView>();
                rankItemView.OnPlayerInfoClick.AddListener(() => {
                    this.ShowPlayerInfoClick(playerId);});
                rankItemView.SetContent(rankData.Rank, rankData.Name, rankData.Points);
            }

            if (this.viewModel.SelfRank != null) {
                this.viewPref.txtOwnRank.text = string.Format(
                    LocalManager.GetValue(LocalHashConst.campaign_rank_label), this.viewModel.SelfRank.Rank);
                this.viewPref.txtPoint.text = this.viewModel.SelfRank.Points.ToString();
            }
            this.viewPref.txtOwnName.text = RoleManager.GetRoleName();
        }
        private void ShowPlayerInfoClick(string playerId) {
            this.viewModel.ShowPlayerInfoClick(playerId);
        }
    }
}
