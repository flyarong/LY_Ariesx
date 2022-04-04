using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class MailSystemItemView : BaseItemViewsHolder {
        #region ui element
        //private GameObject ui;
        [SerializeField]
        private Button btnItemInfo;
        [SerializeField]
        private GameObject mailNewMark;
        [SerializeField]
        private TextMeshProUGUI txtFrom;
        [SerializeField]
        private TextMeshProUGUI txtSubject;
        [SerializeField]
        private TextMeshProUGUI txtTime;
        [SerializeField]
        private TextMeshProUGUI txtContent;
        //[SerializeField]
        //private TextMeshProUGUIHref txtMeshProUGUIHref;
        [SerializeField]
        private GameObject mailAttachment;
        [SerializeField]
        private GameObject mailRead;
        #endregion

        public UnityEvent onClickHref = new UnityEvent();

        public UnityEvent OnInfoClick {
            get {
                this.btnItemInfo.onClick.RemoveAllListeners();
                return this.btnItemInfo.onClick;
            }
        }

        //public void MarkMaukIsReadClock() {
        //    this.MarkMailIsRead();
        //}

        private SystemMessage mail;

        //public bool IsRead {
        //    get {
        //        return this.mail.IsRead;
        //    }
        //    set {
        //        this.mail.IsRead = value;
        //        this.MarkMailIsRead();
        //    }
        //}

        public void SetItemContent(SystemMessage mail, string content) {
            this.mail = mail;
            this.OnMailChange(content);
        }

        private void OnMailChange(string mailContent) {
            //this.btnItemInfo.onClick.RemoveAllListeners();
            this.btnItemInfo.onClick.AddListener(this.OnBtnMarkReadClick);
            this.txtFrom.text = LocalManager.GetValue(LocalHashConst.system_mail);
            //Debug.LogError(this.mail.Subject);
            this.txtSubject.text = this.mail.IsTemplate ?
                LocalManager.GetValue(this.mail.Subject) : this.mail.Subject;
            this.txtTime.text = GameHelper.DateFormat(this.mail.Timestamp);
            // To do 
            //this.txtContent.StripLengthWithSuffix(mailContent);
            string[] splitArray = mailContent.CustomSplit('\n');
            this.txtContent.text = splitArray[0];
            bool isNewMail = this.mail.IsRead;
            this.mailNewMark.SetActiveSafe(!isNewMail);
            bool hasAttachment = this.mail.Attachment != null;
            this.mailAttachment.SetActiveSafe(hasAttachment);
            if (this.mail.IsCollect) {
                this.mailAttachment.SetActiveSafe(false);
            }
            this.mailRead.SetActiveSafe(isNewMail);
        }

        private void OnHrefClick(string template, string content) {
            Debug.LogError(template);
            Application.OpenURL(content);
        }

        private void OnHrefClick(string type, Vector2 coordinate) {
            this.onClickHref.InvokeSafe();
            TriggerManager.Invoke(Trigger.CameraMove, coordinate);
            this.OnBtnMarkReadClick();
        }

        private void OnBtnMarkReadClick() {
            this.mailNewMark.SetActiveSafe(false);
            this.mailRead.SetActiveSafe(true);
            this.mail.IsRead = true;
        }

        //private void MarkMailIsRead() {
        //    //MarkSystemMessageReadReq markReadReq = new MarkSystemMessageReadReq();
        //    //markReadReq.Id.Add(this.mail.Id);
        //    //NetManager.SendMessage(markReadReq, string.Empty, null);
        //    this.mailNewMark.SetActiveSafe(false);
        //    this.mailRead.SetActiveSafe(true);
        //    this.mail.IsRead = true;
        //}
    }
}
