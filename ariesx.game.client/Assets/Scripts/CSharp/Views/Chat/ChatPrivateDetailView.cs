using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class ChatPrivateDetailView : BaseView {
        private ChatPrivateDetailViewModel viewModel;
        private ChatPrivateDetailViewPreference viewPref;
        /*************/

        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<ChatPrivateDetailViewModel>();
            GameObject viewHoldler = UIManager.GetUI("UIChatRoom.PnlChat.PnlChannel.PnlPrivateDetailHoldler");
            PrefabLoader viewLoadler = viewHoldler.GetComponent<PrefabLoader>();
            this.ui = viewLoadler.LoadSubPrefab();
            //this.ui = UIManager.GetUI("UIChatRoom.PnlChat.PnlChannel.PnlPrivateDetail");
            /* Cache the ui components here */
            this.viewPref = this.ui.transform.GetComponent<ChatPrivateDetailViewPreference>();
            this.viewPref.btnBack.onClick.AddListener(this.OnBtnBackClick);
            this.viewPref.scrollRect.onValueChanged.AddListener(this.OnScrollRectChange);
        }

        /* Propert change function */
        public void OnMailChange(bool isNeedFresh) {
            this.viewPref.txtName.text = this.viewModel.Mail.PlayerName;
            if (isNeedFresh) {
                GameHelper.ClearChildren(this.viewPref.pnlList);
                this.ResortPrivateMail();
                int listCount = this.viewModel.Mail.Messages.Count;
                for (int index = 0; index < listCount; index++) {
                    this.NewMessage(this.viewModel.Mail.Messages[index]);
                }
            } else {
                this.InsetLastedMessage();
            }
            this.Format();
        }

        /***************************/
        public void Format() {
            this.StartCoroutine(this.FormatCoroutine());
        }

        private void InsetLastedMessage() {
            this.ResortPrivateMail();
            this.NewMessage(this.viewModel.Mail.Messages[0]);
        }

        private void ResortPrivateMail() {
            this.viewModel.Mail.Messages.Sort((a, b) => {
                return a.Timestamp.CompareTo(b.Timestamp);
            });
        }

        private void NewMessage(PersonalMessage.Message message) {
            GameObject pnlNormalMessage =
                 PoolManager.GetObject(PrefabPath.pnlChatMessage, this.viewPref.pnlList);
            ChatMessageView normalMessageView =
                pnlNormalMessage.GetComponent<ChatMessageView>();
            normalMessageView.PersonalMessageAvater = this.viewModel.Mail.Avatar;
            normalMessageView.PersonalMessage = message;
        }

        private IEnumerator FormatCoroutine() {
            yield return YieldManager.EndOfFrame;
            this.viewPref.contentSizeFitter.onSetLayoutVertical.AddListener(() => {
                this.viewPref.rectTransform.anchoredPosition = new Vector2(
                    this.viewPref.rectTransform.rect.width / 2,
                    this.viewPref.rectTransform.rect.height / 2
                );
                this.viewPref.verticalLayoutGroup.SetOriginal();
                this.viewPref.scrollRect.velocity = Vector2.zero;
            });
        }

        private void OnBtnBackClick() {
            this.viewModel.Return();
        }

        private void OnScrollRectChange(Vector2 value) {

        }

        private void OnInputFieldChange(string value) {
            this.viewModel.Message = value;
        }
    }
}
