using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;
using TMPro;
using System;

namespace Poukoute {
    public class ChatRoomView : BaseView {
        private ChatRoomViewModel viewModel;
        private ChatRoomViewPreference viewPref;

        public const int channelWorld = 0;
        public const int channelState = 1;
        public const int channelAlliance = 2;
        public const int channelPrivate = 3;
        private Vector2 adaptPanelOriginPos;

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIChatRoom");
            this.viewModel = this.GetComponent<ChatRoomViewModel>();
            this.viewPref = this.ui.GetComponent<ChatRoomViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);

            this.viewPref.tabView.InitTab(4);
            this.viewPref.tabView.SetTab(channelWorld,
                new TabInfo(LocalManager.GetValue(LocalHashConst.chat_world), null,
                (state) => {
                    HideTabPrivateMask();
                    OnToggleWorldClick(state);
                    this.SetTabInfoPointInfo();
                }));
            this.viewPref.tabView.SetTab(channelState,
                new TabInfo(LocalManager.GetValue(LocalHashConst.chat_state), null,
                (state) => {
                    HideTabPrivateMask();
                    OnToggleStateClick(state);
                    this.SetTabInfoPointInfo();
                }));
            this.viewPref.tabView.SetTab(channelAlliance,
                new TabInfo(LocalManager.GetValue(LocalHashConst.chat_alliance), null,
                (state) => {
                    HideTabPrivateMask();
                    OnToggleAllianceClick(state);
                    this.SetTabInfoPointInfo();
                }));
            this.viewPref.tabView.SetTab(channelPrivate,
                new TabInfo(LocalManager.GetValue(LocalHashConst.chat_message), null,
                (state) => {
                    HideTabPrivateMask();
                    OnTogglePrivateClick(state);
                    this.SetTabInfoPointInfo();
                }));

