using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using TMPro;

namespace Poukoute {
    public class PayViewPreference : BaseViewPreference {
        [Tooltip("UIPay.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIPay.PnlPay.PnlHead.BtnClose")]
        public Button btnClose;
        [Tooltip("UIPay.PnlDetail.PnlList")]
        public Transform pnlList;
        public CustomContentSizeFitter contentSizeFitter;
        public CustomVerticalLayoutGroup customVerticalLayoutGroup;
        [Tooltip("UIPay.PnlDetail")]
        public CustomScrollRect customScrollRect;
        [Tooltip("UIPay.PnlLoad")]
        public GameObject pnlLoad;
        [Tooltip("UIPay.PnlTip")]
        public GameObject pnlTip;
    }
}