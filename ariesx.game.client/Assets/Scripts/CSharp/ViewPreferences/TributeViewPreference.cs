using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class TributeViewPreference : BaseViewPreference {
        [Tooltip("UITribute.BtnBackground")]
        public Button btnBackground;

        [Tooltip("UITribute.PnlTribute")]
        public Transform pnlTribute;

        [Tooltip("UITribute.PnlDescription.ImgStage")]
        public Image imgStage;
        [Tooltip("UITribute.PnlDescription.ImgStage.TxtStage")]
        public TextMeshProUGUI txtStage;
        [Tooltip("UITribute.PnlTribute.BtnReceive")]
        public Button btnReceive;

        [Tooltip("UITribute.PnlTribute.PnlReward")]
        public Transform pnlReward;
        public LayoutGroup lgReward;

        [Tooltip("UITribute.PnlTribute.PnlBackground.ImgHalo")]
        public Image imgHalo;
    }
}
