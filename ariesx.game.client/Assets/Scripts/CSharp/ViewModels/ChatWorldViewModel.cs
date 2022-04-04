using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class ChatWorldViewModel : BaseViewModel {
        private ChatRoomViewModel parent;
        private ChatRoomModel model;
        private ChatWorldView view;
        /* Model data get set */
        public List<ChatMessage> MessageList {
            get {
                return this.model.worldMessageList;
            }
        }

        public bool IsLoadAll {
            get; set;
        }

        public ChatChannel Channel {
            get {
                return this.model.channel;
            }
        }
        /**********************/

        void Awake() {
            this.view = this.gameObject.AddComponent<ChatWorldView>();
            this.parent = this.transform.parent.GetComponent<ChatRoomViewModel>();
            this.model = ModelManager.GetModelData<ChatRoomModel>();
            NetHandler.AddNtfHandler(typeof(WorldChatNtf).Name, this.WorldChatNtf);
        }

        public void Show() {
            this.view.Show();
            this.view.ResetItems(this.MessageList.Count, true);
        }

        public void Hide() {
            this.view.Hide();
        }

        // To do: set needrefresh.
        protected override void OnReLogin() {
            this.GetChatReq();
        }

        public void ShowPlayerDetailInfo(string playerId) {
            this.parent.ShowPlayerDetailInfo(playerId);
        }

        public void GetChatReq() {
            GetWorldChatReq allWordChat = new GetWorldChatReq();
            NetManager.SendMessage(allWordChat,
                typeof(GetWorldChatAck).Name, this.GetChatAck);
        }

        public void GetChatAck(IExtensible message) {
            GetWorldChatAck ack = message as GetWorldChatAck;
            this.model.Refresh(ack);
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        private void WorldChatNtf(IExtensible message) {
            WorldChatNtf newMessage = message as WorldChatNtf;
            this.MessageList.Add(newMessage.Message);
            if (this.MessageList.Count > this.model.worldMaxCount) {
                this.MessageList.RemoveAt(0);
            }

            this.parent.NewWorldMessage = !this.view.IsVisible;

            this.parent.SetMapChatInfoView(ChatChannel.World);
            if (this.view.IsVisible) {
                this.view.ResetItems(this.MessageList.Count, true);
            }
        }
    }
}
