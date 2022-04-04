using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class ChatNewPriavteViewModel : BaseViewModel, IViewModel {
        private ChatRoomViewModel parent;
        private ChatRoomModel model;
        private ChatNewPriavteView view;
        /* Model data get set */

        //public string PrivateMsgUserName {
        //    get {
        //        return this.model.privateMsgUserName;
        //    }
        //    set {
        //        if (this.model.privateMsgUserName != value) {
        //            this.model.privateMsgUserName = value;
        //        }
        //    }
        //}

        public string PrivateMsgTo {
            get {
                return this.model.privateMsgTo;
            }
            set {
                if (this.model.privateMsgTo != value) {
                    this.model.privateMsgTo = value;
                }
            }
        }

        public string PrivateMsgContent {
            get {
                return this.model.privateMsgContent;
            }
            set {
                if (this.model.privateMsgContent != value) {
                    this.model.privateMsgContent = value;
                }
            }
        }

        public MailType MailType {
            get {
                return this.model.mailType;
            }
            set {
                this.model.mailType = value;
            }
        }
        /**********************/

        /* Other members */
        public bool NeedRefresh {
            get; set;
        }
        /*****************/

        void Awake() {
            this.parent = this.transform.parent.GetComponent<ChatRoomViewModel>();
            this.model = ModelManager.GetModelData<ChatRoomModel>();
            this.view = this.gameObject.AddComponent<ChatNewPriavteView>();
            this.NeedRefresh = true;
        }

        public void Show(string userName, string userId, MailType mailType) {
            this.view.Show();
            this.PrivateMsgTo = userName.CustomIsEmpty() ? userId : userName;
            this.MailType = mailType;
            this.view.SetInfo();
        }

        public void Hide() {
            this.view.Hide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void SendMail() {
            switch (this.MailType) {
                case MailType.Alliance:
                    SendAllianceMessageReq allianceMail = new SendAllianceMessageReq();
                    allianceMail.Content = this.PrivateMsgContent;
                    NetManager.SendMessage(allianceMail,
                        typeof(SendAllianceMessageAck).Name, this.SendAllianceMessageAck);
                    break;
                case MailType.Normal:
                    Debug.LogError("PrivateMsgUserId " + this.PrivateMsgTo);
                    SendPersonalMessageReq mail = new SendPersonalMessageReq() {
                        To = this.PrivateMsgTo,
                        Content = this.PrivateMsgContent
                    };
                    NetManager.SendMessage(mail,
                                           typeof(SendPersonalMessageAck).Name,
                                           this.SendPersonalMessageAck);
                    break;
                default:
                    Debug.LogError("Should not come here");
                    break;
            }
        }

        public void Return() {
            this.parent.Return();
        }

        /* Add 'NetMessageAck' function here*/
        private void SendPersonalMessageAck(IExtensible message) {
            this.parent.Return();
            SendPersonalMessageAck ack = message as SendPersonalMessageAck;
            ack.Message.IsRead = true;
            this.parent.PrivateNewChatAck(ack.Message);
        }

        private void SendAllianceMessageAck(IExtensible message) {
            this.parent.Return();
        }
        /***********************************/
    }
}
