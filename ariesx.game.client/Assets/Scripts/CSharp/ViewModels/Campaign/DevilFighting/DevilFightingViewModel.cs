using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.Events;
using System;

namespace Poukoute {
    public enum DevilFightingViewType {
        Detail,
        Rank,
        None
    }

    public class DevilFightingViewModel: BaseViewModel, IViewModel {
        private CampaignViewModel parent;
        private DevilFightingView view;
        /**********************/

        /* Other members */
        private DevilFightingDetailViewModel DetailViewModel {
            get {
                return this.detailViewModel ?? (this.detailViewModel =
                           PoolManager.GetObject<DevilFightingDetailViewModel>(this.transform));
            }
        }
        private DevilFightingDetailViewModel detailViewModel;

        private DevilFightingRankViewModel RankViewModel {
            get {
                return this.rankViewModel ?? (this.rankViewModel =
                           PoolManager.GetObject<DevilFightingRankViewModel>(this.transform));
            }
        }
        private DevilFightingRankViewModel rankViewModel;

        public DevilFightingViewType ChannelType {
            get {
                return this.channelType;
            }
            set {
                this.channelType = value;
                this.OnChannelTypeChange();
            }
        }

        private DevilFightingViewType channelType = DevilFightingViewType.None;


        /*****************/
        void Awake() {
            this.parent = this.transform.parent.GetComponent<CampaignViewModel>();
            this.view = this.gameObject.AddComponent<DevilFightingView>();
        }

        public void Show(bool EndCampaign = false) {
            if (!this.view.IsVisible) {
                this.view.Show();
            }
            this.view.SetToggleInteractable(!EndCampaign);
            if (!EndCampaign) {
                this.ChannelType = DevilFightingViewType.Detail;
                this.view.SetContent();
            } else {
                this.CampaignEndShowRank();
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
                this.HideAllSubPanels();
            }
        }

        public void HideImmediatly() {
            this.Hide();
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        private void HideAllSubPanels() {
            this.DetailViewModel.Hide();
            this.RankViewModel.Hide();
        }

        public void ShowPlayerInfoClick(string playerId) {
            this.parent.ShowCapaignPlayerInfo(playerId);
        }

        // model-view logic
        public void GetRecentMonsterByLevel(int monsterLevel) {
            this.parent.GetRecentMonsterByLevel(monsterLevel);
        }

        public void ShowDetailPnl(bool isVisible) {
            if (isVisible) {
                this.DetailViewModel.Show();
            } else {
                this.DetailViewModel.Hide();
            }
        }

        public void ShowRankPnl(bool isVisible) {
            if (isVisible) {
                this.RankViewModel.Show();
            } else {
                this.RankViewModel.Hide();
            }
        }

        public void ShowCampaignRewards() {
            this.parent.ShowCampaignRewards();
        }

        public void ShowHeroInfo(string hero) {
            this.parent.ShowHeroInfo(hero);
        }

        // private callbacks
        private void OnChannelTypeChange() {
            switch (this.channelType) {
                case DevilFightingViewType.Detail:
                    this.ShowRankPnl(false);
                    this.ShowDetailPnl(true);
                    break;
                case DevilFightingViewType.Rank:
                    this.ShowDetailPnl(false);
                    this.ShowRankPnl(true);
                    break;
                default:
                    break;
            }
        }

        private void CampaignEndShowRank() {
            this.channelType = DevilFightingViewType.Rank;
            this.view.SetContent();
            this.OnChannelTypeChange();
        }
    }
}
