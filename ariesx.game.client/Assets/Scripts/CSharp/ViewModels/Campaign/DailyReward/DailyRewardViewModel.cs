using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class DailyRewardViewModel : BaseViewModel {
        private CampaignViewModel parent;
        private DailyRewardView view;
        private CampaignModel model;

        /* Model data get set */
        public LoginRewardAck LoginRewardAck {
            get {
                return this.model.loginRewardAck;
            }
            set {
                if (this.model.loginRewardAck != value) {
                    this.model.loginRewardAck = value;
                }
            }
        }

        public int OpenServiceActivityCout {
            get {
                return this.model.OpenServiceActivityCout;
            }
            set {
                this.model.OpenServiceActivityCout = value;
            }
        }
        /**********************/

        /* Other members */
        private bool ShowOpenServiceActivityHUD;
        private long nextZeroTime = 0;
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<CampaignViewModel>();
            this.view = this.gameObject.AddComponent<DailyRewardView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void Show(bool EndCampaign = false) {
            this.view.Show();
            this.view.SetDailyRewardInfo();
        }

        public void Hide() {
            this.view.Hide();
        }

        public void ShowRewardView(LoginRewardConf loginReward) {
            this.parent.ShowLoginRewardReceivedView(loginReward);
        }

        public void ShowNoRewardView(
            LoginRewardConf rewardConf) {
            this.parent.ShowdailyRewardNoRewardView(rewardConf);
        }

        /* Add 'NetMessageAck' function here*/
        public void GetLoginRewardReq() {
            LoginRewardReq loginRewardReq = new LoginRewardReq();
            NetManager.SendMessage(loginRewardReq,
                typeof(LoginRewardAck).Name, this.GetLoginRewardAck);
        }

        private void GetLoginRewardAck(IExtensible message) {
            LoginRewardAck loginRewardAck = message as LoginRewardAck;
            this.LoginRewardAck = loginRewardAck;
            int loginRewardDeyCount = LoginRewardConf.GetAllLoginRewardDey();
            this.ShowOpenServiceActivityHUD = loginRewardAck.RewardDays > 0 &&
                loginRewardAck.RewardDays <= loginRewardDeyCount;
            this.OpenServiceActivityCout = this.ShowOpenServiceActivityHUD ? 1 : 0;
            bool showNotice = !loginRewardAck.TodayStatus;
            this.parent.SetOpenServiceActivityHUD(this.OpenServiceActivityCout,
                this.ShowOpenServiceActivityHUD, showNotice);
            if (this.ShowOpenServiceActivityHUD) {
                UpdateManager.Regist(UpdateInfo.LoginReward, this.UpdateLoginRewardAt);
            }
        }
        /***********************************/

        private void UpdateLoginRewardAt() {
            this.nextZeroTime = RoleManager.GetNextZeroTime() - 28800 * 1000;
            long thisTime = RoleManager.GetCurrentUtcTime();
            long time = nextZeroTime - thisTime;
            if (time <= 0) {
                this.GetLoginRewardReq();
                if (this.view.IsVisible) {
                    this.view.SetDailyRewardInfo();
                }
            }
        }
    }
}
