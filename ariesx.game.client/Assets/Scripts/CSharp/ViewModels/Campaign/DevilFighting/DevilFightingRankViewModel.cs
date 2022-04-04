using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.Events;
using System;

namespace Poukoute {
    public class DevilFightingRankViewModel : BaseViewModel, IViewModel {
        private DevilFightingViewModel parent;
        private DevilFightingRankView view;
        private CampaignModel model;
        /**********************/
        public Activity ChosenActivity {
            get {
                return this.model.chosenActivity;
            }
        }

        /* Other members */

        /*****************/
        void Awake() {
            this.parent = this.transform.parent.GetComponent<DevilFightingViewModel>();
            this.view = this.gameObject.AddComponent<DevilFightingRankView>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.Show();
            }

            this.GetDevilFightingRankInfo();
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
                this.page = 0;
            }
        }

        public void HideImmediatly() {
            this.Hide();
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        public void OnRewardsDetailClick() {
            this.parent.ShowCampaignRewards();
        }

        public void ShowHeroInfo(string hero) {
            this.parent.ShowHeroInfo(hero);
        }

        // Get rank data from server
        private int page = 0;
        private void GetDevilFightingRankInfo() {
            GetMeleeRankReq monsterRankReq = new GetMeleeRankReq() {
                Page = ++this.page
            };
            NetManager.SendMessage(monsterRankReq,
                typeof(GetMeleeRankAck).Name, this.OnGetDevilFightingRankInfo);
        }

        public RankMelee SelfRank;
        public List<RankMelee> RankList = new List<RankMelee>(10);
        private void OnGetDevilFightingRankInfo(IExtensible message) {
            GetMeleeRankAck monsterRankAck = message as GetMeleeRankAck;
            //Debug.LogError(monsterRankAck.Monster.Count + " " + 
            //    ((monsterRankAck.Self != null) ? monsterRankAck.Self.Rank.ToString() : "Self rank is null"));
            this.SelfRank = monsterRankAck.Self;
            this.RankList = monsterRankAck.Monster;
            this.view.SetContent();
        }

        public void ShowPlayerInfoClick(string playerId) {
            this.parent.ShowPlayerInfoClick(playerId);
        }
    }
}