            this.viewPref.tabView.SetCloseCallBack(this.OnBtnCloseClick);
            this.viewPref.ifInput.onEndEdit.AddListener(this.OnEndEdit);
            this.viewPref.btnNewPrivate.onClick.AddListener(this.OnBtnNewPrivateClick);
#if !UNITY_EDITOR
            this.viewPref.ifInput.onSelect.AddListener(this.OnSelect);
#endif
            adaptPanelOriginPos = this.viewPref.ifInputRT.anchoredPosition;
        }


        private void OnSelect(string value) {
            if (Application.platform == RuntimePlatform.Android) {
                float keyboardHeight = AndroidGetKeyboardHeight() * GameConst.RESOULUTION_HEIGHT / Screen.height;
                this.viewPref.ifInputRT.anchoredPosition = Vector3.up * (keyboardHeight);
            } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
                float keyboardHeight = IOSGetKeyboardHeight() * GameConst.RESOULUTION_HEIGHT / Screen.height;
                this.viewPref.ifInputRT.anchoredPosition = Vector3.up * keyboardHeight;
            }
        }

        /// <summary>
        /// 获取安卓平台上键盘的高度
        /// </summary>
        /// <returns></returns>
        private int AndroidGetKeyboardHeight() {
            using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").
                    Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

                using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect")) {
                    View.Call("getWindowVisibleDisplayFrame", Rct);
                    return Screen.height - Rct.Call<int>("height");
                }
            }
        }


        public float IOSGetKeyboardHeight() {
            return TouchScreenKeyboard.area.height;
        }

        public void SetInfo(bool isSubWindow = false) {
            ShowPnlBottom();
            float alpha = isSubWindow ? 120 / 255f : 0;
            this.viewPref.btnBackground.GetComponent<Image>().color = new Color(0, 0, 0, alpha);
            this.viewPref.pnlNewPrivate.gameObject.SetActiveSafe(false);
            this.viewPref.tabView.SetActiveTab(channelState);
            this.SetTabInfoPointInfo();
        }


        public void ClearInput() {
            this.viewPref.ifInput.text = string.Empty;
        }

        public void SetReturn() {
            this.viewPref.ifInput.gameObject.SetActiveSafe(true);
            this.viewPref.pnlNewPrivate.gameObject.SetActiveSafe(false);
            this.viewPref.tabView.SetAllOff();
        }

        public void Return() {
            switch (this.viewModel.PreviouseView.Name) {
                case "ChatPrivateView":
                    this.viewPref.tabView.SetActiveTab(channelPrivate);
                    this.ShowPnlBottom();
                    break;
                default:
                    break;
            }
        }

        public void ShowAllianceChatroom() {
            this.viewModel.ShowMailNew(mailType: MailType.Alliance);
        }

        public void SendMessageTo(string userName, string userId) {
            this.viewModel.ShowMailNew(userName, userId, mailType: MailType.Normal);
        }

        public void OnNewStatusChange(ChatChannel channel) {
            if (channel == ChatChannel.World) {
                this.viewPref.tabView.SetPointVisible(channelWorld, this.viewModel.NewWorldMessage);
            } else if (channel == ChatChannel.State) {
                this.viewPref.tabView.SetPointVisible(channelState, this.viewModel.NewStateMessage);
            } else if (channel == ChatChannel.Alliance) {
                this.viewPref.tabView.SetPointVisible(channelAlliance, this.viewModel.NewAllianceMessage);
            } else if (channel == ChatChannel.Private) {
                this.viewPref.tabView.SetPointVisible(channelPrivate, this.viewModel.NewPrivateMessage);
            }
        }

        private void SetTabInfoPointInfo() {
            this.viewPref.tabView.SetPointVisible(channelWorld, this.viewModel.NewWorldMessage);
            this.viewPref.tabView.SetPointVisible(channelState, this.viewModel.NewStateMessage);
            this.viewPref.tabView.SetPointVisible(channelAlliance, this.viewModel.NewAllianceMessage);
            this.viewPref.tabView.SetPointVisible(channelPrivate, this.viewModel.NewPrivateMessage);
        }

        bool isNewPrivate = false;
        private void OnBtnNewPrivateClick() {
            isNewPrivate = true;
            this.ShowTabPrivateMask();
            this.HidePnlBottom();
            this.viewModel.ShowMailNew();
        }

        private void OnAvatarClick(ChatMessage message) {
            // To do: set and show player info.
            Debug.LogError("OnAvatarClick");
        }

        private void OnEndEdit(string value) {
            this.viewPref.ifInputRT.anchoredPosition = adaptPanelOriginPos;
            this.viewModel.OnBtnSendClick(this.viewPref.ifInput.text);
        }

        private void OnToggleWorldClick(bool state) {
            if (state) {
                this.viewModel.HideAllSubView();
                this.viewPref.tabView.SetPointVisible(channelWorld, false);
            }
            ShowPnlBottom();
            this.viewPref.pnlNewPrivate.gameObject.SetActiveSafe(!state);
            this.viewPref.ifInput.gameObject.SetActiveSafe(state);
            this.viewModel.ShowWorld(state);
        }

        private void OnToggleStateClick(bool state) {
            if (state) {
                this.viewModel.HideAllSubView();
                this.viewPref.tabView.SetPointVisible(channelState, false);
            }
            ShowPnlBottom();
            this.viewPref.pnlNewPrivate.gameObject.SetActiveSafe(!state);
            this.viewPref.ifInput.gameObject.SetActiveSafe(state);
            this.viewModel.ShowState(state);
        }

        private void OnToggleAllianceClick(bool state) {
            if (state) {
                this.viewModel.HideAllSubView();
                this.viewPref.tabView.SetPointVisible(channelAlliance, false);
            }
            ShowPnlBottom();
            this.viewPref.pnlNewPrivate.gameObject.SetActiveSafe(!state);
            this.viewPref.ifInput.gameObject.SetActiveSafe(state);
            this.viewModel.ShowAlliance(state);
        }

        private void OnTogglePrivateClick(bool state) {
            if (state) {
                this.viewModel.HideAllSubView();
                this.viewPref.tabView.SetPointVisible(channelPrivate, false);
            }
            if (isNewPrivate) {
                ShowPnlBottom();
            } else {
                isNewPrivate = false;
            }
            this.viewPref.pnlNewPrivate.gameObject.SetActiveSafe(state);
            this.viewPref.ifInput.gameObject.SetActiveSafe(!state);
            this.viewModel.ShowPrivate(state);
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        public void HidePnlBottom() {
            this.viewPref.pnlBottom.gameObject.SetActiveSafe(false);
        }
        public void ShowPnlBottom() {
            this.viewPref.pnlBottom.gameObject.SetActiveSafe(true);
        }
        #region FTE

        public void OnChatStep2Start() {
            FteManager.SetMask(
                this.viewPref.tabView.GetTabTransform(channelAlliance));
        }

        public void OnChatStep2End() {
            this.viewPref.tabView.SetActiveTab(channelAlliance);
        }

        //bool isShowTabMask = false;
        public void ShowTabPrivateMask() {
            this.viewPref.pnlTabMask.gameObject.SetActiveSafe(true);
        }

        private void HideTabPrivateMask() {
            if (isNewPrivate) {
                isNewPrivate = false;
            } else {
                this.viewPref.pnlTabMask.gameObject.SetActiveSafe(false);
            }

        }

        #endregion
    }
}
