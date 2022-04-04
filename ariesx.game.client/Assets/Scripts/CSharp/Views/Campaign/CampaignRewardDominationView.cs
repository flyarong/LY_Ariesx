using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class CampaignRewardDominationView : BaseView {
        private CampaignRewardDominationViewModel viewModel;
        private CampaignRewardDominationViewPreference viewPref;
        /* UI Members*/

        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<CampaignRewardDominationViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UICampaignRewardDomination");
            /* Cache the ui components here */
            this.viewPref = this.ui.transform.GetComponent<CampaignRewardDominationViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
        }

        /// <summary>
        /// 设置恶魔入侵的奖励详情
        /// </summary>
        internal void SetContent(List<Domination> dominations) {
            //Debug.LogError("+++"+dominations.Count);
            Domination domination = null;
            CampaignRewardItem itemView = null;
            for (int i = 0; i < dominations.Count; i++) {
                domination = dominations[i];
                int rewardsCount = dominations.Count;
                GameHelper.ResizeChildreCount(this.viewPref.pnlList,
                    rewardsCount, PrefabPath.pnlCampaignRewardItemTitle);//
                //排名奖励
                Transform rewardItemTitle = this.viewPref.pnlList.GetChild(i);
                int rewardCount = domination.RankRewared.Count;
                GameHelper.ResizeChildreCount(rewardItemTitle,
                    rewardCount + 2, PrefabPath.pnlCampaignRewardItem);
                for (int k = 0; k < rewardCount; k++) {
                    itemView = rewardItemTitle.GetChild(k).GetComponent<CampaignRewardItem>();
                    itemView.OnHeroClick = this.viewModel.ShowHeroInfo;
                    itemView.SetContent(domination.RankRewared[k]);                    
                }

                itemView = rewardItemTitle.GetChild(rewardCount).GetComponent<CampaignRewardItem>();
                itemView.SetContentLastBloodReward(domination.LastBloodReward);
                itemView = rewardItemTitle.GetChild(rewardCount + 1).GetComponent<CampaignRewardItem>();
                itemView.SetContentAllianceReward(domination.AllianceReward);

                GameObject rewardTitle = PoolManager.GetObject(PrefabPath.pnlCampaignRewardTitle, rewardItemTitle);
                rewardTitle.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text =
                    string.Format(LocalManager.GetValue(LocalHashConst.domination_defeat), dominations[i].Level);
                rewardTitle.transform.SetAsFirstSibling();
            }
            this.viewPref.rectTransform.anchoredPosition = Vector2.zero;
        }

        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
