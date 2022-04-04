using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;

namespace Poukoute {
    public class DevilFightingRankView: BaseView {
        private DevilFightingRankViewModel viewModel;
        private DevilFightingRankViewPreference viewPref;


        /*************/
        void Awake() {
            this.viewModel = this.gameObject.GetComponent<DevilFightingRankViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlDevilFightingHoldler.PnlDevilFightingRank");
            this.viewPref = this.ui.transform.GetComponent<DevilFightingRankViewPreference>();
            this.viewPref.btnRankRewardsDetail.onClick.AddListener(this.OnRewardsDetailClick);
            this.viewPref.btnShowMoreDetail.onClick.AddListener(this.OnRewardsDetailClick);
        }

        public void SetContent() {
            int count = this.viewModel.ChosenActivity.Melee.RankReward.Count;

            if (count > 0) {
                this.viewPref.firstRankReward.OnHeroClick = this.viewModel.ShowHeroInfo;
                UIManager.SetUICanvasGroupEnable(this.viewPref.firstRankRewardCG, true);
                this.viewPref.firstRankReward.SetContent(
                    this.viewModel.ChosenActivity.Melee.RankReward[0].Reward);                
            } else {
                UIManager.SetUICanvasGroupEnable(this.viewPref.firstRankRewardCG, false);
            }

            if (count > 1) {
                this.viewPref.secondRankReward.OnHeroClick = this.viewModel.ShowHeroInfo;
                UIManager.SetUICanvasGroupEnable(this.viewPref.secondRankRewardCG, true);
                this.viewPref.secondRankReward.SetContent(
                    this.viewModel.ChosenActivity.Melee.RankReward[1].Reward);                
            } else {
                UIManager.SetUICanvasGroupEnable(this.viewPref.secondRankRewardCG, false);
            }

            if (count > 2) {
                this.viewPref.thirdRankReward.OnHeroClick = this.viewModel.ShowHeroInfo;
                UIManager.SetUICanvasGroupEnable(this.viewPref.thirdRankRewardCG, true);
                this.viewPref.thirdRankReward.SetContent(
                    this.viewModel.ChosenActivity.Melee.RankReward[2].Reward);                
            } else {
                UIManager.SetUICanvasGroupEnable(this.viewPref.thirdRankRewardCG, false);
            }
            this.SetRankInfo();
        }

        private void SetRankInfo() {
            int rankListCount = this.viewModel.RankList.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlList,
                rankListCount, PrefabPath.pnlCommonActivityRankItem);
            DevilFightingRankItemView rankItemView = null;
            RankMelee rankData = null;
            for (int i = 0; i < rankListCount; i++) {
                rankData = this.viewModel.RankList[i];
                string playerId = rankData.Id;
                rankItemView =
                    this.viewPref.pnlList.GetChild(i).GetComponent<DevilFightingRankItemView>();
                rankItemView.SetContent(rankData.Rank, rankData.Name, rankData.Points);
                rankItemView.OnPlayerInfoClick.AddListener(()=> {
                    this.ShowPlayerInfoClick(playerId);
                });
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

        private void OnRewardsDetailClick() {
            this.viewModel.OnRewardsDetailClick();
        }

    }
}
