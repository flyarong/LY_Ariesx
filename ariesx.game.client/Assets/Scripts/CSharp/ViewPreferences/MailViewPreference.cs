using Protocol;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class MailViewPreference : BaseViewPreference {
        [Tooltip("UIMail.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIMail.PnlMail")]
        public Transform pnlMail;

        [Tooltip("UIMail.PnlMail.PnlTab")]
        public PanelNewTabsView tabView;
        [Tooltip("UIMail.PnlMail.PnlContent.PnlContent.PnlBattle")]
        public Transform pnlBattle;
        [Tooltip("UIMail.PnlMail.PnlContent.PnlContent.PnlSystem")]
        public Transform pnlSystem;
    }
}
