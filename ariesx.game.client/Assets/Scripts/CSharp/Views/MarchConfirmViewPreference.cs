using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class MarchConfirmViewPreference : BaseViewPreference {
        [Tooltip("UITroopInfo.PnlMarchConfirm.PnlTitle.BtnClose")]
        public Button btnClose;

        [Tooltip("UITroopInfo.PnlMarchConfirm.PnlContent.TxtWarning")]
        public TextMeshProUGUI txtWarning;
        [Tooltip("UITroopInfo.PnlMarchConfirm.PnlContent.PnlTip.TxtContent")]
        public TextMeshProUGUI txtContent;
        [Tooltip("UITroopInfo.PnlMarchConfirm.PnlContent.PnlButtons.BtnRecruit")]
        public CustomButton btnRecruit;
        [Tooltip("UITroopInfo.PnlMarchConfirm.PnlContent.PnlButtons.BtnRecruit.PnlContent.Text")]
        public TextMeshProUGUI txtBtnRecruit;
        [Tooltip("UITroopInfo.PnlMarchConfirm.PnlContent.PnlButtons.BtnGo")]
        public CustomButton btnStillGo;
        [Tooltip("UITroopInfo.PnlMarchConfirm.PnlContent.ImgTip")]
        public Image imgTip;
        [Tooltip("UITroopInfo.PnlMarchConfirm.PnlContent.PnlButtons.BtnRecruit")]
        public Animator animBtnEffect;
    }
}
