using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using UnityEngine;

namespace Poukoute {
    public class MailNormalItemView : BaseItemViewsHolder {

        #region UI element
        [SerializeField]
        private Image imgRedPoint;
        [SerializeField]
        private Image imgAvatar;
        [SerializeField]
        private TextMeshProUGUI txtFrom;
        [SerializeField]
        private TextMeshProUGUI txtAlliance;
        [SerializeField]
        private TextMeshProUGUI txtContent;
        [SerializeField]
        private TextMeshProUGUI txtTime;
        [SerializeField]
        private Button btnMail;
        #endregion

        public UnityEvent OnClick {
            get {
                this.btnMail.onClick.RemoveAllListeners();
                return this.btnMail.onClick;
            }
        }

        private PersonalMessage mail;

        public PersonalMessage Mail {
            get {
                return this.mail;
            }
            set {
                if (this.mail != value) {
                    this.mail = value;
                    this.OnMailChange();
                }
            }
        }

        public bool IsRead {
            get {
                return this.mail.IsRead;
            }
            set {
                this.mail.IsRead = value;
                this.imgRedPoint.gameObject.SetActiveSafe(!value);
            }
        }

        private int index;
        public int Index {
            get {
                return this.index;
            }
            set {
                if (this.index != value) {
                    this.index = value;
                    this.transform.SetSiblingIndex(value);
                }
            }
        }

        private void OnMailChange() {
            this.imgAvatar.sprite = RoleManager.GetRoleAvatarByKey(mail.Avatar);
            this.txtFrom.text = mail.PlayerName;
            this.txtTime.text = GameHelper.DateFormat(mail.Timestamp);
            bool isAllianceMsg = !mail.AllianceName.CustomIsEmpty() &&
                mail.AllianceName.CustomEquals(RoleManager.GetAllianceName());
            this.txtAlliance.gameObject.SetActiveSafe(isAllianceMsg);
            this.imgRedPoint.gameObject.SetActiveSafe(!this.mail.IsRead);
            this.txtContent.text = NPCCityConf.GetMapSNLocalName(this.mail.MapSN);


        }

    }
}
