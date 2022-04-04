using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class OccupationRankViewModel : BaseViewModel {
        private RankViewModel parent;
        private OccupationRankView view;
        private RankModel model;

        public OccupationRank OccupationRankInfo {
            get {
                return this.model.OccupationRankInfo;
            }
        }

        public RankAlliance OwnRankInfo {
            get {
                return this.OccupationRankInfo.ownRankInfo;
            }
            set {
                if (this.OccupationRankInfo.ownRankInfo != value) {
                    this.OccupationRankInfo.ownRankInfo = value;
                }
            }
        }

        public List<RankAlliance> RankInfoList {
            get {
                return this.OccupationRankInfo.rankDataList;
            }
        }

        public bool IsLoadAll {
            get {
                return this.OccupationRankInfo.isLoadAll;
            }
            set {
                this.OccupationRankInfo.isLoadAll = value;
            }
        }

        public int Page {
            get {
                return this.OccupationRankInfo.page;
            }
            set {
                this.OccupationRankInfo.page = value;
            }
        }

        public int PageCount {
            get {
                return this.OccupationRankInfo.pageCount;
            }
        }

        /***** Other *****/
        private bool firstPageContainOwnInfo = false;
        private bool NeedRefresh { get; set; }
        private bool isRequestPage = false;
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<OccupationRankView>();
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
            AllianceOccupationRankReq occupationRankReq = new AllianceOccupationRankReq() {
                Page = ++this.Page
            };
            NetManager.SendMessage(occupationRankReq,
                typeof(AllianceOccupationRankAck).Name, this.OnAllianceOccupationRankAck);
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

            return this.OwnRankInfo != null && this.OwnRankInfo.OccupationRank != 0;
        }

        public void ShowOccupationIntro() {
            this.parent.ShowOccupationIntro();
        }

        private void OnAllianceOccupationRankAck(IExtensible message) {
            AllianceOccupationRankAck rankAck = message as AllianceOccupationRankAck;
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
            foreach (RankAlliance rankInfo in rankAck.Alliances) {
                this.RankInfoList.Add(rankInfo);
            }

            if (this.Page == 1) {
                this.SetFirstPageContainOwnInfo();
                this.SetOwnRankInfo();
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
            this.IsLoadAll = false;
            this.Page = 0;
        }
    }
}
