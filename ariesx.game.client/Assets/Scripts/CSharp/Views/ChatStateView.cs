using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class ChatStateView : SRIA<ChatStateViewModel, ChatMessageView> {
        private ChatStateViewPeference viewPref;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<ChatStateViewModel>();
        }

        /* UI Members*/

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIChatRoom.PnlChat.PnlChannel.PnlState");
            this.viewModel = this.gameObject.GetComponent<ChatStateViewModel>();
            this.viewPref = this.ui.transform.GetComponent<ChatStateViewPeference>();
            this.SRIAInit(this.viewPref.scrollRect, this.viewPref.listVerticalLayoutGroup,
                BaseParams.ContentGravity.END, 93f);
        }

        /*************/

        #region SRIA implementation
        protected override ChatMessageView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlChatMessage, this.viewPref.pnlList);
            ChatMessageView itemView = itemObj.GetComponent<ChatMessageView>();
            //itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.MessageList[itemIndex]);

            return itemView;
        }

        protected override void UpdateViewsHolder(ChatMessageView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.MessageList[itemView.ItemIndex]);
            itemView.MarkForRebuild();
            this.ScheduleComputeVisibilityTwinPass(true);
        }
        #endregion

        private void OnItemContentChange(ChatMessageView itemView, ChatMessage itemData) {
            itemView.WorldChatMessage = itemData;
            string id = itemData.PlayerId;
            itemView.OnAvatarClick.AddListener(() => {
                this.OnAvatarClick(id);
            });
            this.SetChatTimeVisible(itemData, itemView);
        }

        private void SetChatTimeVisible(ChatMessage message,
            ChatMessageView messageView) {
            if (messageView.ItemIndex > 0) {
                messageView.ResetTimeVisible(message.Timestamp,
                    this.viewModel.MessageList[messageView.ItemIndex - 1].Timestamp);
            } else {
                messageView.ResetTimeVisible(6 * 60, 0);
            }
        }

        private void OnAvatarClick(string playerId) {
            this.viewModel.ShowPlayerDetailInfo(playerId);
        }

        private void FormateViewBaseOnChat(int preCount, int currentCount) {
            base.UpdateContentGravity(
                this.ContentVirtualSizeToViewportRatio < 1.0d ?
                BaseParams.ContentGravity.START : BaseParams.ContentGravity.END);
        }

        protected override void OnVisible() {
            this.ItemsRefreshed += this.FormateViewBaseOnChat;
        }

        protected override void OnInvisible() {
            this.ItemsRefreshed -= this.FormateViewBaseOnChat;
        }
    }
}
