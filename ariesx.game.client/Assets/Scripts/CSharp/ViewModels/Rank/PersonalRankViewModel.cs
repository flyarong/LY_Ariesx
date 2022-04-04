using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class PersonalRankViewModel : BaseViewModel {
        private RankModel model;
        private PersonalRankView view;
        private RankViewModel parent;
        private BuildModel buildModel;
        private const int townhallRequireLevel = 3;

        public PersonalRank PersonalRankInfo {
            get {
                return this.model.PersonalRankInfo;
            }
        }

        public RankPlayer OwnRankInfo {
            get {
                return this.PersonalRankInfo.ownRankInfo;
            }
            set {
                if (this.PersonalRankInfo.ownRankInfo != value) {
                    this.PersonalRankInfo.ownRankInfo = value;
                }
            }
        }

        public List<RankPlayer> RankInfoList {
            get {
                return this.PersonalRankInfo.rankDataList;
            }
        }

        public bool IsLoadAll {
            get {
                return this.PersonalRankInfo.isLoadAll;
            }
            set {
                this.PersonalRankInfo.isLoadAll = value;
            }
        }

        public int Page {
            get {
                return this.PersonalRankInfo.page;
            }
            set {
                this.PersonalRankInfo.page = value;
            }
        }

        public int PageCount {
            get {
                return this.PersonalRankInfo.pageCount;
            }
        }


        /***** Other *****/
        private bool firstPageContainOwnInfo = false;
        private bool NeedRefresh { get; set; }
        private bool isRequestPage = false;
        /*****************/

        private void Awake() {
            this.view = this.gameObject.AddComponent<PersonalRankView>();
            this.parent = this.transform.parent.GetComponent<RankViewModel>();
            this.model = ModelManager.GetModelData<RankModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.NeedRefresh = true;
        }

        public void Show() {
            this.view.Show();
            bool isUnlockedWorldRank = !this.UnlockedWorldRank();
            this.view.SetUnlockedWorldRankCG(isUnlockedWorldRank);
            if (isUnlockedWorldRank) {
                return;
            }
            if (this.NeedRefresh) {
                this.IsLoadAll = false;
                this.Page = 0;
                this.RankInfoListReq();
            } else {
                this.SetOwnRankInfo();
            }
        }

        public bool UnlockedWorldRank() {
            return this.buildModel.GetBuildLevelByName(ElementName.townhall) >=
                townhallRequireLevel;
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

        public void ShowPlayerInfo(string playerId) {
            this.parent.ShowPlayerInfo(playerId);
        }

        public void SetOwnRankInfo() {
            this.parent.SetOwnRankInfo();
        }

        public void RankInfoListReq() {
            if (this.IsLoadAll || this.isRequestPage) {
                return;
            }
            this.isRequestPage = true;
            RankReq rankReq = new RankReq() {
                Page = ++this.Page
            };
            NetManager.SendMessage(rankReq, typeof(RankAck).Name,
                this.RankInfoAck);
        }

        public void JumpToOwnRank() {
            this.view.ScrollTo(this.OwnRankInfo.Rank);
        }

        public bool IsShowOwnRankInfo() {
            if (this.OwnRankInfo == null ||
                this.firstPageContainOwnInfo) {
                return false;
            }
            return this.IsOwnRankInfoVisible();
        }

        public bool IsOwnRankInfoVisible() {
            return this.OwnRankInfo != null && this.OwnRankInfo.Rank != 0;
        }

        private void RankInfoAck(IExtensible message) {
            RankAck rankAck = message as RankAck;
            this.isRequestPage = false;
            int newDataCount = rankAck.Players.Count;
            if (newDataCount < this.PageCount) {
                this.IsLoadAll = true;
            }

            if (this.NeedRefresh) {
                this.RankInfoList.Clear();
            }

            int originCount = this.RankInfoList.Count;
            this.OwnRankInfo = rankAck.Self;
            foreach (RankPlayer rankInfo in rankAck.Players) {
                this.RankInfoList.Add(rankInfo);
            }

            if (this.Page == 1 && this.view.IsVisible) {
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
            string roleId = RoleManager.GetRoleId();
            foreach (RankPlayer rankInfo in this.RankInfoList) {
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
