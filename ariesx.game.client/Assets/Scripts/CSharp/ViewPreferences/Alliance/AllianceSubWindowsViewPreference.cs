using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class AllianceSubWindowsViewPreference : BaseViewPreference {
        [Tooltip("UIAllianceWindows.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIAllianceWindows.PnlWindows")]
        public Transform pnlWindows;
        [Tooltip("UIAllianceWindows.PnlClose.BtnClose Button")]
        public Button btnClose;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlTitle")]
        public CanvasGroup windowsHeadCG;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlTitle.text TextMeshProUGUI")]
        public TextMeshProUGUI windowTitle;
    }
}
