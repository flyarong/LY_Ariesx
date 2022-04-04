using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class ChatPrivateViewModel : BaseViewModel {
        private ChatRoomViewModel parent;
        private ChatRoomModel model;
        private ChatPrivateView view;
        /* Model data get set */

        public ChatChannel Channel {
            get {
                return this.model.channel;
            }
        }

        public List<PersonalMessage> MessageList {
            get {
                return this.model.privateList;
            }
        }

        public Dictionary<string, PersonalMessage> MessageDict {
            get {
                return this.model.privateDict;
            }
        }

        public int NewPrivateMessage {
            get {
                return this.model.newPrivateMessage;
            }
            set {
                if (this.model.newPrivateMessage != value) {
                    this.model.newPrivateMessage = value;
                }
            }
        }

        public int Page {
            get {
                return this.model.privatePage;
            }
            set {
                this.model.privatePage = value;
            }
        }

        public bool IsLoadAll {
            get {
                return this.model.isPrivateLoadAll;
            }
            set {
                this.model.isPrivateLoadAll = value;
            }
        }
        /**********************/

        /* Other members */
        private bool isRequestPage = false;
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<ChatRoomViewModel>();
            this.model = ModelManager.GetModelData<ChatRoomModel>();
            this.view = this.gameObject.AddComponent<ChatPrivateView>();
            NetHandler.AddDataHandler(typeof(NewPersonalMessageNtf).Name,
                                                this.NewPersonalMessageNtf);
        }

        public void Show() {
            this.view.Show();
            this.IsLoadAll = false;
            this.isRequestPage = false;
            this.Page = 1;
            this.NormalMailReq();
        }

        public void Hide() {
            this.view.Hide();
        }

        protected override void OnReLogin() {
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        private void ClearAll() {
            this.MessageList.Clear();
            this.MessageDict.Clear();
        }

        public void ShowMailNew() {
            this.parent.ShowMailNew();
        }

        public void ShowMail(PersonalMessage mail) {
            if (!mail.IsRead) {
                this.NewPrivateMessage--;
                this.MarkAsRead(mail.PlayerId);
                mail.IsRead = true;
            }
            this.parent.ShowMailDetail(mail.PlayerId, typeof(ChatPrivateView));
        }

        public void MarkAsRead(string playerId) {
            MarkPersonalMessagesReadReq markAllReq = new MarkPersonalMessagesReadReq();
            markAllReq.Id.Add(playerId);
            NetManager.SendMessage(markAllReq, string.Empty, null);
        }

        public void NormalMailReq() {
            if (this.IsLoadAll || this.isRequestPage) {
                return;
            }
            this.isRequestPage = true;
            GetPersonalMessagesReq getMails = new GetPersonalMessagesReq();
            NetManager.SendMessage(getMails,
                typeof(GetPersonalMessagesAck).Name, this.NormalMailAck);
        }
        /* Add 'NetMessageAck' function here*/
        // May cause confict with system req.
        private void NormalMailAck(IExtensible message) {
            GetPersonalMessagesAck getMailsAck = message as GetPersonalMessagesAck;
            this.isRequestPage = false;
            this.IsLoadAll = true;
            foreach (PersonalMessage mail in getMailsAck.PersonalMessages) {
                if (!this.MessageDict.ContainsKey(mail.PlayerId)) {
                    this.MessageList.Add(mail);
                    this.MessageDict[mail.PlayerId] = mail;
                }
            }
            this.ResortPrivateMessage();
            this.view.ResetItems(this.MessageList.Count);
        }

        // To do: need tell system and normal mail.
        private void NewPersonalMessageNtf(IExtensible message) {
            NewPersonalMessageNtf newMails = message as NewPersonalMessageNtf;
            if (newMails.PersonalMessage.Messages.Count != 0) {
                this.NewPersonalMessage(newMails.PersonalMessage);
            }
        }

        public void NewPersonalMessage(PersonalMessage message) {
            PersonalMessage personalMessage;
            if (this.MessageDict.TryGetValue(message.PlayerId, out personalMessage)) {
                if (this.view.IsVisible) {
                    this.view.UpdateMail(message);
                }

                this.MessageList.Remove(personalMessage);
                personalMessage = message;
                this.MessageList.Add(personalMessage);
                this.MessageDict[message.PlayerId] = personalMessage;
                this.ResortPrivateMessage();
            } else {
                this.MessageList.Insert(0, message);
                this.MessageDict.Add(message.PlayerId, message);
                this.view.InsertItems(0, 1);
            }
        }

        private void ResortPrivateMessage() {
            this.MessageList.Sort((a, b) => {
                return -a.Timestamp.CompareTo(b.Timestamp);
            });
        }
        /***********************************/

        void OnDisable() {
            this.isRequestPage = false;
        }
    }
}
