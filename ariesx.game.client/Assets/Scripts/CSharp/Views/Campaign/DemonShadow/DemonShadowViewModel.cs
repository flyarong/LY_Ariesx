using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;
using System.Text.RegularExpressions;

namespace Poukoute {
    public enum DemonShadownType {
        Detail,
        History,
        None
    }
    public class DemonShadowViewModel : BaseViewModel, IViewModel {
        private CampaignViewModel parent;
        private DemonShadowView view;
        /* Model data get set */

        /**********************/

        /* Other members */
        private DemonShadowDetailsViewModel demonShadowDetailsViewModel;
        private DemonShadowHistoryViewModel demonShadowHistoryViewModel;

        public DemonShadownType DemonType {
            get {
                return this.demonType;
            }
            set {
                this.demonType = value;
                this.OnDemonShadowTypeShow();
            }
        }

        DemonShadownType demonType = DemonShadownType.None;

        void Awake() {
            this.parent = this.transform.parent.GetComponent<CampaignViewModel>();
            this.view = this.gameObject.AddComponent<DemonShadowView>();
            this.demonShadowDetailsViewModel =
                PoolManager.GetObject<DemonShadowDetailsViewModel>(this.transform);
            this.demonShadowHistoryViewModel =
                PoolManager.GetObject<DemonShadowHistoryViewModel>(this.transform);
        }

        public void Show(bool EndCampaign = false) {
            if (!this.view.IsVisible) {
                this.view.Show();
            }
            this.view.SetToggleInteractable(!EndCampaign);
            if (!EndCampaign) {
                this.DemonType = DemonShadownType.Detail;//根据设置的活动类型自动选择
                this.view.SetTabInfo();
            } else {
                this.CampaignEndShowRank();
            }
        }

        private void CampaignEndShowRank() {
            this.demonType = DemonShadownType.History;
            this.view.SetTabInfo();
            this.OnDemonShadowTypeShow();
        }

        public void ShowCampaignBossInfo(BossTroop dominaInfo) {
            this.parent.ShowCampaignBossInfo(dominaInfo);
        }

        public void MoveWithClick(Vector2 coordinate) {
            this.parent.Move(coordinate);
        }

        private void OnDemonShadowTypeShow() {
            switch (this.demonType) {
                case DemonShadownType.Detail:
                    this.ShowHistoryPnl(false);
                    this.ShowDetailPnl(true);
                    break;
                case DemonShadownType.History:
                    this.ShowDetailPnl(false);
                    this.ShowHistoryPnl(true);
                    break;
                default:
                    break;
            }
        }

        private void ShowHistoryPnl(bool isVisible) {
            if (isVisible) {
                this.demonShadowHistoryViewModel.Show();
            } else {
                this.demonShadowHistoryViewModel.Hide();
            }
        }

        private void ShowDetailPnl(bool isVisible) {
            if (isVisible) {
                this.demonShadowDetailsViewModel.Show();
            } else {
                this.demonShadowDetailsViewModel.Hide();
            }
        }


        /*****************/

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

        public void ShowDemonShadowHistoryRank(DominationHistory record) {
            this.parent.ShowDemonShadowHistoryRank(record);
        }

        public void ShowCampaignRules(string content) {
            this.parent.ShowCampaignRules(content);
        }

        public void ShowCampaignRewardDomination() {
            this.parent.ShowCampaignRewardDomination();
        }

        public void ShowCampaignCitySelect() {
            this.parent.ShowCampaignCitySelect();
        }
        /* Add 'NetMessageAck' function here*/

        /***********************************/
    }
}
