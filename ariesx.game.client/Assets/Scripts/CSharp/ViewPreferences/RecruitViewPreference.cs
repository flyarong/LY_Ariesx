using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class RecruitViewPreference : BaseViewPreference {
        [Tooltip("UIRecruit.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIRecruit.PnlRecruit.PnlContent")]
        public RectTransform srollRectTransform;
        [Tooltip("UIRecruit.PnlRecruit.PnlContent.ScrollView")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIRecruit.PnlRecruit.PnlContent.ScrollView.PnlList")]
        public Transform pnlList;
        [Tooltip("UIRecruit.PnlRecruit.PnlContent.ScrollView.PnlContent")]
        public CustomVerticalLayoutGroup verticalLayoutGroup;
        public CustomContentSizeFitter contentSizeFitter;
        public RectTransform rectTransform;
        [Tooltip("UIRecruit.PnlRecruit.PnlTips")]
        public Transform pnlTips;
        [Tooltip("UIRecruit.PnlRecruit.PnlTitle.BtnClose")]
        public Button btnClose;
        [Tooltip("UIRecruit.PnlRecruit.PnlTitle")]
        public TextMeshProUGUI txtTitle;
        [Tooltip("UIRecruit.PnlRecruit.PnlTreatmentResource.PnlResource")]
        public Transform pnlResource;
        [Tooltip("UIRecruit.PnlRecruit.BtnTreatmentAll")]
        public CustomButton btnTreatmentAll;
    }
}
