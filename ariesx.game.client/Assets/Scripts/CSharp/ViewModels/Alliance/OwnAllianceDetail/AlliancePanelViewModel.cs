using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class AlliancePanelViewModel : BaseViewModel {
        private AllianceDetailViewModel parent;
        private AllianceDetailModel model;
        private AllianceCreateOrJoinModel allianceCreateOrJoinModel;
        private AlliancePanelView view;

        /* Model data get set */
        public PlayerPublicInfo SelfPlayerPublickInfo {
            get {
                return this.model.allianceDetail.selfInfo;
            }
            set {
                if (this.model.allianceDetail.selfInfo != value) {
                    this.model.allianceDetail.selfInfo = value;
                    RoleManager.ResetOwnAllianceRole(
                        value == null ? (int)AllianceRole.None : value.AllianceRole);
                }
            }
        }

        public AllianceViewType Channel {
            get {
                return this.model.allianceViewType;
            }
        }

        public Alliance SelfAlliance {
            get {
                return this.model.allianceDetail.alliance;
            }

            set {
                if (this.model.allianceDetail.alliance != value) {
                    this.model.allianceDetail.alliance = value;
                    this.model.influnceCondition = value != null ? value.JoinCondition.ForceLimit : 0;
                    this.model.joinCondition = value != null ? (JoinConditionType)Enum.ToObject(typeof(JoinConditionType),
                        value.JoinCondition.Type) : JoinConditionType.Free;
                }
            }
        }

        public int AllianceEmblem {
            get {
                return this.allianceCreateOrJoinModel.allianceEmblem;
            }
            set {
                this.allianceCreateOrJoinModel.allianceEmblem = value;
            }
        }

        public string AllianceId {
            get {
                return this.SelfAlliance != null ? this.SelfAlliance.Id : string.Empty;
            }
        }
        /**********************/

        /* Other members */
        public bool NeedFresh {
            get; set;
        }

        //private bool GetAllianceMembering = false;
        //private AllianceMembersViewModel allianceMembersViewModel;
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<AlliancePanelView>();
            this.allianceCreateOrJoinModel = ModelManager.GetModelData<AllianceCreateOrJoinModel>();
            this.model = ModelManager.GetModelData<AllianceDetailModel>();
            this.parent = this.transform.parent.GetComponent<AllianceDetailViewModel>();
            this.NeedFresh = true;
        }

        public void Show() {
            this.view.Show();
            this.GetMyAlliance();
        }

        public void Hide() {
            this.view.Hide(() => {
                this.NeedFresh = true;
            });
        }

        protected override void OnReLogin() {
            this.NeedFresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void ShowAllianceMemOperate(PlayerPublicInfo playerInfo,
                ButtonClickWithLabel greenBtnInfo, ButtonClickWithLabel redBtnInfo) {
            this.parent.ShowAllianceMemOperate(playerInfo, greenBtnInfo, redBtnInfo);
        }

        public void ShowAllianceChatroom() {
            this.parent.ShowAllianceChatroom();
        }

        public void ShowMembersList() {
            this.parent.ShowAllianceMembersList(this.AllianceId);
        }

        private UnityAction OnGetMyAlliance;
        public void RefreshAllianceInfo() {
            this.GetMyAlliance();
            this.OnGetMyAlliance = this.ShowMembersList;
        }

        public void ShowEditAllianceInfoPanel() {
            this.parent.ShowSubWindowByType(AllianceSubWindowType.Setting);
        }

        /* Add 'NetMessageReq' function here*/
        public void GetMyAlliance() {
            GetMyAllianceReq getMyAllianceReq = new GetMyAllianceReq();
            NetManager.SendMessage(getMyAllianceReq,
                                    typeof(GetMyAllianceAck).Name,
                                    this.GetMyAllianceAck);
        }



        public void ResetUserAllianceInfo() {
            this.SelfPlayerPublickInfo = null;
            this.SelfAlliance = null;
        }

        public void DissolveAllianceReq() {
            DissolveAllianceReq dissolveAlliance = new DissolveAllianceReq();
            NetManager.SendMessage(dissolveAlliance,
                                    typeof(DissolveAllianceAck).Name,
                                    this.DissolveAllianceAck);
        }

        public void QuitAllianceReq() {
            QuitAllianceReq quitAllianceReq = new QuitAllianceReq();
            NetManager.SendMessage(quitAllianceReq,
                                    typeof(QuitAllianceAck).Name,
                                    this.QuitAllianceAck);
            this.parent.IsInitiativeQuitAlliance = true;
        }

        public void ShowAllianceDisplayBoard(DisplayType tape) {
            this.parent.ShowAllianceDisplayBoard(tape);
        }
        /***********************************/


        /* Add 'NetMessageAck' function here*/
        private void DissolveAllianceAck(IExtensible message) {
            this.QuitAllianceAck(message);
        }

        private void QuitAllianceAck(IExtensible message) {
            this.SelfPlayerPublickInfo = null;
            this.SelfAlliance = null;
        }

        private void GetMyAllianceAck(IExtensible message) {
            GetMyAllianceAck getMyAllianceAck = message as GetMyAllianceAck;
            this.SelfPlayerPublickInfo = getMyAllianceAck.Self;
            this.SelfAlliance = getMyAllianceAck.Alliance;
            this.AllianceEmblem = getMyAllianceAck.Alliance.Emblem;
            if (this.view.IsVisible) {
                this.view.SetAllianceHeadContent(
                    this.SelfAlliance, (AllianceRole)this.SelfPlayerPublickInfo.AllianceRole);
            } else {
                this.NeedFresh = true;
            }

            if (this.OnGetMyAlliance != null) {
                this.OnGetMyAlliance.Invoke();
                this.OnGetMyAlliance = null;
            }
        }
        /***********************************/
    }
}
