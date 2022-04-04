using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;

namespace Poukoute {
    public class CampaignRewardView : BaseView {
        private CampaignRewardViewModel viewModel;
        private CampaignRewardViewPreference viewPref;


        /*************/
        void Awake() {
            this.viewModel = this.gameObject.GetComponent<CampaignRewardViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UICampaignReward");
            this.viewPref = this.ui.transform.GetComponent<CampaignRewardViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
        }

        public void SetContent() {
            int rewardCount = this.viewModel.ChoosedActivity.RankRewards.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlList,
                rewardCount, PrefabPath.pnlCampaignRewardItem);
            CampaignRewardItem itemView = null;
            for (int i = 0; i < rewardCount; i++) {
                itemView = this.viewPref.pnlList.GetChild(i).GetComponent<CampaignRewardItem>();
                itemView.OnHeroClick = this.viewModel.ShowHeroInfo;
                itemView.SetContent(this.viewModel.ChoosedActivity.RankRewards[i]);
            }
            this.viewPref.rectTransform.anchoredPosition = Vector2.zero;
        }

        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
