using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class ChatStateViewModel : BaseViewModel {
        private ChatRoomViewModel parent;
        private ChatRoomModel model;
        private ChatStateView view;
        /* Model data get set */
        public List<ChatMessage> MessageList {
            get {
                return this.model.stateMessageList;
            }
        }

        public ChatChannel Channel {
            get {
                return this.model.channel;
            }
        }

        /*****************************************************/

        void Awake() {
            this.view = this.gameObject.AddComponent<ChatStateView>();
            this.parent = this.transform.parent.GetComponent<ChatRoomViewModel>();
            this.model = ModelManager.GetModelData<ChatRoomModel>();
            NetHandler.AddNtfHandler(typeof(StateChatNtf).Name, this.StateChatNtf);
        }

        public void Show() {
            this.view.Show();
            this.view.ResetItems(this.MessageList.Count, true);
        }

        public void Hide() {
            this.view.Hide();
        }

        protected override void OnReLogin() {
            this.GetChatReq();
        }

        public void ShowPlayerDetailInfo(string playerId) {
            this.parent.ShowPlayerDetailInfo(playerId);
        }

        public void GetChatReq() {
            GetStateChatReq allStateChat = new GetStateChatReq();
            NetManager.SendMessage(allStateChat,
                typeof(GetStateChatAck).Name, this.GetChatAck);
        }

        public void GetChatAck(IExtensible message) {
            GetStateChatAck ack = message as GetStateChatAck;
            this.model.Refresh(ack);
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        private void StateChatNtf(IExtensible message) {
            StateChatNtf newMessage = message as StateChatNtf;
            this.MessageList.Add(newMessage.Message);
            if (this.MessageList.Count > this.model.worldMaxCount) {
                this.MessageList.RemoveAt(0);
            }

            this.parent.NewStateMessage = !this.view.IsVisible;

            this.parent.SetMapChatInfoView(ChatChannel.State);
            if (this.view.IsVisible) {
                this.view.ResetItems(this.MessageList.Count, true);
            }
        }
    }
}
