using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class HoldGroundKingRankingViewModel: BaseViewModel, IViewModel {
        private HoldGroundKingViewModel parent;
        private HoldGroundKingRankingView view;
        private CampaignModel model;
        /* Model data get set */

        /**********************/

        /* Other members */
        public Activity RewardBasicContent {
            get {
                return this.model.chosenActivity;
            }
        }
        /*****************/

        //private int page = 0;
        public RankOccupy SelfRank;
        public List<RankOccupy> rankOccupyList = new List<RankOccupy>(10);

        void Awake() {
            this.parent = this.transform.parent.GetComponent<HoldGroundKingViewModel>();
            this.view = this.gameObject.AddComponent<HoldGroundKingRankingView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.Show();
            }
            this.GetOccupyRankReq();
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
        public void OnRewardsDetailClick() {
            this.parent.ShowHoldCampaignRewards();
        }

        public void ShowHeroInfo(string hero) {
            this.parent.ShowHeroInfo(hero);
        }

        private void GetOccupyRankReq() {
            GetOccupyRankReq occupyRankReq = new GetOccupyRankReq();
            NetManager.SendMessage(occupyRankReq,
                typeof(GetOccupyRankAck).Name, this.GetOccupyRankAck);
        }

        private void GetOccupyRankAck(IExtensible message) {
            GetOccupyRankAck monsterRankAck = message as GetOccupyRankAck;
            this.SelfRank = monsterRankAck.Self;
            this.rankOccupyList = monsterRankAck.Players;
            this.view.SetRewardBasicContent();
        }

        public void ShowPlayerInfoClick(string playerId) {
            this.parent.ShowPlayerInfoClick(playerId);
        }
    }
}
