using System;
using UnityEngine.UI;
using Protocol;
using UnityEngine;
using TMPro;

namespace Poukoute {
    public class AllianceSlaveInfoMessageView : ListItemView {
        //private GameObject ui;
        public Image imgBackground;
        public Transform pnlContent;
        public TextMeshProUGUI txtMessage;
        public Transform pnlTime;
        public TextMeshProUGUI txtTimeStamp;

        private AllianceMessageInfo info;
        public AllianceMessageInfo Info {
            set {
                if (this.info != value) {
                    this.info = value;
                    this.OnInfoChange();
                }
            }
        }

        public long MessageTimestamp {
            get; set;
        }

        public long PreMessageTimestamp {
            get; set;
        }

        private void OnInfoChange() {
            string[] infoArgs = this.info.Args.ToArray();
            if (LocalManager.GetValue(this.info.Template).CustomIsEmpty() ||
                infoArgs.Length < 1
                ) {
                Debug.LogUpload("OnInfoChange " + this.info.Template + " " + infoArgs.Length);
                return;
            }
            this.txtMessage.text = string.Format(
                    LocalManager.GetValue(this.info.Template), infoArgs);
            bool showTime = ((this.MessageTimestamp - this.PreMessageTimestamp) > GameConst.MESSAGE_TIME_SHOW_OFFSET);
            this.pnlTime.gameObject.SetActiveSafe(showTime);
            if (showTime) {
                this.txtTimeStamp.text = GameHelper.HistoryTimeFormat(this.MessageTimestamp);
            }
            this.CaculateSize();
        }

        private void CaculateSize() {
            this.pnlContent.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputHorizontal();
            this.pnlContent.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            this.pnlContent.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            this.transform.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputHorizontal();
            this.transform.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            this.transform.GetComponent<CustomContentSizeFitter>().SetLayoutVertical();
            this.Height = this.transform.GetComponent<RectTransform>().rect.height;
        }
    }
}