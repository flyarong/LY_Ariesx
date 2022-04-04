using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using TMPro;

namespace Poukoute {
    [DisallowMultipleComponent]
    public class DevilFightingDetailViewPreference : BaseViewPreference {
        [Tooltip("PnlDevilFighting.PnlDevilFightingDetails.TextCampaignTitle")]
        public TextMeshProUGUI txtCampaignTitle;

        public PanelStageRewardsView stageRewardView;

        [Tooltip("PnlDevilFighting.PnlDevilFightingDetails.PnlContent.PnlTaskScrollRect CustomScrollRect")]
        public CustomScrollRect scrollRect;
        [Tooltip("PnlDevilFighting.PnlDevilFightingDetails.PnlContent.PnlTaskScrollRect.")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup verticalLayoutGroup;
        public RectTransform rectTransform;
        public CustomContentSizeFitter contentSizeFitter;

        public CanvasGroup devilFightingGuidLabelCG;
        public CanvasGroup devilFightingGuidContentCG;

    }
}
