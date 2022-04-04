using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class ChatPrivateView : SRIA<ChatPrivateViewModel, MailNormalItemView> {
        private ChatPrivateViewPreference viewPref;
        /*************/

        protected override void OnUIInit() {
            GameObject viewHoldler = UIManager.GetUI("UIChatRoom.PnlChat.PnlChannel.PnlPrivateHoldler");
            PrefabLoader viewLoadler = viewHoldler.GetComponent<PrefabLoader>();
            this.ui = viewLoadler.LoadSubPrefab();
            //this.ui = UIManager.GetUI("UIChatRoom.PnlChat.PnlChannel.PnlPrivate");
            this.viewModel = this.gameObject.GetComponent<ChatPrivateViewModel>();
            this.viewPref = this.ui.transform.GetComponent<ChatPrivateViewPreference>();
            this.SRIAInit(this.viewPref.scrollRect,
                this.viewPref.listVerticalLayoutGroup, defaultItemSize:120f);
        }


        #region SRIA implementation
        protected override MailNormalItemView CreateViewsHolder(int itemIndex) {
            GameObject itemObj =
                PoolManager.GetObject(PrefabPath.pnlNormalMailItem, this.viewPref.pnlList);
            MailNormalItemView itemView = itemObj.GetComponent<MailNormalItemView>();
            itemView.ItemIndex = itemIndex;
            this.OnItemContentChange(itemView,
                this.viewModel.MessageList[itemIndex]);

            return itemView;
        }

        protected override void UpdateViewsHolder(MailNormalItemView itemView) {
            this.OnItemContentChange(itemView,
                this.viewModel.MessageList[itemView.ItemIndex]);
        }
        #endregion

        private void OnItemContentChange(MailNormalItemView itemView, PersonalMessage itemData) {
            itemView.Mail = itemData;
            itemView.OnClick.AddListener(() => this.OnMailClick(itemData, itemView));
        }

        /***************************/
        private void OnBtnNewClick() {
            this.viewModel.ShowMailNew();
        }

        private void OnMailClick(PersonalMessage mail, MailNormalItemView item) {
            this.viewModel.ShowMail(mail);
            item.IsRead = true;
        }

        public void UpdateMail(PersonalMessage mail) {
            MailNormalItemView mailItemView = this.GetMailItemView(mail.PlayerId);
            this.UpdateViewsHolder(mailItemView);
        }

        private MailNormalItemView GetMailItemView(string playerId) {
            MailNormalItemView itemView = null;
            foreach (Transform item in this.viewPref.pnlList) {
                itemView = item.GetComponent<MailNormalItemView>();
                if (itemView.Mail.PlayerId.CustomEquals(playerId)) {
                    return itemView;
                }
            }
            return null;
        }
    }
}
