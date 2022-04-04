using UnityEngine;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class ChatAllianceView : SRIA<ChatAllianceViewModel, ChatAllianceMessageView> {
        private ChatAllianceViewPreference viewPref;
        /*************/
        private bool hasAnyApplyRequest = false;

        protected override void OnUIInit() {
            GameObject viewHoldler = UIManager.GetUI("UIChatRoom.PnlChat.PnlChannel.PnlAllianceHoldler");
            PrefabLoader viewLoadler = viewHoldler.GetComponent<PrefabLoader>();
            this.ui = viewLoadler.LoadSubPrefab();
            //this.ui = UIManager.GetUI("UIChatRoom.PnlChat.PnlChannel.PnlAlliance");
            this.viewModel = this.transform.GetComponent<ChatAllianceViewModel>();
            this.viewPref = this.ui.transform.GetComponent<ChatAllianceViewPreference>();
            this.viewPref.btnFilter.onClick.AddListener(this.OnBtnFilterClick);

            this.SRIAInit(this.viewPref.scrollRect,
                this.viewPref.listVerticalLayoutGroup,
                BaseParams.ContentGravity.END, 93f);
        }

        #region SRIA implementation
        protected override ChatAllianceMessageView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlChatAllianceMessage, this.viewPref.pnlList);
            ChatAllianceMessageView itemView = itemObj.GetComponent<ChatAllianceMessageView>();
            //itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.AllianceMessageList[itemIndex]);

            return itemView;
        }

        protected override void UpdateViewsHolder(ChatAllianceMessageView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.AllianceMessageList[itemView.ItemIndex]);
            itemView.MarkForRebuild();
            this.ScheduleComputeVisibilityTwinPass(true);
        }
        #endregion

        private void OnItemContentChange(ChatAllianceMessageView itemView, AllianceMessage itemData) {
            itemView.AllianceMessage = itemData;
            itemView.OnAvatarClick = this.OnAvatarClick;
            itemView.OnAcceptClick = this.OnAcceptClick;
            itemView.OnRefuseClick = this.OnRefuseClick;
            this.SetChatTimeVisible(itemData, itemView, itemView.ItemIndex);
        }

        public void UpdateApplyView(int dataIndex, bool isDel = false) {
            AllianceMessage itemData = this.viewModel.AllianceMessageList[dataIndex];
            ChatAllianceMessageView allianceMessage;
            int listCount = this._VisibleItems.Count;
            for (int i = 0; i < listCount; i++) {
                allianceMessage = this._VisibleItems[i];
                if (allianceMessage.MessageId == itemData.Id) {
                    if (isDel) {
                        UIManager.HideUI(this._VisibleItems[i].gameObject);
                    } else {
                        allianceMessage.AllianceMessage = itemData;
                        this.SetChatTimeVisible(itemData, allianceMessage, dataIndex);
                    }
                    break;
                }
            }
        }

        private void SetChatTimeVisible(AllianceMessage message,
            ChatAllianceMessageView messageView, int dataIndex) {
            if (dataIndex > 0) {
                messageView.ResetTimeVisible(message.Timestamp,
                    this.viewModel.AllianceMessageList[dataIndex - 1].Timestamp);
            } else {
                messageView.ResetTimeVisible(6 * 60, 0);
            }
        }

        /***************************/
        private void OnBtnFilterClick() {
            this.viewModel.OnlyShowInfo = true;
            this.viewModel.Show();
            this.viewPref.btnFilter.gameObject.SetActiveSafe(false);
        }

        private void OnAvatarClick(string playerId) {
            this.viewModel.ShowPlayerDetailInfo(playerId);
        }

        private void OnAcceptClick(string id) {
            this.viewModel.AcceptApplyReq(id);
        }

        private void OnRefuseClick(string id) {
            this.viewModel.RefuseApplyReq(id);
        }

        private void FormateViewBaseOnChat(int preCount, int currentCount) {
            base.UpdateContentGravity(
                this.ContentVirtualSizeToViewportRatio < 1.0d ?
                BaseParams.ContentGravity.START : BaseParams.ContentGravity.END);
        }

        protected override void OnVisible() {
            this.ItemsRefreshed += this.FormateViewBaseOnChat;
            this.viewPref.btnFilter.gameObject.SetActiveSafe(true);
            if (RoleManager.GetAllianceRole() == AllianceRole.Owner ||
                RoleManager.GetAllianceRole() == AllianceRole.Leader ||
                RoleManager.GetAllianceRole() == AllianceRole.Elder) {
                this.viewPref.btnFilter.gameObject.SetActiveSafe(this.hasAnyApplyRequest);
            } else {
                this.viewPref.btnFilter.gameObject.SetActiveSafe(false);
            }
        }

        protected override void OnInvisible() {
            this.viewModel.OnlyShowInfo = false;
            this.ItemsRefreshed -= this.FormateViewBaseOnChat;
        }
    }
}
