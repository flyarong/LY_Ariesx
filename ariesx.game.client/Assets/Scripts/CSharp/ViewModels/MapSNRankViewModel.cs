using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class MapSNRankViewModel: BaseViewModel {
        private RankModel model;
        private MapSNRankView view;
        private RankViewModel parent;
        
        
        /* Model data get set */

        public MapSNRank MapSNRankInfo {
            get {
                return this.model.MapSNRankInfo;
            }
        }


        public RankPlayer SelfRankInfo {
            get {
                return this.MapSNRankInfo.ownRankInfo;
            }
            set {
                if (this.MapSNRankInfo.ownRankInfo != value) {
                    this.MapSNRankInfo.ownRankInfo = value;
                }
            }
        }

        public List<RankPlayer> MapSNRankInfoList {
            get {
                return this.MapSNRankInfo.rankDataList;
            }
        }

        public bool IsLoadAll {
            get {
                return this.MapSNRankInfo.isLoadAll;
            }
            set {
                this.MapSNRankInfo.isLoadAll = value;
            }
        }

        public int Page {
            get {
                return this.MapSNRankInfo.page;
            }
            set {
                this.MapSNRankInfo.page = value;
            }
        }

        public int PageCount {
            get {
                return this.MapSNRankInfo.pageCount;
            }
        }

        /**********************/

        /* Other members */
        private bool firstPageContainSelfInfo = false;
        private bool NeedRefresh { get; set; }
        private bool isRequestPage = false;
        /*****************/

        void Awake() {
            this.model = ModelManager.GetModelData<RankModel>();
            this.parent = this.transform.parent.GetComponent<RankViewModel>();
            this.view = this.gameObject.AddComponent<MapSNRankView>();
            this.NeedRefresh = true;
        }

        public void Show() {
            this.view.Show();
            if (this.NeedRefresh) {
                this.IsLoadAll = false;
                this.Page = 0;
                this.MapSNRankReq();
            } else {
                this.SetSelfRankInfo();
            }
        }

        public void Hide() {
            this.view.Hide(this.OnInvisible);
        }

        public void JumpToSelfRank() {
            this.view.ScrollTo(this.SelfRankInfo.MapSNRank);
        }

        protected override void OnReLogin() {
            this.NeedRefresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void ShowPlayerInfo(string playerId) {
            this.parent.ShowPlayerInfo(playerId);
        }

        public bool IsShowSelfRankInfo() {
            if (this.SelfRankInfo == null ||
                this.firstPageContainSelfInfo) {
                return false;
            }
            return this.IsSelfRankInfoVisible();
        }

        public bool IsSelfRankInfoVisible() {
            return this.SelfRankInfo != null && this.SelfRankInfo.Rank != 0;
        }

        private void SetFirstPageContainSelfInfo() {
            this.firstPageContainSelfInfo = false;
            string roleId = RoleManager.GetRoleId();
            foreach (RankPlayer rankInfo in this.MapSNRankInfoList) {
                if (rankInfo.Id.CustomEquals(roleId)) {
                    this.firstPageContainSelfInfo = true;
                    break;
                }
            }
        }

        public void SetSelfRankInfo() {
            this.parent.SetOwnRankInfo();
        }

        /* Add 'NetMessageAck' function here*/
        private void MapSNRankReq() {
            if (this.IsLoadAll || this.isRequestPage) {
                return;
            }            
            this.isRequestPage = true;
            MapSNRankReq rankReq = new MapSNRankReq() {
                Page = ++this.Page
            };
            NetManager.SendMessage(
                rankReq, typeof(MapSNRankAck).Name, this.MapSNRankAck);
        }

        private void MapSNRankAck(IExtensible message) {
            MapSNRankAck mapSNRank = message as MapSNRankAck;
            this.isRequestPage = false;
            int newDataCount = mapSNRank.Players.Count;
            if (newDataCount < this.PageCount) {
                this.IsLoadAll = true;
            }
            if (this.NeedRefresh) {
                this.MapSNRankInfoList.Clear();
            }
            int originCount = this.MapSNRankInfoList.Count;
            this.SelfRankInfo = mapSNRank.Self;
            foreach (RankPlayer rankInfo in mapSNRank.Players) {
                this.MapSNRankInfoList.Add(rankInfo);
            }
            if (this.Page == 1 && this.view.IsVisible) {
                this.SetFirstPageContainSelfInfo();
                this.SetSelfRankInfo();
                this.NeedRefresh = false;
                this.view.ResetItems(newDataCount);
                if (!this.IsLoadAll) {
                    this.view.DataRequestAction += this.MapSNRankReq;
                }
            } else {
                this.view.InsertItems(originCount, newDataCount);
            }
        }

        private void OnInvisible() {
            this.isRequestPage = false;
            this.NeedRefresh = true;
            this.IsLoadAll = false;
            this.Page = 0;
        }

        /***********************************/
    }
}
