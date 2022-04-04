using ProtoBuf;
using Protocol;
using System;
using System.Collections.Generic;

namespace Poukoute {
    public class AllianceMembersViewModel : BaseViewModel {
        private AllianceViewModel parent;
        private AllianceMembersView view;
        private AllianceDetailModel model;
        //Public propeties

        public List<AllianceMemberWithIndex> SelfAllianceMembersInfo {
            get {
                return this.model.allianceDetail.membersList;
            }
        }

        public string AllianceLogo {
            get {
                return this.model.allianceLogo;
            }
            set {
                this.model.allianceLogo = value;
            }
        }

        public Alliance SelfAlliance {
            get {
                return this.model.allianceDetail.alliance;
            }

            set {
                if (this.model.allianceDetail.alliance != value) {
                    this.model.allianceDetail.alliance = value;
                    this.AllianceLogo = value != null ? value.Emblem.ToString() : "1";
                    this.model.influnceCondition = value != null ? value.JoinCondition.ForceLimit : 0;
                    this.model.joinCondition =
                        value != null ?
                        (JoinConditionType)Enum.ToObject(typeof(JoinConditionType), value.JoinCondition.Type) :
                        JoinConditionType.Free;
                }
            }
        }

        private EventAbdication SelfEventAbdication {
            get {
                return this.SelfAlliance != null ? this.SelfAlliance.EventAbdication : null;
            }
        }

        public string SelfAllianceId {
            get {
                return this.SelfAlliance != null ? this.SelfAlliance.Id : string.Empty;
            }
        }

        public bool IsLoadAll {
            get {
                return this.model.allianceDetail.isLoadAll;
            }
            set {
                this.model.allianceDetail.isLoadAll = value;
            }
        }

        public int Page {
            get {
                return this.model.allianceDetail.page;
            }
            set {
                this.model.allianceDetail.page = value;
            }
        }

        public int PageCount {
            get {
                return this.model.allianceDetail.pageCount;
            }
        }

        public AllianceMemberWithIndex CurrentPlayer {
            get {
                return this.model.currentPlayerInfo;
            }

            set {
                this.model.currentPlayerInfo = value;
                this.GetPlayerPublickInfo();
            }
        }

        public AllianceMemberSortType SortType {
            get {
                return this.model.allianceDetail.sortType;
            }
            set {
                if (this.model.allianceDetail.sortType != value) {
                    this.model.allianceDetail.sortType = value;
                    this.ReSetMemberList();
                }
            }
        }

        #region other data(s)
        public bool NeedFresh {
            get; set;
        }

        private bool GetAllianceMembering = false;
        private string viewAllianceId = string.Empty;
        private ButtonClickWithLabel promoteBtn = null;
        private ButtonClickWithLabel demoteBtn = null;
        private ButtonClickWithLabel kickoutBtn = null;
        #endregion

        private void Awake() {
            this.parent = this.transform.parent.GetComponent<AllianceViewModel>();
            this.view = this.gameObject.AddComponent<AllianceMembersView>();
            this.model = ModelManager.GetModelData<AllianceDetailModel>();
        }

        private void Start() {
            this.promoteBtn = new ButtonClickWithLabel(
                        LocalManager.GetValue(LocalHashConst.alliance_promote), this.PromotePlayer);
            this.demoteBtn = new ButtonClickWithLabel(
                        LocalManager.GetValue(LocalHashConst.alliance_demotion), this.DemotionPlayer);
            this.kickoutBtn = new ButtonClickWithLabel(
                        LocalManager.GetValue(LocalHashConst.alliance_kickout), this.KickMemberOut);
        }

        public void ShowAllianceMembersList(string allianceId, bool isShowBtn = true) {
            if (!this.view.IsVisible) {
                this.view.PlayShow();
                TriggerManager.Regist(Trigger.BeenKickedOutAlliance, this.Hide);
            }
            this.viewAllianceId = allianceId.CustomIsEmpty() ? this.SelfAllianceId : allianceId;
            this.NeedFresh = true;
            this.GetAllianceMember();
            this.view.SetAllianceSortContent();
            this.view.ShowSortBtn(isShowBtn);
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.PlayHide();
                this.ResetStatus();
                TriggerManager.Unregist(Trigger.BeenKickedOutAlliance, this.Hide);
            }
        }

        private void GetAllianceMember() {
            if (this.GetAllianceMembering || this.IsLoadAll) {
                return;
            }

            if (this.NeedFresh) {
                this.IsLoadAll = false;
                this.SelfAllianceMembersInfo.Clear();
                this.Page = 0;
            }

            GetAllianceMembersReq getAllianceMembersReq = new GetAllianceMembersReq() {
                Id = this.viewAllianceId,
                Page = ++this.Page,
                OrderBy = Enum.GetName(typeof(AllianceMemberSortType), this.SortType)
            };
            this.GetAllianceMembering = true;
            NetManager.SendMessage(getAllianceMembersReq,
                                    typeof(GetAllianceMembersAck).Name,
                                    this.GetAllianceMemberAck);
        }

        public void SetCurrentPlayerInfo(AllianceMemberWithIndex memeber) {
            this.CurrentPlayer = memeber;
        }

        private void ReloadAllianceInfo() {
            this.ResetStatus();
            this.parent.ShowAllianceInfo();
        }

        public void ShowAllianceDisplayBoard(DisplayType type) {
            this.parent.ShowAllianceDisplayBoard(type);
        }

        private void ResetStatus() {
            this.IsLoadAll = false;
            this.NeedFresh = true;
            this.GetAllianceMembering = false;
        }

