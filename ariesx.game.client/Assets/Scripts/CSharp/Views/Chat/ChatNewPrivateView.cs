using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Poukoute {
    public class ChatNewPriavteView : BaseView {
        private ChatNewPriavteViewModel viewModel;
        private ChatNewPrivateViewPreference viewPref;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<ChatNewPriavteViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIChatRoom.PnlMessageNew");
            this.viewPref = this.ui.transform.GetComponent<ChatNewPrivateViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            //this.viewPref.ifTo.characterLimit = LocalConst.GetLimit(LocalConst.PLAYER_NAME);
            this.viewPref.ifTo.onValueChanged.AddListener(this.OnIfToChange);
            this.viewPref.ifContent.characterLimit = LocalConst.GetLimit(LocalConst.MAIL);
            this.viewPref.ifContent.onValueChanged.AddListener(this.OnContentChange);
            this.viewPref.ifContentPlaceholder.text = 
                string.Concat("0/", LocalConst.GetLimit(LocalConst.MAIL));
            this.viewPref.btnSend.onClick.AddListener(this.OnBtnSendClick);
        }

        public void SetInfo() {
            this.viewPref.btnSend.interactable = false;
            this.ResetContent();
            switch (this.viewModel.MailType) {
                case MailType.Alliance:
                    this.viewPref.ifTo.text = LocalManager.GetValue(LocalHashConst.alliance_member);
                    this.viewPref.ifTo.interactable = false;
                    break;
                case MailType.Normal:
                    this.viewPref.ifTo.text = this.viewModel.PrivateMsgTo;
                    this.viewPref.ifTo.interactable = true;
                    break;
                default:
                    Debug.LogError("Should not come here");
                    break;
            }
        }

        private void ResetContent() {
            this.viewPref.ifTo.text = string.Empty;
            this.viewPref.ifContent.text = string.Empty;
        }

        private bool isToAvaliable = false;
        private bool isContentAvaliable = false;

        /* Propert change function */

        /***************************/
        protected void OnBtnCloseClick() {
            this.viewModel.Return();
        }

        void OnIfToChange(string value) {
            this.isToAvaliable = !string.IsNullOrEmpty(value);
            this.viewPref.btnSend.interactable = this.isContentAvaliable && this.isToAvaliable;
        }

        void OnContentChange(string value) {
            this.isContentAvaliable = !string.IsNullOrEmpty(value);
            this.viewPref.btnSend.interactable = this.isContentAvaliable && this.isToAvaliable;
            this.viewPref.ifContentPlaceholder.text = 
                string.Concat(value.Length, "/", LocalConst.GetLimit(LocalConst.MAIL));
        }

        void OnBtnSendClick() {
            this.viewModel.PrivateMsgTo = this.viewPref.ifTo.text;
            this.viewModel.PrivateMsgContent = this.viewPref.ifContent.text;
            this.viewModel.SendMail();
        }
    }
}
