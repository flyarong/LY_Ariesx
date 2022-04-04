using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

namespace Poukoute {
    public class DisplayBoardViewPreference : BaseViewPreference {
        [Tooltip("UIDisplayBoard.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIDisplayBoard.PnlContent.PnlHead.BtnClose")]
        public Button btnClose;
        [Tooltip("UIDisplayBoard.PnlContent.PnlHead.TxtTitle")]
        public TextMeshProUGUI txtTitle;

        [Tooltip("UIDisplayBoard.PnlContent.PnlDesc.TxtDesc")]
        public TextMeshProUGUI txtDesc;
        public RectTransform rectTransform;
        public CustomContentSizeFitter contentSizeFiltter;
    }
}
