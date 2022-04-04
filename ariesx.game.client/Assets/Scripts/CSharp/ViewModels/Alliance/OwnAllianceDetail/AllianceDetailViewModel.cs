using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class AllianceDetailViewModel : BaseViewModel, IViewModel {
        private AllianceViewModel parent;
        private AllianceDetailModel model;
        private AllianceDetailView view;
        //private BuildModel buildModel;

        /* Model data get set */
        public System.Type PreviouseView {
            get; set;
        }

        public AllianceViewType Channel {
            get {
                return this.model.allianceViewType;
            }
            set {
                if (this.model.allianceViewType != value) {
                    this.model.allianceViewType = value;
                    this.OnChannelChange();
                }
            }
        }
        //private bool AlreadyInAlliance = false;
        /**********************/
        /* Other members */
        public bool IsInitiativeQuitAlliance {
            get {
                return this.parent.isInitiativeQuitAlliance;
            }
            set {
                this.parent.isInitiativeQuitAlliance = value;
            }
        }
        private AlliancePanelViewModel alliancePanleViewModel;
        private AlliancePanelViewModel AlliancePanleViewModel {
            get {
                return this.alliancePanleViewModel ?? (this.alliancePanleViewModel =
                           PoolManager.GetObject<AlliancePanelViewModel>(this.transform));
            }
        }
        private AllianceCitiesViewModel allianceCitiesViewModel;
        private AllianceCitiesViewModel AllianceCitiesViewModel {
            get {
                return this.allianceCitiesViewModel ?? (this.allianceCitiesViewModel =
                           PoolManager.GetObject<AllianceCitiesViewModel>(this.transform));
            }
        }
        private AllianceSubordinateViewModel allianceSubordinateViewModel;
        private AllianceSubordinateViewModel AllianceSubordinateViewModel {
            get {
                return this.allianceSubordinateViewModel ?? (this.allianceSubordinateViewModel =
                           PoolManager.GetObject<AllianceSubordinateViewModel>(this.transform));
            }
        }
        /*****************/

        void Awake() {
            this.model = ModelManager.GetModelData<AllianceDetailModel>();
            this.view = this.gameObject.AddComponent<AllianceDetailView>();
            this.parent = this.transform.parent.GetComponent<AllianceViewModel>();
        }

        public void Show() {
            this.view.Show();
            if (this.Channel == AllianceViewType.None) {
                this.Channel = AllianceViewType.Alliance;
            }
            this.view.SetAllianceDetailView();
        }

        public void HideAllianceView() {
            this.view.SetReturn();
            this.parent.Hide();
        }

        public void Hide() {
            this.view.Hide(() => {
                this.Channel = AllianceViewType.None;
                this.HideAllSubPanel();
            });
        }

        public void HideAllSubPanel() {
            this.AlliancePanleViewModel.Hide();
            this.AllianceSubordinateViewModel.Hide();
            this.AllianceCitiesViewModel.Hide();
        }

        public void HideImmediatly() {
            this.Hide();
        }

        public void GetMyAlliance() {
            this.alliancePanleViewModel.GetMyAlliance();
        }

        public void ResetUserAllianceInfo() {
            this.alliancePanleViewModel.ResetUserAllianceInfo();
        }

        public void ShowAllianceDisplayBoard(DisplayType type) {
            this.parent.ShowAllianceDisplayBoard(type);
        }

        public void Move(Vector2 coordinate) {
            this.parent.Move(coordinate);
        }

        public void RefreshMarkInTile(Vector2 coord, MapMarkType type, bool isAdd) {
            this.parent.RefreshMarkInTile(coord, type, isAdd);
        }

        public void ShowAllianceMembersList(string allianceId) {
            this.parent.ShowAllianceMembersList(allianceId);
        }

        public void ShowAllianceChatroom() {
            this.parent.ShowAllianceChat();
        }

        public void HideAllianceDetail() {
            this.model.allianceViewType = AllianceViewType.None;
            this.parent.HideCurrentPanel();
        }

        public void RefreshAfterQuitAlliance() {
            this.parent.RefreshAfterQuitAlliance();
        }

        public void ShowSubWindowByType(AllianceSubWindowType type) {
            this.parent.ShowSubWindowByType(type);
        }

        public void ApplyJoinAlliance() {
            this.parent.ApplyJoinAlliance(string.Empty);
        }

        public void HideAllSubViews() {
            this.parent.HideSubWindows();
        }

        public void ShowPlayerDetailInfo(string playerId) {
            this.parent.ShowPlayerDetailInfo(playerId);
        }

        public void ShowAllianceMemOperate(PlayerPublicInfo playerInfo,
                ButtonClickWithLabel greenBtnInfo, ButtonClickWithLabel redBtnInfo) {
            this.parent.ShowAllianceMemOperate(playerInfo, greenBtnInfo, redBtnInfo);
        }

        public void RefreshAllianeDetailByType(AllianceViewType type) {
            this.Channel = type;
            this.AlliancePanleViewModel.NeedFresh = true;
        }

        private void HideAllSubWidows() {
            if (this.allianceCitiesViewModel != null) {
                this.allianceCitiesViewModel.Hide();
            }
            if (this.allianceSubordinateViewModel != null) {
                this.allianceSubordinateViewModel.Hide();
            }
            if (this.alliancePanleViewModel != null) {
                this.alliancePanleViewModel.Hide();
            }
        }

        private void OnChannelChange() {
            this.HideAllSubWidows();

            switch (this.Channel) {
                case AllianceViewType.Alliance:
                    this.AlliancePanleViewModel.Show();
                    break;
                case AllianceViewType.City:
                    this.AllianceCitiesViewModel.Show();
                    break;
                case AllianceViewType.Subordinate:
                    this.AllianceSubordinateViewModel.Show();
                    break;
                default:
                    break;
            }
        }

        public void JumbCityItemCood(Coord coord) {
            this.parent.JumbCityItemCood(coord);
        }

        /***********************************/
    }
}
