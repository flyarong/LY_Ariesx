using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class ContinentDisputesRankViewModel: BaseViewModel , IViewModel {
        private ContinentDisputesRankView view;
        private ContinentDisputesViewModel parent;
        private CampaignModel model;
        /* Model data get set */
        public Activity RewardBasicContent {
            get {
                return this.model.chosenActivity;
            }
        }

        /**********************/

        /* Other members */
        public RankCapture selfRank;
        public List<RankCapture> rankCaptureList = new List<RankCapture>(10);
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<ContinentDisputesViewModel>();
            this.view = this.gameObject.AddComponent<ContinentDisputesRankView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.Show();
            }
            this.GetCaptureRankReq();
        }

        internal void OnRewardsDetailClick() {
            this.parent.ShowHoldCampaignRewards();
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
            }
        }

        public void HideImmediatly() {
            this.Hide();
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        public void ShowHeroInfo(string hero) {
            this.parent.ShowHeroInfo(hero);
        }

        /* Add 'NetMessageAck' function here*/
        private void GetCaptureRankReq() {
            GetCaptureRankReq captureRankReq = new GetCaptureRankReq();
            NetManager.SendMessage(captureRankReq, typeof(
                GetCaptureRankAck).Name, this.GetCaptureRankAck1);
        }

        private void GetCaptureRankAck1(IExtensible message) {
            GetCaptureRankAck captureRankAck = message as GetCaptureRankAck;
            this.selfRank = captureRankAck.Self;
            this.rankCaptureList = captureRankAck.Alliances;
            this.view.SetRewardBasicContent();
        }

        public void ShowCampaignAllianceInfoClick(string allianceId) {
            this.parent.ShowCampaignAllianceInfoClick(allianceId);
        }

        /***********************************/
    }
}
