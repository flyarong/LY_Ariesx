using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class SubordinateStatusView : BaseView {

        /* UI Members*/
        private SubordinateStatusViewModel viewModel;
        private AllianceSubordinateStatusViewPreference viewPref;
        /*************/
        

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<SubordinateStatusViewModel>();
            //this.InitUi();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIAllianceWindows.PnlWindows.PnlSubordinateStatus");
            this.viewPref = this.ui.transform.GetComponent<AllianceSubordinateStatusViewPreference>();
        }

        public void SetList() {
            GameHelper.ClearChildren(this.viewPref.pnlList);
            int messageCount = this.viewModel.MessageList.Count;
            long preTimestamp = 0;
            for (int i = 0; i < messageCount; i++) {
                if (i > 0) {
                    preTimestamp = this.viewModel.MessageList[i - 1].Timestamp;
                }
                this.NewMessage(this.viewModel.MessageList[i], preTimestamp);
            }
            this.FormatList();
        }


        private void NewMessage(AllianceMessage message, long preTimeStamp) {
            GameObject messageObj = null;
            if (message.SlaveInfo != null) {
                messageObj = PoolManager.GetObject(PrefabPath.pnlSlaveInfoMessage, this.viewPref.pnlList);
                AllianceSlaveInfoMessageView listItemView = messageObj.GetComponent<AllianceSlaveInfoMessageView>();
                listItemView.MessageTimestamp = message.Timestamp;
                listItemView.PreMessageTimestamp = preTimeStamp;
                listItemView.Info = message.SlaveInfo;
            }
        }

        public void FormatList() {
            this.viewPref.listContentSizeFitter.onSetLayoutVertical.AddListener(() => {
                this.viewPref.listRectTransform.anchoredPosition = new Vector2(
                    0,
                    -this.viewPref.listRectTransform.rect.height / 2
                );
                this.viewPref.listVerticalLayoutGroup.SetOriginal();
                this.viewPref.scrollRect.velocity = Vector2.zero;
            });
        }
    }
}
