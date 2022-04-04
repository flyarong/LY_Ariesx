using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute { 
public class AllianceDisplayBoardViewPeference : BaseViewPreference {
        [Tooltip("UIAllianceDisplayBoard.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIAllianceDisplayBoard.PnlContent.PnlTitle.BtnClose")]
        public Button btnClose;
        public TextMeshProUGUI txtTitle;
        [Tooltip("UIAllianceDisplayBoard.PnlContent.PnlDesc.PnlDescList")]
        public Transform PnlDescList;
        [Tooltip("UIAllianceDisplayBoard.PnlContent.PnlDesc.PnlDescList")]
        public VerticalLayoutGroup pnlDescListLayoutGroup;
        public CustomContentSizeFitter customContentSizeFitter;

    }
}
