using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class AllianceSubordinateViewModel : BaseViewModel {
        private AllianceDetailViewModel parent;
        private AllianceDetailModel model;
        private AllianceSubordinateView view;
        /* Model data get set */

        public List<FallenPlayer> AllianceSubordinates {
            get {
                return this.model.subordinates.subordinates;
            }
        }

        public FallenPlayer CurrentPlayer {
            get {
                return this.currentPlayer;
            }
            set {
                if (this.currentPlayer != value) {
                    this.currentPlayer = value;
                }
                this.OnCurrentPlayerInfoChange();
            }
        }
        private FallenPlayer currentPlayer;

        public bool IsLoadAll {
            get {
                return this.model.subordinates.isLoadAll;
            }
            set {
                this.model.subordinates.isLoadAll = value;
            }
        }

        public int Page {
            get {
                return this.model.subordinates.page;
            }
            set {
                this.model.subordinates.page = value;
            }
        }

        public int PageCount {
            get {
                return this.model.subordinates.pageCount;
            }
        }
        /**********************/

        /* Other members */
        public bool NeedFresh {
            get; set;
        }
        private bool isRequestPage = false;
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<AllianceSubordinateView>();
            this.parent = this.transform.parent.GetComponent<AllianceDetailViewModel>();
            this.model = ModelManager.GetModelData<AllianceDetailModel>();
            this.NeedFresh = true;
        }

        public void Show() {
            this.view.Show();
            if (this.NeedFresh) {
                this.view.InitView();
                this.IsLoadAll = false;
                this.Page = 0;
                this.view.ResetSortBtnInfo();
                this.GetSubordinatesReq();
            }
        }

        public void Hide() {
            this.view.Hide();
            this.NeedFresh = true;
        }

        protected override void OnReLogin() {
            Debug.LogError("OnReLogin callded");
            this.NeedFresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void SetCurrentFallenPlayerInfo(FallenPlayer player) {
            this.CurrentPlayer = player;
        }

        public void ShowSubordinateStatus() {
            this.parent.ShowSubWindowByType(AllianceSubWindowType.SubordinateStatus);
        }


        /* Add 'NetMessageReq' function here*/
        public void GetSubordinatesReq() {
            if (this.IsLoadAll || this.isRequestPage) {
                return;
            }
            this.isRequestPage = true;
            GetAllianceFallenPlayersReq getSubordinateReq = new GetAllianceFallenPlayersReq() {
                Page = ++this.Page
            };
            NetManager.SendMessage(getSubordinateReq,
                                    typeof(GetAllianceFallenPlayersAck).Name,
                                    this.GetSubordinatesAck);
        }

        /* Add 'NetMessageAck' function here*/
        private void GetSubordinatesAck(IExtensible message) {
            GetAllianceFallenPlayersAck getSubordinatesAck = message as GetAllianceFallenPlayersAck;
            this.isRequestPage = false;
            if (this.NeedFresh) {
                this.AllianceSubordinates.Clear();
            }
            int newDataCount = getSubordinatesAck.Players.Count;
            if (newDataCount < this.PageCount) {
                this.IsLoadAll = true;
            }
            int originCount = this.AllianceSubordinates.Count;
            this.AllianceSubordinates.AddRange(getSubordinatesAck.Players);
            if (this.Page == 1 && this.view.IsVisible) {
                this.NeedFresh = false;
                this.view.ResetItems(newDataCount);
                if (!this.IsLoadAll) {
                    this.view.DataRequestAction += this.GetSubordinatesReq;
                }
            } else {
                this.view.InsertItems(originCount, newDataCount);
            }
        }

        /************* property change *************/
        private void OnCurrentPlayerInfoChange() {
            this.parent.ShowPlayerDetailInfo(
                this.currentPlayer.Id);
        }
    }
}
