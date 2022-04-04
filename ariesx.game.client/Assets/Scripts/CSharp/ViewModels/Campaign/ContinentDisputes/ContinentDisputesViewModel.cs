using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public enum ContinentDisputesViewType {
        Detail,
        Rank,
        Statistics,
        None
    }
    public class ContinentDisputesViewModel : BaseViewModel, IViewModel {
        //private CampaignViewModel campaignViewModel;
        private ContinentDisputesView view;
        private CampaignViewModel parent;
        /* Model data get set */
        public ContinentDisputesViewType ViewType {
            get {
                return this.viewType;
            }
            set {
                this.viewType = value;
                this.OnChannelTypeChange();
            }
        }
        /**********************/

        /* Other members */
        private ContinentDisputesDetailsViewModel detailsViewModel;
        private ContinentDisputesRankViewModel rankingViewModel;
        private ContinentDisputesStatisticsViewModel statisticsViewModel;

        private ContinentDisputesViewType viewType = ContinentDisputesViewType.None;
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<CampaignViewModel>();
            //this.campaignViewModel = this.transform.parent.GetComponent<CampaignViewModel>();
            this.view = this.gameObject.AddComponent<ContinentDisputesView>();
            this.detailsViewModel =
                PoolManager.GetObject<ContinentDisputesDetailsViewModel>(this.transform);
            this.rankingViewModel =
                PoolManager.GetObject<ContinentDisputesRankViewModel>(this.transform);
            this.statisticsViewModel =
                PoolManager.GetObject<ContinentDisputesStatisticsViewModel>(this.transform);
        }

        internal void ShowHoldCampaignRewards() {
            this.parent.ShowCampaignRewards();
        }

        public void Show(bool EndCampaign = false) {
            if (!this.view.IsVisible) {
                this.view.Show();
            }
            this.view.SetToggleInteractable(!EndCampaign);
            if (!EndCampaign) {
                this.ViewType = ContinentDisputesViewType.Detail;
                this.view.SetTabsInfo();
            } else {
                this.CampaignEndShowRank();
            }

        }

        private void CampaignEndShowRank() {
            this.ViewType = ContinentDisputesViewType.Rank;
            this.view.SetTabsInfo();
            this.OnChannelTypeChange();

        }

        public void ShowCampaignAllianceInfoClick(string allianceId) {
            this.parent.ShowCampaignAllianceInfoClick(allianceId);
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
            }
            this.HideAllSubPanels();
        }

        public void HideImmediatly() {
            this.Hide();
        }

        protected override void OnReLogin() {
            this.Hide();
        }
        public void HideView() {
            this.parent.Hide();
        }
        /* Add 'NetMessageAck' function here*/

        public void ShowDetailsPnl(bool isVisible) {
            if (isVisible) {
                this.detailsViewModel.Show();
            } else {
                this.detailsViewModel.Hide();
            }
        }

        public void ShowRankingPnl(bool isVisible) {
            if (isVisible) {
                this.rankingViewModel.Show();
            } else {
                this.rankingViewModel.Hide();
            }
        }

        public void ShowStatistics(bool isVisible) {
            if (isVisible) {
                this.statisticsViewModel.Show();
            } else {
                this.statisticsViewModel.Hide();
            }
        }

        private void HideAllSubPanels() {
            this.rankingViewModel.Hide();
            this.detailsViewModel.Hide();
            this.statisticsViewModel.Hide();
        }

        private void OnChannelTypeChange() {
            switch (this.viewType) {
                case ContinentDisputesViewType.Detail:
                    this.ShowRankingPnl(false);
                    this.ShowStatistics(false);
                    this.ShowDetailsPnl(true);
                    break;
                case ContinentDisputesViewType.Rank:
                    this.ShowDetailsPnl(false);
                    this.ShowStatistics(false);
                    this.ShowRankingPnl(true);
                    break;
                case ContinentDisputesViewType.Statistics:
                    this.ShowDetailsPnl(false);
                    this.ShowRankingPnl(false);
                    this.ShowStatistics(true);
                    break;
                default:
                    break;
            }
        }

        public void ShowHeroInfo(string hero) {
            this.parent.ShowHeroInfo(hero);
        }

        public void MoveWithClick(Vector2 coordinate) {
            this.parent.MoveWithClick(coordinate);
        }
        /***********************************/
    }
}
