using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

// To do: When not in alliance need a show page.
namespace Poukoute {
    public class ChatAllianceViewModel : BaseViewModel {
        private ChatRoomViewModel parent;
        private ChatRoomModel model;
        private ChatAllianceView view;

        /* Model data get set */
        public List<AllianceMessage> AllianceMessageList {
            get {
                if (this.OnlyShowInfo) {
                    return this.model.allianceInfoList;
                } else {
                    return this.model.allianceMessageList;
                }
            }
        }

        public List<AllianceMessage> AllianceSlaveInfoList {
            get {
                return this.model.allianceSlaveInfoList;
            }
        }

        public ChatChannel Channel {
            get {
                return this.model.channel;
            }
        }
        
        public bool OnlyShowInfo {
            get; set;
        }
        /**********************/

        /* Other members */
        public bool IsVisible {
            get {
                return this.view.IsVisible;
            }
        }
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<ChatAllianceView>();
            this.parent = this.transform.parent.GetComponent<ChatRoomViewModel>();
            this.model = ModelManager.GetModelData<ChatRoomModel>();
            this.OnlyShowInfo = false;
            NetHandler.AddNtfHandler(typeof(AllianceMessageNtf).Name,
                                                    this.AllianceMessageNtf);
        }

        private void Start() {
            TriggerManager.Regist(Trigger.BeenKickedOutAlliance, this.OnBeenKickcedOutALliance);
        }

        public void Show() {
            this.view.Show();
            this.view.ResetItems(this.AllianceMessageList.Count, true);
        }

        public void Hide() {
            this.view.Hide();
        }

        protected override void OnReLogin() {
            if (!RoleManager.GetAllianceId().CustomIsEmpty()) {
                this.GetAllianceChatReq();
            }
        }

        public void ShowPlayerDetailInfo(string playerId) {
            this.parent.ShowPlayerDetailInfo(playerId);
        }

        public void GetAllianceChatReq() {
            GetAllianceMessagesReq allianceMessageReq = new GetAllianceMessagesReq();
            NetManager.SendMessage(allianceMessageReq,
                typeof(GetAllianceMessagesAck).Name, this.GetAllianceChatAck);
        }

        private void GetAllianceChatAck(IExtensible message) {
            GetAllianceMessagesAck ack = message as GetAllianceMessagesAck;
            this.model.Refresh(ack);
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void AcceptApplyReq(string id) {
            ApprovalJoinReq approvalJoinReq = new ApprovalJoinReq() {
                Id = id
            };
            NetManager.SendMessage(approvalJoinReq, string.Empty, null);
        }

        public void RefuseApplyReq(string id) {
            RefuseJoinReq refuseJoinReq = new RefuseJoinReq() {
                Id = id
            };
            NetManager.SendMessage(refuseJoinReq, string.Empty, null);
        }

        private void UpdateAllianceMessage(AllianceMessage message, string method) {
            for (int index = 0; index < this.AllianceMessageList.Count; index++) {
                if (message.Id.CustomEquals(this.AllianceMessageList[index].Id)) {
                    this.AllianceMessageList[index] = message;
                    this.view.UpdateApplyView(index, method.CustomEquals("del"));
                    return;
                }
            }
            Debug.LogError("no exist message");
        }

        private void OnBeenKickcedOutALliance() {
            this.model.allianceInfoList.Clear();
            this.model.allianceMessageList.Clear();
            this.AllianceSlaveInfoList.Clear();
        }

        /* Add 'NetMessageAck' function here*/
        private void AllianceMessageNtf(IExtensible message) {
            AllianceMessageNtf newMessage = message as AllianceMessageNtf;
            //Debug.LogError("newMessage " + newMessage.Method);
            if (newMessage.Method.CustomEquals("new")) {
                if (newMessage.Message.SlaveInfo != null) {
                    this.AllianceSlaveInfoList.Add(newMessage.Message);
                } else {
                    this.AllianceMessageList.Add(newMessage.Message);
                    if (this.AllianceMessageList.Count > this.model.allianceMessageMaxCount) {
                        this.AllianceMessageList.RemoveAt(0);
                    }
                }
                if (this.Channel != ChatChannel.Alliance) {
                    this.parent.NewAllianceMessage = true;
                } else if (this.view.IsVisible) {
                    this.view.ResetItems(this.AllianceMessageList.Count, true);
                }
            } else if (newMessage.Method.CustomEquals("update") ||
                       newMessage.Method.CustomEquals("del")) {
                this.UpdateAllianceMessage(newMessage.Message, newMessage.Method);
            }
            this.parent.SetMapChatInfoView(ChatChannel.Alliance);
        }
        /***********************************/
    }
}
