using UnityEngine;
using UnityEngine.Events;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class AllianceApplyViewPreference : BaseViewPreference {

        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlApply.PnlApply.PnlContent")]
        public TMP_InputField ifDescription;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlApply.PnlApply.PnlContent.TxtContentLimit")]
        public TextMeshProUGUI txtContentLimit;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlApply.PnlApply.PnlConfirm.BtnSend")]
        public CustomButton btnSend;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlApply.PnlApply.PnlConfirm.BtnCancel")]
        public CustomButton btnCancel;
    }
}
