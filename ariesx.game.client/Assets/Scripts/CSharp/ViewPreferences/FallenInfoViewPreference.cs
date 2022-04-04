using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class FallenInfoViewPreference : BaseViewPreference {
        [Tooltip("UIFallenInfo.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIFallenInfo.PnlFallen")]
        public Transform pnlFallen;
        [Tooltip("UIFallenInfo.PnlFallen.PnlHead.TxtTitle")]
        public TextMeshProUGUI txtHeadTitle;
        [Tooltip("UIFallenInfo.PnlFallen.PnlHead.BtnClose")]
        public Button btnClose;
        [Tooltip("UIFallenInfo.PnlFallen.BtnPay")]
        public CustomButton btnPayResource;

        [Tooltip("UIFallenInfo.PnlPayResource")]
        public Transform pnlPayResource;
        [Tooltip("UIFallenInfo.PnlPayResource.PnlTop.TxtTitle")]
        public TextMeshProUGUI txtTopTitle;
        [Tooltip("UIFallenInfo.PnlPayResource.PnlTop.BtnReturn")]
        public Button btnReturn;
        [Tooltip("UIFallenInfo.PnlPayResource.PnlTop.BtnClose")]
        public Button btnPayClose;
        [Tooltip("UIFallenInfo.PnlPayResource.PnlProgress")]
        public Slider sliProgress;
        [Tooltip("UIFallenInfo.PnlPayResource.PnlProgress.PnlAlreadyPaid")]
        public Slider paidProgress;
        [Tooltip("UIFallenInfo.PnlPayResource.PnlProgress.TxtTip")]
        public TextMeshProUGUI txtTipText;
        [Tooltip("UIFallenInfo.PnlPayResource.PnlProgress.TxtProgress")]
        public TextMeshProUGUI txtProgress;
        [Tooltip("UIFallenInfo.PnlPayResource.PnlResources")]
        public Transform pnlResources;
        [Tooltip("UIFallenInfo.PnlPayResource.BtnPay")]
        public CustomButton btnPay;
    }
}
