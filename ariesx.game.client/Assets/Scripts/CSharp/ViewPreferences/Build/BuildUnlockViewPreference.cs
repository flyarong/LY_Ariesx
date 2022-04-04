using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using TMPro;

namespace Poukoute {
    public class BuildUnlockViewPreference : BaseViewPreference {
        [Tooltip("UIBuildUnlock.PnlContent")]
        public Transform pnlUnlockContent;
        [Tooltip("UIBuildUnlock.PnlContent.PnlTitle.BtnClose")]
        public Button btnUnlockClose;
        [Tooltip("UIBuildUnlock.PnlContent.BtnUnlock")]
        public Button btnUnlock;
        [Tooltip("UIBuildUnlock.PnlUnlock.BtnUnlock.PnlContent.TxtPrice")]
        public TextMeshProUGUI txtPrice;
        [Tooltip("UIBuildUnlock.BtnBackground")]
        public CustomButton btnBackClose;
        [Tooltip("UIBuildUnlock.PnlContent.pnlContentBG.TxtContent")]
        public TextMeshProUGUI txtContent;
        [Tooltip("UIBuildUnlock.PnlContent.BtnBuyMonthly.")]
        public Button btnBuyMonthly;
    }
}
