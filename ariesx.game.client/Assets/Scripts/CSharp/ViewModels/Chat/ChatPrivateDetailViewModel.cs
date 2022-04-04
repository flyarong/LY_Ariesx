using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class ChatPrivateDetailViewModel : BaseViewModel, IViewModel {
        private ChatRoomViewModel parent;
        private ChatPrivateDetailView view;

        /* Other members */
        private PersonalMessage mail;
        public PersonalMessage Mail {
            get {
                return this.mail;
            }
            set {
                //if (this.mail != value) {
                    this.mail = value;
                    this.OnMailChange();
                //}
            }
        }

        private int page;
        public int Page {
            get {
                return this.page;
            }
            set {
                this.page = value;
            }
        }

        private string message;
        public string Message {
            get {
                return this.message;
            }
            set {
                this.message = value;
            }
        }

        public bool IsVisible {
            get {
                return this.view.IsVisible;
            }
        }

        private bool NeedRefresh { get; set; }
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<ChatRoomViewModel>();
            this.view = this.gameObject.AddComponent<ChatPrivateDetailView>();
            NetHandler.AddNtfHandler(typeof(NewPersonalMessageNtf).Name,
                                        this.NewPersonalMessageNtf);
            this.NeedRefresh = true;
        }        

        public void RefreshCurrentPrivateChat(string message) {
            this.Show(this.Mail.PlayerId);
        }

        private void OnMailChange() {
            if (!this.view.IsVisible) {
                return;
            }
            this.view.OnMailChange(this.NeedRefresh);
        }

        public void Show(string playerId) {
            if (!this.IsVisible) {
                this.view.Show();
            }
            this.GetPersonalMessageReq(playerId);
        }

        public void Hide() {
            if (this.IsVisible) {
                this.view.Hide();

            }
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        // To do: set needrefresh.
        protected override void OnReLogin() {
            if (this.IsVisible) {
                this.Show(this.Mail.PlayerId);
            }
        }

        public void Return() {
            this.parent.Return();
        }

        private void GetPersonalMessageReq(string playerId) {
            GetPersonalMessageReq getPersonalMailsReq = new GetPersonalMessageReq() {
                Id = playerId
            };
            NetManager.SendMessage(getPersonalMailsReq,
                typeof(GetPersonalMessageAck).Name, this.GetPersonalMailsAck);
        }

        private void MarkReadReq() {
            this.parent.MarkAsRead(this.Mail.PlayerId);
        }

        /* Add 'NetMessageAck' function here*/
        public void NewPersonalMessageNtf(IExtensible message) {
            if (this.IsVisible) {
                NewPersonalMessageNtf newPersonalMailNtf = message as NewPersonalMessageNtf;
                string newMessagePlayerId = newPersonalMailNtf.PersonalMessage.PlayerId;
                if (newMessagePlayerId.CustomEquals(this.Mail.PlayerId)) {
                    this.NeedRefresh = false;
                    this.Mail = newPersonalMailNtf.PersonalMessage;
                    this.MarkReadReq();
                }
            }
        }

        private void GetPersonalMailsAck(IExtensible message) {
            GetPersonalMessageAck getPernalMailsAck = message as GetPersonalMessageAck;
            this.NeedRefresh = true;
            this.Mail = getPernalMailsAck.PersonalMessage;
        }
        /***********************************/
    }
}
