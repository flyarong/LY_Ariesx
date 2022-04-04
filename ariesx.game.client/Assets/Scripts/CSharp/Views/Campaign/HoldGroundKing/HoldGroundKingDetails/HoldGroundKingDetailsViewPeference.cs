using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    [DisallowMultipleComponent]
    public class HoldGroundKingDetailsViewPeference: BaseViewPreference {
        [Tooltip("PnlHoldGroundKingDetails.TextCampaignTitle")]
        public TextMeshProUGUI txtCampaignTitle;
        [Tooltip("PnlHoldGroundKingDetails.PnlHoldStageRewards")]
        public PanelStageRewardsView stageRewardView;
        [Tooltip("PnlHoldGroundKingDetails.PnlGetIntegralGuideBG.PnlGetIntegralGuideScroll")]
        public CustomScrollRect scrollRect;
        [Tooltip("PnlHoldGroundKingDetails.PnlGetIntegralGuideBG")]
        public CanvasGroup HoldGroundKingGuidLabelCG;
        public CanvasGroup HoldGroundKingGuidContentCG;
        [Tooltip("PnlHoldGroundKingDetails.PnlGetIntegralGuideBG.PnlGuideTypeList")]
        public CustomContentSizeFitter customContentSizeFitter;
        [Tooltip("PnlHoldGroundKingDetails.PnlGetIntegralGuideBG.PnlGetIntegralGuideScroll.PnlGuideTypeList")]
        public Transform pnlGroundIntegralGuide;
        public Transform pnlBridgeGuide;
        public Transform pnlMarginCityGuide;
    }
}
