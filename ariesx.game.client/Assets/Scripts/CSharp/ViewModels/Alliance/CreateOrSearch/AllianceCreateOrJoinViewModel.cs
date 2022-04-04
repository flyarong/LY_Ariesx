using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class AllianceCreateOrJoinViewModel : BaseViewModel, IViewModel {
        private AllianceViewModel parent;
        private AllianceDetailModel allianceDetailModel;
        private AllianceCreateOrJoinModel model;
        private AllianceCreateOrJoinView view;
        //private BuildModel buildModel;

        /* Model data get set */
        public bool NeedRefresh {
            get; set;
        }

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
        /**********************/

        /* Other members */
        private AllianceCreateViewModel createViewModel;
        private AllianceCreateViewModel CreateViewModel {
            get
            {
                return this.createViewModel ??
                       (this.createViewModel = PoolManager.GetObject<AllianceCreateViewModel>(this.transform));
            }
        }
        private AllianceSearchViewModel searchViewModel;
        private AllianceSearchViewModel SearchViewModel {
            get
            {
                return this.searchViewModel ??
                       (this.searchViewModel = PoolManager.GetObject<AllianceSearchViewModel>(this.transform));
            }
        }

        /*****************/

        void Awake() {
            this.allianceDetailModel = ModelManager.GetModelData<AllianceDetailModel>();
            this.model = ModelManager.GetModelData<AllianceCreateOrJoinModel>();
            this.parent = this.transform.parent.GetComponent<AllianceViewModel>();
            this.view = this.gameObject.AddComponent<AllianceCreateOrJoinView>();
            this.NeedRefresh = true;
            NetHandler.AddNtfHandler(typeof(RelationNtf).Name, this.OnRelationChangeNtf);
        }

        public void OnAllianceLogoChange(int logoId) {
            this.CreateViewModel.AllianceEmblem = logoId;
        }

        public void Show() {
            bool notInAlliance = RoleManager.GetAllianceId().CustomIsEmpty();
            if (notInAlliance) {
                this.view.SetCreateOrJoinAllianceView(this.parent.CanCreateAlliance());
                this.view.PlayShow(() => this.parent.OnAddViewAboveMap(this));
            }
        }

        public void Hide(UnityAction action = null) {
            this.view.PlayHide(() => {
                action.InvokeSafe();
                this.parent.OnRemoveViewAboveMap(this);
                this.Channel = AllianceViewType.None;
            });
            this.HideAllSubView();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => {
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        public void ShowSearch(bool visible) {
            if (visible) {
                this.SearchViewModel.Show();
            } else {
                this.SearchViewModel.Hide();
            }
        }

        public void ShowCreate(bool visible) {
            if (visible) {
                this.CreateViewModel.Show();
            } else {
                this.CreateViewModel.Hide();
            }
        }

        public void ApplyToJoinAlliance(string allianceId, string message) {
            this.SearchViewModel.JoinAllianceReq(allianceId, message);
        }

        public void ShowSubWindowByType(AllianceSubWindowType type) {
            this.parent.ShowSubWindowByType(type);
        }

        public void HideAllSubView() {
            this.parent.HideSubWindows();
            this.ShowSearch(false);
            this.ShowCreate(false);
        }

        public void ShowAllianceInfo(string allianceId) {
            this.parent.ShowAllianceInfo(allianceId);
        }

        private void OnChannelChange() {
            if (this.Channel == AllianceViewType.None)
                return;
            this.view.OnAllianceViewTypeChange();
            switch (this.Channel) {
                case AllianceViewType.Search:
                    this.ShowSearch(true);
                    break;
                case AllianceViewType.Create:
                    this.ShowCreate(true);
                    break;
                default:
                    break;
            }
        }

        private void OnRelationChangeNtf(IExtensible message) {
            RelationNtf relationNtf = message as RelationNtf;
            if (this.view.IsVisible &&
                !relationNtf.AllianceId.CustomIsEmpty()) {
                this.allianceDetailModel.allianceViewType = AllianceViewType.None;
                this.view.afterHideCallback = this.parent.Show;
                this.Hide();
            }
        }
        /***********************************/
    }
}
