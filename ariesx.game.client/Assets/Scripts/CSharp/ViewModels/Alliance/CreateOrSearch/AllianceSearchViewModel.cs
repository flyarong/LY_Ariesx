using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class AllianceSearchViewModel : BaseViewModel {
        private AllianceCreateOrJoinViewModel parent;
        private AllianceCreateOrJoinModel model;
        private AllianceSearchView view;
        /* Model data get set */
        public string SearchAllianceName {
            get {
                return this.model.searchAllianceChannel.allianceName;
            }
            set {
                this.model.searchAllianceChannel.allianceName = value;
            }
        }

        public List<AllianceCache> AlliancesList {
            get {
                return this.model.searchAllianceChannel.alliancesList;
            }
        }

        public bool IsLoadAll {
            get {
                return this.model.searchAllianceChannel.isLoadAll;
            }
            set {
                this.model.searchAllianceChannel.isLoadAll = value;
            }
        }

        public int CountShow {
            get {
                return this.model.searchAllianceChannel.countShow;
            }
        }

        public int PageCount {
            get {
                return this.model.searchAllianceChannel.pageCount;
            }
        }

        public int Page {
            get {
                return this.model.searchAllianceChannel.page;
            }

            set {
                this.model.searchAllianceChannel.page = value;
            }
        }

        public bool NeedFresh {
            get; set;
        }

        /*****************************************************/

        void Awake() {
            this.view = this.gameObject.AddComponent<AllianceSearchView>();
            this.model = ModelManager.GetModelData<AllianceCreateOrJoinModel>();
            this.parent = this.transform.parent.GetComponent<AllianceCreateOrJoinViewModel>();
            this.NeedFresh = true;
        }

        public void Show() {
            this.view.Show();
            if (this.NeedFresh) {
                this.IsLoadAll = false;
                this.Page = 0;
                this.GetAllAlliancesReq();
            } else {
                this.view.Refresh();
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide(/*this.ResetAllianceSearch*/);
                this.NeedFresh = true;
            }
        }

        protected override void OnReLogin() {
            this.NeedFresh = true;
            if (this.view.IsVisible) {
                this.ResetAllianceSearch();
                this.Show();
            }
        }

        /* Add 'NetMessageReq' function here*/
        public void SearchAllianceReq() {
            this.Page = 0;
            this.AlliancesList.Clear();
            SearchAllianceReq searchAllianceReq = new SearchAllianceReq() {
                Name = this.SearchAllianceName
            };
            NetManager.SendMessage(searchAllianceReq,
                typeof(SearchAllianceAck).Name,
                this.SearchAllianceAck,
                this.ErrorCallback,
                this.ResetAllianceSearch
            );
        }

        public void GetAllAlliancesReq() {
            if (!this.view.IsVisible || this.IsLoadAll) {
                return;
            }

            GetAlliancesReq alliancesReq = new GetAlliancesReq() {
                Page = ++this.Page
            };
            NetManager.SendMessage(alliancesReq,
                typeof(GetAlliancesAck).Name,
                this.GetAllAlliancesAck,
                this.ErrorCallback,
                this.ResetAllianceSearch);
        }

        private void ErrorCallback(IExtensible message) {
            this.ResetAllianceSearch();
        }

        private void ResetAllianceSearch() {
            this.AlliancesList.Clear();
        }

        public void JoinAllianceReq(string id, string message) {

            JoinAllianceReq joinAllianceReq = new JoinAllianceReq() {
                Id = id,
                Message = message
            };
            NetManager.SendMessage(joinAllianceReq, string.Empty, null);
        }

        public void ShowAllianceInfo(string allianceId) {
            this.parent.ShowAllianceInfo(allianceId);
        }
        /***********************************/


        /* Add 'NetMessageAck' function here*/
        private void SearchAllianceAck(IExtensible message) {
            SearchAllianceAck searchAllianceAck = message as SearchAllianceAck;
            if (searchAllianceAck.AllianceCache != null) {
                this.ResetAllianceSearch();
                this.IsLoadAll = true;
                this.AlliancesList.Add(searchAllianceAck.AllianceCache);
                this.view.ResetItems(1);
            }
        }

        private void GetAllAlliancesAck(IExtensible message) {
            GetAlliancesAck ackMsg = message as GetAlliancesAck;
            //Debug.LogError("GetAllAlliancesAck.Count== " + alliancesAck.AllianceCaches.Count);
            if (ackMsg.AllianceCaches.Count < this.PageCount) {
                this.IsLoadAll = true;
            }

            if (this.NeedFresh) {
                this.ResetAllianceSearch();
            }

            int originCount = this.AlliancesList.Count;
            int newDataCount = ackMsg.AllianceCaches.Count;
            foreach (AllianceCache alliance in ackMsg.AllianceCaches) {
                this.AlliancesList.Add(alliance);
            }

            if (this.Page == 1 && this.view.IsVisible) {
                this.NeedFresh = false;
                this.view.ResetItems(newDataCount);
                if (!this.IsLoadAll) {
                    this.view.DataRequestAction += this.GetAllAlliancesReq;
                }
            } else {
                this.view.InsertItems(originCount, newDataCount);
            }
        }
        /***********************************/
    }
}
