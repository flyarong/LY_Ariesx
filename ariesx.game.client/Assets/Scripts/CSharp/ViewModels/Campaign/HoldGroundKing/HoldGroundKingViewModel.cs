using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public enum HoldGroundKingType {
        Detail,
        Rank,
        Statistics,
        None
    }
    public class HoldGroundKingViewModel : BaseViewModel, IViewModel {
        private CampaignViewModel parent;
        private HoldGroundKingView view;
        /* Other members */
        private HoldGroundKingDetailsViewModel kingDetailsViewModel;
        private HoldGroundKingRankingViewModel kingRankingViewModel;
        private HoldGroundKingStatisticsViewModel kingStatisticsViewModel;
        public HoldGroundKingType KingType {
            get {
                return this.holdGroundType;
            }
            set {
                this.holdGroundType = value;
                this.OnHoldGroundTypeShow();
            }
        }

        HoldGroundKingType holdGroundType = HoldGroundKingType.None;
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<CampaignViewModel>();
            this.view = this.gameObject.AddComponent<HoldGroundKingView>();
            this.kingDetailsViewModel =
                PoolManager.GetObject<HoldGroundKingDetailsViewModel>(this.transform);
            this.kingRankingViewModel =
                PoolManager.GetObject<HoldGroundKingRankingViewModel>(this.transform);
            this.kingStatisticsViewModel =
                PoolManager.GetObject<HoldGroundKingStatisticsViewModel>(this.transform);
        }

        public void Show(bool EndCampaign = false) {
            if (!this.view.IsVisible) {
                this.view.Show();
            }
            this.view.SetToggleInteractable(!EndCampaign);
            if (!EndCampaign) {
                this.KingType = HoldGroundKingType.Detail;
                this.view.SetTabInfo();
            } else {
                this.CampaignEndShowRank();
            }
        }

        public void customContentSizeFitterSettle() {
            this.kingDetailsViewModel.CustomContentSizeFitterSettle();
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
                HideAllSubPanels();
            }
        }

        public void HideImmediatly() {
            this.Hide();
        }

        public void ShowPlayerInfoClick(string playerId) {
            this.parent.ShowCapaignPlayerInfo(playerId);
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        /* Add 'NetMessageAck' function here*/

        private void CampaignEndShowRank() {
            this.holdGroundType = HoldGroundKingType.Rank;
            this.view.SetTabInfo();
            this.OnHoldGroundTypeShow();
        }

        private void OnHoldGroundTypeShow() {
            switch (this.holdGroundType) {
                case HoldGroundKingType.Detail:
                    this.ShowStatisticsPnl(false);
                    this.ShowRankPnl(false);
                    this.ShowDetailPnl(true);
                    break;
                case HoldGroundKingType.Rank:
                    this.ShowStatisticsPnl(false);
                    this.ShowDetailPnl(false);
                    this.ShowRankPnl(true);
                    break;
                case HoldGroundKingType.Statistics:
                    this.ShowDetailPnl(false);
                    this.ShowRankPnl(false);
                    this.ShowStatisticsPnl(true);
                    break;
                default:
                    break;
            }
        }

        public void ShowRankPnl(bool isVisible) {
            if (isVisible) {
                this.kingRankingViewModel.Show();
            } else {
                this.kingRankingViewModel.Hide();
            }
        }

        public void ShowDetailPnl(bool isVisible) {
            if (isVisible) {
                this.kingDetailsViewModel.Show();
            } else {
                this.kingDetailsViewModel.Hide();
            }
        }

        public void ShowStatisticsPnl(bool isVisible) {
            if (isVisible) {
                this.kingStatisticsViewModel.Show();
            } else {
                this.kingStatisticsViewModel.Hide();
            }
        }

        public void ShowHeroInfo(string hero) {
            this.parent.ShowHeroInfo(hero);
        }

        private void HideAllSubPanels() {
            this.kingDetailsViewModel.Hide();
            this.kingRankingViewModel.Hide();
            this.kingStatisticsViewModel.Hide();
        }

        public void ShowHoldCampaignRewards() {
            this.parent.ShowCampaignRewards();
        }

        public void HideView() {
            this.parent.Hide();
        }

        public void MoveWithClick(Vector2 coordinate) {
            this.parent.MoveWithClick(coordinate);
        }
        /***********************************/
    }
}
