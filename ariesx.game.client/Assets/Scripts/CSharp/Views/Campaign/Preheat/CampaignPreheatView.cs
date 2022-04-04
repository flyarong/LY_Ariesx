using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;

namespace Poukoute {
    public class CampaignPreheatView : BaseView {
        private CampaignPreheatViewModel viewModel;
        private CampaignPreheatViewPreference viewPref;

        /*************/

        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<CampaignPreheatViewModel>();
            GameObject viewHoldler = UIManager.GetUI("UICampaign.PnlCampaign.PnlPreheatHoldler");
            PrefabLoader viewLoadler = viewHoldler.GetComponent<PrefabLoader>();
            this.ui = viewLoadler.LoadSubPrefab();
            //this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlPreheat");
            this.viewPref = this.ui.transform.GetComponent<CampaignPreheatViewPreference>();
            this.viewPref.BtnCampaignRewards.onClick.AddListener(this.OnBtnCampaignRewardsClick);
            this.viewPref.btnCampaignRule.onClick.AddListener(this.OnBtnCampaignRuleClick);
        }

        public void SetContent(CampaignType type) {
            switch (type) {
                case CampaignType.melee:
                    this.viewPref.txtCampaignDesc.text = string.Format(
                        LocalManager.GetValue(LocalHashConst.melee_prepare_title),
                        CampaignModel.MonsterLocalName
                    );
                    break;
                case CampaignType.occupy:
                    this.viewPref.txtCampaignDesc.text =
                        LocalManager.GetValue(LocalHashConst.occupy_prepare_title);
                    break;
                case CampaignType.capture:
                    this.viewPref.txtCampaignDesc.text =
                        LocalManager.GetValue(LocalHashConst.capture_prepare_title);
                    break;
                case CampaignType.domination:
                    this.viewPref.txtCampaignDesc.text =
                        LocalManager.GetValue(LocalHashConst.domination_detail_title);
                    break;
                case CampaignType.none:
                    break;
                default:
                    this.viewPref.txtCampaignDesc.text = string.Empty;
                    break;
            }
            //this.SetCampaigRemainTime();
            this.SetCampaigRewardsTypes();
        }

        public void SetCampaignRemainTimeInfo() {
            if (this.viewModel.CampaigRemainTime < 0) {
                this.viewModel.OnCampaignPreheatDone();
                Debug.LogError("Unregist CampaignPreheat");
                UpdateManager.Unregist(UpdateInfo.CampaignPreheat);
                return;
            }
            this.viewModel.CampaigRemainTime =
                this.viewModel.ChosenActivity.Base.StartTime - RoleManager.GetCurrentUtcTime() / 1000;
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(this.viewModel.CampaigRemainTime);
            //Debug.LogError("CampaigRemainTime " + this.viewModel.CampaigRemainTime + " " + timeSpan.Days);
            bool isTimeMoreThanOneDay = timeSpan.Days > 0;
            if (isTimeMoreThanOneDay) {
                this.viewPref.countDownDayView.SetContent(timeSpan.Days);
                Debug.LogError("Unregist CampaignPreheat");
                UpdateManager.Unregist(UpdateInfo.CampaignPreheat);
            } else {
                this.viewPref.countDownHourView.SetContent(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }
            UIManager.SetUICanvasGroupVisible(this.viewPref.countDownDayCG, isTimeMoreThanOneDay);
            UIManager.SetUICanvasGroupVisible(this.viewPref.countDownHourCG, !isTimeMoreThanOneDay);
        }

        private void SetCampaigRewards(List<ItemType> rewardItemType) {
            int rewardsCount = rewardItemType.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlRewardsList,
                rewardsCount, PrefabPath.pnlItemWithCount);
            ItemWithCountView itemView = null;
            int i = 0;
            for (; i < rewardsCount; i++) {
                itemView = this.viewPref.pnlRewardsList.GetChild(i).GetComponent<ItemWithCountView>();
                itemView.SetResourceInfo(rewardItemType[i]);
            }
        }

        private void SetCampaigRewardsTypes() {
            List<ItemType> campaginRewardType = new List<ItemType>(5);
            List<ItemType> tmpRewardType = new List<ItemType>(5);
            if (this.viewModel.ChosenActivity.CampaignType == CampaignType.domination) {
                this.viewModel.DominationRewardList = this.viewModel.ChosenActivity.Domination;
                foreach (Domination donimation in this.viewModel.DominationRewardList) {
                    LastBloodReward lastBloodReward = donimation.LastBloodReward;
                    tmpRewardType = lastBloodReward.Reward.GetRewardItemTypes();
                    this.MergeRewardsTypes(campaginRewardType, tmpRewardType);

                    AllianceReward allianceReward = donimation.AllianceReward;
                    tmpRewardType = allianceReward.Reward.GetRewardItemTypes();
                    this.MergeRewardsTypes(campaginRewardType, tmpRewardType);

                    List<RankReward> rankRewards = donimation.RankRewared;
                    foreach (RankReward rankReward in rankRewards) {
                        tmpRewardType = rankReward.Reward.GetRewardItemTypes();
                        this.MergeRewardsTypes(campaginRewardType, tmpRewardType);
                    }
                }
            } else {
                //int campaignRewardsCount = this.viewModel.ChosenActivity.IntegralReward.Count;
                List<Protocol.IntegralReward> IntegralRewards =
                    this.viewModel.ChosenActivity.IntegralReward;
                foreach (IntegralReward intergralReward in IntegralRewards) {
                    tmpRewardType = intergralReward.Reward.GetRewardItemTypes();
                    this.MergeRewardsTypes(campaginRewardType, tmpRewardType);
                }
                List<RankReward> rankRewards = this.viewModel.ChosenActivity.RankRewards;
                foreach (RankReward rankReward in rankRewards) {
                    tmpRewardType = rankReward.Reward.GetRewardItemTypes();
                    this.MergeRewardsTypes(campaginRewardType, tmpRewardType);
                }
            }
            this.SetCampaigRewards(campaginRewardType);
        }

        private void MergeRewardsTypes(List<ItemType> originTypes, List<ItemType> sourceTypes) {
            if (originTypes.Count >= 5) {
                return;
            }
            foreach (ItemType itemType in sourceTypes) {
                originTypes.TryAdd(itemType);
            }
        }

        private void OnBtnCampaignRewardsClick() {
            if (this.viewModel.ChosenActivity.CampaignType == CampaignType.domination) {
                this.viewModel.ShowDomination();
            } else {
                this.viewModel.ShowCampaignRewards();
            }

        }

        private void OnBtnCampaignRuleClick() {
            this.viewModel.ShowCampaignRules();
        }

        protected override void OnInvisible() {
            UpdateManager.Unregist(UpdateInfo.CampaignPreheat);
        }
    }
}
