using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class DailyRewardView: BaseView {
        private DailyRewardViewModel viewModel;
        private DailyRewardViewPeference viewPref;
        /* UI Members*/
        private int allLoginRewardCount;
        /*************/

        protected override void OnUIInit() {
            GameObject viewHoldler = UIManager.GetUI("UICampaign.PnlCampaign.PnlDailyRewardHoldler");
            PrefabLoader viewLoadler = viewHoldler.GetComponent<PrefabLoader>();
            this.ui = viewLoadler.LoadSubPrefab();
            //this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlDailyRewardHoldler");
            this.viewModel = this.gameObject.GetComponent<DailyRewardViewModel>();
            this.viewPref = this.ui.transform.GetComponent<DailyRewardViewPeference>();
            /* Cache the ui components here */
        }

        /* Propert change function */


        public void SetDailyRewardInfo() {
            this.allLoginRewardCount = LoginRewardConf.GetAllLoginRewardDey();
            LoginRewardConf loginRewardConf = new LoginRewardConf();
            GameHelper.ResizeChildreCount(this.viewPref.PnlList,
                this.allLoginRewardCount, PrefabPath.pnlDailyRewardItem);
            DailyRewardItemView itemView = null;
            LoginRewardAck loginRewardAck = this.viewModel.LoginRewardAck;
            int rewardDays = loginRewardAck.RewardDays;
            bool isReceived = false;
            bool todayStatus = loginRewardAck.TodayStatus;
            for (int i = 1; i < this.allLoginRewardCount + 1; i++) {
                int index = i;
                bool isToday = index == rewardDays;
                if (index < rewardDays) {
                    isReceived = true;
                } else if (isToday && todayStatus == true) {
                    isReceived = true;
                } else {
                    isReceived = false;
                }
                loginRewardConf = LoginRewardConf.AllLoginRewardDict[index];
                itemView = this.viewPref.PnlList.GetChild(index - 1).
                    GetComponent<DailyRewardItemView>();
                itemView.SetToDeyRewardItemInfo(
                    loginRewardConf, isToday, isReceived, todayStatus);
                LoginRewardConf loginReward = loginRewardConf;
                bool isShowReceived = isToday && !todayStatus;
                if (isShowReceived) {
                    itemView.OnShowContentClick.AddListener(() => {
                        this.viewModel.ShowRewardView(loginReward);
                    });
                } else {
                    itemView.OnShowContentClick.AddListener(() => {
                        this.viewModel.ShowNoRewardView(loginReward);
                    });
                }
            }
        }

        /***************************/

    }
}
