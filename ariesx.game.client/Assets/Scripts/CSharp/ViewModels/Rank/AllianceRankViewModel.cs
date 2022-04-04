using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class AllianceRankViewModel : BaseViewModel {
        private RankModel model;
        private AllianceRankView view;
        private RankViewModel parent;

        public AllianceRank AllianceRankInfo {
            get {
                return this.model.AllianceRankInfo;
            }
        }

        public RankAlliance OwnRankInfo {
            get {
                return this.AllianceRankInfo.ownRankInfo;
            }
            set {
                if (this.AllianceRankInfo.ownRankInfo != value) {
                    this.AllianceRankInfo.ownRankInfo = value;
                }
            }
        }

        public List<RankAlliance> RankInfoList {
            get {
                return this.AllianceRankInfo.rankDataList;
            }
        }

        public bool IsLoadAll {
            get {
                return this.AllianceRankInfo.isLoadAll;
            }
            set {
                this.AllianceRankInfo.isLoadAll = value;
            }
        }      

        public int Page {
            get {
                return this.AllianceRankInfo.page;
            }
            set {
                this.AllianceRankInfo.page = value;
            }
        }

        public int PageCount {
            get {
                return this.AllianceRankInfo.pageCount;
            }
        }

        /***** Other *****/
        private bool firstPageContainOwnInfo = false;
        private bool NeedRefresh { get; set; }
        private bool isRequestPage = false;
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<AllianceRankView>();
            this.model = ModelManager.GetModelData<RankModel>();
            this.parent = this.transform.parent.GetComponent<RankViewModel>();
            this.NeedRefresh = true;
        }

        public void Show() {
            this.view.Show();
            if (this.NeedRefresh) {
                this.IsLoadAll = false;
                this.Page = 0;
                this.RankInfoListReq();
            } else {
                this.SetOwnRankInfo();
            }
        }

        public void Hide(bool needRefresh = false) {
            this.view.Hide(this.OnInvisible);
        }

        protected override void OnReLogin() {
            this.NeedRefresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void RankInfoListReq() {
            if (this.IsLoadAll || this.isRequestPage) {
                return;
            }
            this.isRequestPage = true;
            AllianceRankReq allianceRankReq = new AllianceRankReq() {
                Page = ++this.Page
            };
            NetManager.SendMessage(allianceRankReq,
                typeof(AllianceRankAck).Name, this.AllianceRankInfoAck);
        }

        public void ShowAllianceInfo(string allianceId) {
            this.parent.ShowAllianceInfo(allianceId);
        }

        public void SetOwnRankInfo() {
            this.parent.SetOwnRankInfo();
        }

        public void JumpToOwnRank() {
            this.view.ScrollTo(this.OwnRankInfo.Rank);
        }

        public bool IsShowOwnRankInfo() {
            if (this.OwnRankInfo == null ||
                this.firstPageContainOwnInfo) {
                return false;
            }

            return  this.IsOwnRankInfoVisible();
        }

        public bool IsOwnRankInfoVisible() {
            return this.OwnRankInfo != null && this.OwnRankInfo.Rank != 0;
        }

        private void AllianceRankInfoAck(IExtensible message) {
            AllianceRankAck rankAck = message as AllianceRankAck;
            this.isRequestPage = false;
            if (rankAck.Alliances.Count < this.PageCount) {
                this.IsLoadAll = true;
            }

            if (this.NeedRefresh) {
                this.RankInfoList.Clear();
            }

            int originCount = this.RankInfoList.Count;
            int newDataCount = rankAck.Alliances.Count;
            this.OwnRankInfo = rankAck.Self;
            //Debug.LogError(rankAck.Self);
            //Debug.LogError(this.OwnRankInfo);
            foreach (RankAlliance rankInfo in rankAck.Alliances) {
                this.RankInfoList.Add(rankInfo);
            }

            if (this.Page == 1 && this.view.IsVisible) {
                this.SetFirstPageContainOwnInfo();
                this.NeedRefresh = false;
                this.view.ResetItems(newDataCount);
                if (!this.IsLoadAll) {
                    this.view.DataRequestAction += this.RankInfoListReq;
                }
            } else {
                this.view.InsertItems(originCount, newDataCount);
            }
        }

        private void SetFirstPageContainOwnInfo() {
            this.firstPageContainOwnInfo = false;
            string roleId = RoleManager.GetAllianceId();
            foreach (RankAlliance rankInfo in this.RankInfoList) {
                if (rankInfo.Id.CustomEquals(roleId)) {
                    this.firstPageContainOwnInfo = true;
                    break;
                }
            }
        }

        private void OnInvisible() {
            this.isRequestPage = false;
            this.NeedRefresh = true;
            //this.RankInfoList.Clear();
            this.IsLoadAll = false;
            this.Page = 0;
        }
    }
}
