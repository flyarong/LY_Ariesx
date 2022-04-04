using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class HeroSelectViewPreference : BaseViewPreference {
        [Tooltip("UITroop.PnlFormation.PnlContent.ScrollView.PnlContent.PnlSelect RectTransform")]
        public RectTransform rectTransform;
        public VerticalLayoutGroup verticalLayoutGroup;
        public ContentSizeFitter contentSizeFitter;
        [Tooltip("UITroop.PnlFormation.PnlContent.ScrollView.PnlContent.PnlSelect Image")]
        public Image imgHighlight;
        [Tooltip("UITroop.PnlFormation.PnlContent.ScrollView.PnlContent.PnlSelect.PnlAvatar")]
        public Transform pnlAvatar;
        [Tooltip("UITroop.PnlFormation.PnlContent.ScrollView.PnlContent.PnlSelect.PnlAvatar.PnlDrag CustomDrag")]
        public CustomDrag drag;
        [Tooltip("UITroop.PnlFormation.PnlContent.ScrollView.PnlContent.PnlSelect.PnlAvatar.PnlDrag CustomClick")]
        public CustomClick click;

        [Tooltip("UITroop.PnlFormation.PnlContent.ScrollView.PnlContent.PnlSelect.PnlButtons")]
        public Transform pnlButtons;
        public RectTransform buttonsRectTransform;
        [Tooltip("UITroop.PnlFormation.PnlContent.ScrollView.PnlContent.PnlSelect canvasGroup")]
        public CanvasGroup canvasGroup;
    }
}
