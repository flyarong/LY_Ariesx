using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Poukoute {
    public class ChatNewPrivateViewPreference : BaseViewPreference {
        [Tooltip("UIChatRoom.PnlMessageNew.BtnBackground")]
        public Button btnBackground;

        [Tooltip("UIChatRoom.PnlMessageNew.PnlTitle.BtnClose")]
        public Button btnClose;

        [Tooltip("UIChatRoom.PnlMessageNew.PnlDetail.PnlTo.IfContent")]
        public TMP_InputField ifTo;
        [Tooltip("UIChatRoom.PnlMessageNew.PnlDetail.PnlContent.IfContent")]
        public TMP_InputField ifContent;
        [Tooltip("UIChatRoom.PnlMessageNew.PnlDetail.PnlContent.IfContent.Placeholder")]
        public TextMeshProUGUI ifContentPlaceholder;
        [Tooltip("UIChatRoom.PnlMessageNew.PnlDetail.PnlButton.BtnSend")]
        public CustomButton btnSend;
    }
}
