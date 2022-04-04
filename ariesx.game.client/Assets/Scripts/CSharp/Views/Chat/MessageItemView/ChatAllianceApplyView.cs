using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;
using TMPro;

namespace Poukoute {
    public class ChatAllianceApplyView : ListItemView {
        //private GameObject ui;
        #region ui element
        [SerializeField]
        private TextMeshProUGUI txtMessage;
        [SerializeField]
        private TextMeshProUGUI txtContent;
        [SerializeField]
        private Button btnRefuse;
        [SerializeField]
        private Button btnAccept;
        #endregion        

        public UnityEvent OnRefuseClick {
            get {
                this.btnRefuse.onClick.RemoveAllListeners();
                return btnRefuse.onClick;
            }
        }

        public UnityEvent OnAcceptClick {
            get {
                this.btnAccept.onClick.RemoveAllListeners();
                return btnAccept.onClick;
            }
        }

        private AllianceMessageJoinRequest request;
        public AllianceMessageJoinRequest Request {
            get {
                return this.request;
            }
            set {
                if (this.request != value) {
                    this.request = value;
                    this.OnRequestChagne();
                }
            }
        }

        private void OnRequestChagne() {
            this.txtMessage.text =
                string.Format(LocalManager.GetValue(LocalHashConst.tpl_new_member_apply), this.request.PlayerName);
            this.txtContent.text = this.request.Message;
            this.Height = 180;
        }
    }
}
