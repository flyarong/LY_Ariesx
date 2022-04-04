using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class BannaerViewPreference : BaseViewPreference {
        // Banner
        [Tooltip("UIBanner.PnlBanner")]
        public Transform pnlBanner;
        [Tooltip("UIBanner.PnlBanner.PnlContent")]
        public Transform pnlBannerContent;
        [Tooltip("UIBanner.PnlBanner.PnlContent.PnlChapter.Text")]
        public TextMeshProUGUI txtBanner;
        [Tooltip("UIBanner.PnlBanner.PnlContent.PnlTitle.Text")]
        public TextMeshProUGUI txtBannerSub;
        public Image imgLeft;
        public Image imgRight;
        public AnimationCurve showCurve;
        public AnimationCurve hideCurve;
    }
}
