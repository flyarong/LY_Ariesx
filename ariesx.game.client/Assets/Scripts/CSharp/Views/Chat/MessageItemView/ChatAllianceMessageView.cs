using UnityEngine;
using UnityEngine.Events;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class ChatAllianceMessageView : BaseItemViewsHolder {
        #region ui element
        [SerializeField]
        private GameObject pnlChatMsg;
        [SerializeField]
        private GameObject pnlApplyMessage;
        [SerializeField]
        private ChatMessageView chatMessageView;
        [SerializeField]
        private ChatAllianceApplyView applyView;
        #endregion

        private AllianceMessage allianceMessage;
        public AllianceMessage AllianceMessage {
            get {
                return this.allianceMessage;
            }
            set {
                if (this.allianceMessage != value) {
                    this.allianceMessage = value;
                    this.OnAllianceMessageChange();
                }
            }
        }
        
        private string messageId = string.Empty;
        public string MessageId {
            get {
                return this.messageId;
            }
        }

        public UnityAction<string> OnAvatarClick = null;
        public UnityAction<string> OnAcceptClick = null;
        public UnityAction<string> OnRefuseClick = null;

        private bool isAllianceApply = false;

        /***************************/

        public override void MarkForRebuild() {
            if (this.chatMessageView != null) {
                this.chatMessageView.MarkForRebuild();
            }
        }

        public void ResetTimeVisible(long curMsgTime, long preMsgTime) {
            if (!this.isAllianceApply) {
                this.chatMessageView.ResetTimeVisible(curMsgTime, preMsgTime);
            }
        }

        private void OnAllianceMessageChange() {
            this.messageId = this.allianceMessage.Id;
            this.isAllianceApply = (this.allianceMessage.JoinRequest != null &&
                !this.allianceMessage.JoinRequest.Processed);
            this.pnlApplyMessage.gameObject.SetActiveSafe(this.isAllianceApply);
            this.pnlChatMsg.gameObject.SetActiveSafe(!this.isAllianceApply);
            if (this.isAllianceApply) {
                applyView.Request = this.allianceMessage.JoinRequest;
                applyView.OnRefuseClick.AddListener(() => {
                    this.OnRefuseClick(
                        this.allianceMessage.JoinRequest.PlayerId);
                });
                applyView.OnAcceptClick.AddListener(() => {
                    this.OnAcceptClick(
                        this.allianceMessage.JoinRequest.PlayerId);
                });
            } else {
                chatMessageView.AllianceMessage = this.allianceMessage;
                chatMessageView.OnAvatarClick.AddListener(() => {
                    this.OnAvatarClick(this.allianceMessage.Chat.PlayerId);
                });
            }
        }
    }
}