        #region NetMessageReq function
        private void GetPlayerPublickInfo() {
            GetPlayerPublicInfoReq getPlayerInfo = new GetPlayerPublicInfoReq() {
                Id = this.CurrentPlayer.member.Id
            };
            NetManager.SendMessage(getPlayerInfo,
                                    typeof(GetPlayerPublicInfoAck).Name,
                                    this.GetPlayerPublicInfoAck);
        }

        private bool isNeedRefresh = false;
        private void PromotePlayer() {
            if ((AllianceRole)(this.CurrentPlayer.member.Role + 1) == AllianceRole.Owner) {
                UIManager.ShowConfirm(
                    LocalManager.GetValue(LocalHashConst.notice_title_warning),
                    string.Format(LocalManager.GetValue(LocalHashConst.alliance_promote_tip),
                                  this.CurrentPlayer.member.Name),
                    () => {
                        this.isNeedRefresh = true;
                        this.PromotePlayerReq();
                    }, () => { });
            } else {
                this.isNeedRefresh = false;
                this.PromotePlayerReq();
            }
        }

        private void KickMemberOut() {
            UIManager.ShowConfirm(
                    LocalManager.GetValue(LocalHashConst.notice_title_warning),
                    string.Format(LocalManager.GetValue(LocalHashConst.alliance_kickout_tip),
                                                         this.CurrentPlayer.member.Name),
                    this.KickMemberOutReq, () => { });
        }

        private void PromotePlayerReq() {
            PromoteReq promoteReq = new PromoteReq() {
                Id = this.CurrentPlayer.member.Id,
                Role = (int)this.CurrentPlayer.member.Role + 1
            };
            NetManager.SendMessage(promoteReq,
                                   typeof(PromoteAck).Name,
                                   this.PromotePlayerAck);
        }

        private void DemotionPlayer() {
            PromoteReq promoteReq = new PromoteReq() {
                Id = this.CurrentPlayer.member.Id,
                Role = (int)this.CurrentPlayer.member.Role - 1
            };
            NetManager.SendMessage(promoteReq,
                                    typeof(PromoteAck).Name,
                                    this.DemotionPlayerAck);
        }

        private void KickMemberOutReq() {
            KickMemberReq kickMemberReq = new KickMemberReq() {
                Id = this.CurrentPlayer.member.Id
            };
            NetManager.SendMessage(kickMemberReq,
                                    typeof(KickMemberAck).Name,
                                    this.KickMemberOutAck);
        }

        private void ReSetMemberList() {
            this.GetAllianceMembering = false;
            this.NeedFresh = true;
            this.IsLoadAll = false;
            this.GetAllianceMember();
        }
        #endregion


        #region NetMessageAck function
        private void GetPlayerPublicInfoAck(IExtensible message) {
            GetPlayerPublicInfoAck playerInfoAck = message as GetPlayerPublicInfoAck;

            long abdicationTime = (this.SelfEventAbdication != null) ?
                    this.SelfEventAbdication.FinishAt : 0;
            int ownAllianceRoleInteger = (int)RoleManager.GetAllianceRole();
            bool canChangeRole = ownAllianceRoleInteger > (int)AllianceRole.Elder;
            bool canChangePlayerRole = ownAllianceRoleInteger > playerInfoAck.Info.AllianceRole;
            this.promoteBtn.enable =
            this.demoteBtn.enable = canChangeRole && canChangePlayerRole;

            this.parent.ShowAllianceMemOperate(
                playerInfoAck.Info, this.promoteBtn,
                playerInfoAck.Info.AllianceRole == (int)AllianceRole.Member ?
                this.kickoutBtn : this.demoteBtn
            );
        }

        private void GetAllianceMemberAck(IExtensible message) {
            GetAllianceMembersAck ackMsg = message as GetAllianceMembersAck;
            if (ackMsg.Members.Count < this.PageCount) {
                this.IsLoadAll = true;
            }

            int originCount = this.SelfAllianceMembersInfo.Count;
            int newDataCount = ackMsg.Members.Count;
            int index = 0;
            foreach (AllianceMember memeber in ackMsg.Members) {
                AllianceMemberWithIndex memberWithIndex = new AllianceMemberWithIndex() {
                    member = memeber,
                    index = ++index,
                    abdicationTime = (this.SelfEventAbdication != null) ?
                    this.SelfEventAbdication.FinishAt : 0
                };
                this.SelfAllianceMembersInfo.Add(memberWithIndex);
            }

            this.view.SetMembersInfo();
            if (this.Page == 1 && this.view.IsVisible) {
                this.NeedFresh = false;
                this.view.ResetItems(newDataCount);
                if (!this.IsLoadAll) {
                    this.view.DataRequestAction += this.GetAllianceMember;
                }
            } else {
                this.view.InsertItems(originCount, newDataCount);
            }
            this.GetAllianceMembering = false;
        }

        private void KickMemberOutAck(IExtensible message) {
            this.ReSetMemberList();
        }

        private void PromotePlayerAck(IExtensible message) {
            if (this.isNeedRefresh) {
                this.ReloadAllianceInfo();
                this.isNeedRefresh = false;
            } else {
                this.CurrentPlayer.member.Role += 1;
                this.view.RefreshCurrentPlayer();
            }
        }

        private void DemotionPlayerAck(IExtensible message) {
            this.CurrentPlayer.member.Role -= 1;
            this.view.RefreshCurrentPlayer();
        }
        #endregion
    }
}
