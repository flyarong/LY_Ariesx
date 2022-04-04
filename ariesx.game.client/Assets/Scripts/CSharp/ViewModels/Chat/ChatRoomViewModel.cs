using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class ChatRoomViewModel : BaseViewModel, IViewModel {
        private ChatRoomModel model;
        private ChatRoomView view;
        private MapViewModel parent;

        private bool newWorldMessage;
        public bool NewWorldMessage {
            get {
                return this.newWorldMessage;
            }
            set {
                this.newWorldMessage = value;
                this.OnNewStatusChange(ChatChannel.World);
            }
        }

        private bool newStateMessage;
        public bool NewStateMessage {
            get {
                return this.newStateMessage;
            }
            set {
                this.newStateMessage = value;
                this.OnNewStatusChange(ChatChannel.State);
            }
        }

        private bool newAllianceMessage;
        public bool NewAllianceMessage {
            get {
                return this.newAllianceMessage;
            }
            set {
                this.newAllianceMessage = value;
                this.OnNewStatusChange(ChatChannel.Alliance);
            }
        }

        public int NewPrivateMessage {
            get {
                return this.model.newPrivateMessage;
            }
            set {
                this.model.newPrivateMessage = value;
                this.OnNewStatusChange(ChatChannel.Private);
            }
        }

        public ChatChannel Channel {
            get {
                return this.model.channel;
            }
            set {
                if (this.model.channel != value) {
                    this.model.channel = value;
                    this.OnChannelChange();
                }
            }
        }

        public string PrivateMsgUserId {
            get {
                return this.model.privateMsgTo;
            }
            set {
                if (this.model.privateMsgTo != value) {
                    this.model.privateMsgTo = value;
                }
            }
        }

        /***** Other *****/
        private ChatWorldViewModel worldViewModel;
        private ChatStateViewModel stateViewModel;
        private ChatAllianceViewModel allianceViewModel;
        private ChatPrivateViewModel privateViewModel;
        private ChatPrivateDetailViewModel detailViewModel;
        private ChatNewPriavteViewModel NewPriavteViewModel {
            get {
                if (this.newPriavteViewModel == null) {
                    this.newPriavteViewModel =
                        PoolManager.GetObject<ChatNewPriavteViewModel>(this.transform);
                }
                return this.newPriavteViewModel;
            }
        }
        private ChatNewPriavteViewModel newPriavteViewModel;

        private string allianceId = string.Empty;
        public System.Type PreviouseView {
            get; set;
        }

        public UnityEvent ReturnAction {
            get; set;
        }

        private bool NeedRefresh { get; set; }
        /*****************/

        private void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.model = ModelManager.GetModelData<ChatRoomModel>();
            this.view = this.gameObject.AddComponent<ChatRoomView>();
            this.worldViewModel = PoolManager.GetObject<ChatWorldViewModel>(this.transform);
            this.stateViewModel = PoolManager.GetObject<ChatStateViewModel>(this.transform);
            this.allianceViewModel = PoolManager.GetObject<ChatAllianceViewModel>(this.transform);
            this.privateViewModel = PoolManager.GetObject<ChatPrivateViewModel>(this.transform);
            this.detailViewModel = PoolManager.GetObject<ChatPrivateDetailViewModel>(this.transform);

            this.ReturnAction = new UnityEvent();
            this.NeedRefresh = true;

            NetHandler.AddNtfHandler(typeof(NewPersonalMessageCountNtf).Name,
                                            this.NewPersonalMessageCountNtf);
            FteManager.SetStartCallback(GameConst.ALLIANCE_CHAT, 2, this.OnChatStep2Start);
            FteManager.SetEndCallback(GameConst.ALLIANCE_CHAT, 2, this.OnChatStep2End);
        }

        public void SetMapChatInfoView(ChatChannel channel) {
            switch (channel) {
                case ChatChannel.State:
                    this.parent.SetMapChatStateInfo();
                    break;
                case ChatChannel.World:
                    this.parent.SetMapChatWorldInfo();
                    break;
                case ChatChannel.Alliance:
                    this.parent.SetMapChatAllianceInfo();
                    break;
                default:
                    break;
            }
        }

        public void Show(bool isSubWindows = false) {
            this.view.PlayShow(() => {
                this.parent.OnAddViewAboveMap(this);
            }, true);
            this.view.SetInfo(isSubWindows);
        }

        public void Hide() {
            this.view.PlayHide(() => {
                this.parent.OnRemoveViewAboveMap(this);
                this.detailViewModel.Hide();
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(() => {
                this.detailViewModel.Hide();
                this.parent.OnRemoveViewAboveMap(this);
            });
        }

        public void ShowPlayerDetailInfo(string playerId) {
            this.parent.ShowPlayerDetailInfo(playerId);
        }

        public void MarkAsRead(string playerName) {
            this.privateViewModel.MarkAsRead(playerName);
        }

        public void ShowWorld(bool state) {
            if (state) {
                this.Channel = ChatChannel.World;
                this.worldViewModel.Show();
            } else {
                this.worldViewModel.Hide();
            }
        }

        public void SendMessageTo(string userName, string userId, bool isSubWindow = false) {
            this.Show(isSubWindow);
            this.view.SendMessageTo(userName, userId);
        }

        public void ShowAllianceChatroom() {
            this.view.afterShowCallback = this.view.ShowAllianceChatroom;
            this.Show();
        }

        public void ShowAlliance(bool state) {
            if (state) {
                this.Channel = ChatChannel.Alliance;
                this.allianceViewModel.Show();
            } else {
                this.allianceViewModel.Hide();
            }
        }

        public void ShowState(bool state) {
            if (state) {
                this.Channel = ChatChannel.State;
                this.stateViewModel.Show();
            } else {
                this.stateViewModel.Hide();
            }
        }

        public void ShowPrivate(bool state) {
            if (state) {
                this.Channel = ChatChannel.Private;
                this.privateViewModel.Show();
            } else {
                this.privateViewModel.Hide();
            }
        }

        public void HideAllSubView() {
            this.HideMailNew();
            this.HideMailDetail();
        }

        public void SetPreviouse(System.Type view, UnityAction action) {
            this.view.SetReturn();
            this.PreviouseView = view;
            this.ReturnAction.AddListener(action);
        }

        public void ShowMailNew(string userName = "", string userId = "",
                    MailType mailType = MailType.Normal) {
            this.NewPriavteViewModel.Show(userName, userId, mailType);
            this.SetPreviouse(typeof(ChatPrivateView), this.HideMailNew);
        }

        public void HideMailNew() {
            this.NewPriavteViewModel.Hide();
        }

        public void ShowMailDetail(string playerId, System.Type type) {
            this.privateViewModel.Hide();
            this.detailViewModel.Show(playerId);
            this.SetPreviouse(type, this.HideMailDetail);
            this.PrivateMsgUserId = playerId;
            this.view.ShowTabPrivateMask();
        }

        public void HideMailDetail() {
            this.detailViewModel.Hide();
        }

        public void Return() {
            // To do
            this.view.Return();
            this.ReturnAction.InvokeSafe();
            this.ReturnAction.RemoveAllListeners();
        }

        public void OnBtnSendClick(string input) {
            if (input.CustomIsEmpty()) {
                return;
            }
            switch (this.Channel) {
                case ChatChannel.World:
                    this.WorldChatReq(input);
                    break;
                case ChatChannel.Private:
                    this.PrivateChatReq(input);
                    break;
                case ChatChannel.Alliance:
                    this.AllianceChatReq(input);
                    break;
                case ChatChannel.State:
                    this.StateChatReq(input);
                    break;
                default:
                    break;
            }
        }

        public void OnChannelChange() {
            if (this.Channel == ChatChannel.World) {
                this.NewWorldMessage = false;
            } else if (this.Channel == ChatChannel.Alliance) {
                this.NewAllianceMessage = false;
            } else if (this.Channel == ChatChannel.State) {
                this.NewStateMessage = false;
            }
        }

        private void OnNewStatusChange(ChatChannel channel) {
            if (this.view.IsVisible) {
                this.view.OnNewStatusChange(channel);
            }
        }

        public void ChatRoomReq() {
            this.worldViewModel.GetChatReq();
            this.ChatAllianceMessagesReq();
            this.stateViewModel.GetChatReq();
        }

        public void ChatAllianceMessagesReq() {
            this.allianceViewModel.GetAllianceChatReq();
        }

        private void WorldChatReq(string input) {
            WorldChatReq worldChat = new WorldChatReq() {
                Content = input
            };
            NetManager.SendMessage(worldChat, typeof(WorldChatAck).Name, this.WorldChatAck);
        }

        private void StateChatReq(string input) {
            StateChatReq stateChat = new StateChatReq() {
                Content = input
            };
            NetManager.SendMessage(stateChat, typeof(StateChatAck).Name, this.StateChatAck);
        }

        private void AllianceChatReq(string input) {
            if (this.allianceId.CustomIsEmpty()) {
                this.allianceId = RoleManager.GetAllianceId();
            }

            if (string.IsNullOrEmpty(this.allianceId)) {
                UIManager.ShowTip(LocalManager.GetValue(LocalHashConst.no_alliance_tip), TipType.Info);
                this.AllianceChatAck(null);
                return;
            }

            AllianceChatReq allianceChat = new AllianceChatReq() {
                Content = input
            };
            NetManager.SendMessage(allianceChat, typeof(AllianceChatAck).Name,
                this.AllianceChatAck);
        }

        private void PrivateChatReq(string input) {
            SendPersonalMessageReq sendMail = new SendPersonalMessageReq() {
                To = this.PrivateMsgUserId,
                Content = input
            };
            Debug.LogError(input);
            NetManager.SendMessage(sendMail,
                typeof(SendPersonalMessageAck).Name,
                (message) => { this.PrivateChatAck(input); });
        }

        private void WorldChatAck(IExtensible message) {
            this.view.ClearInput();
        }

        private void StateChatAck(IExtensible message) {
            this.view.ClearInput();
        }

        private void AllianceChatAck(IExtensible message) {
            this.view.ClearInput();
        }

        public void PrivateChatAck(string message) {
            this.view.ClearInput();
            if (this.detailViewModel.IsVisible) {
                this.detailViewModel.RefreshCurrentPrivateChat(message);
            }
        }

        public void PrivateNewChatAck(PersonalMessage message) {
            message.IsRead = true;
            this.privateViewModel.NewPersonalMessage(message);
        }

        private void NewPersonalMessageCountNtf(IExtensible message) {
            NewPersonalMessageCountNtf newPersonMessageCountNtf =
                message as NewPersonalMessageCountNtf;
            this.NewPrivateMessage = newPersonMessageCountNtf.Count;
        }

        #region FTE

        private void OnChatStep2Start(string index) {
            this.view.afterShowCallback = () => {
                if (!this.allianceViewModel.IsVisible) {
                    this.view.afterHideCallback = FteManager.StopFte;
                    this.view.OnChatStep2Start();
                } else {
                    FteManager.StopFte();
                }
                this.parent.StartChapterDailyGuid();
            };
            this.Show();
        }

        private void OnChatStep2End() {
            this.view.afterHideCallback = null;
            this.view.OnChatStep2End();
        }

        #endregion
    }
}
