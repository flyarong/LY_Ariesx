using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Poukoute {
    public class ContinentDisputesDetailsViewPeference: BaseViewPreference {
        [Tooltip("PnlContinentDisputesDetails.TextCampaignTitle")]
        public TextMeshProUGUI txtCampaignTitle;

        [Tooltip("PnlContinentDisputesDetails.PnlPassPointsStageRewards")]
        public PanelStageRewardsView panelStageRewardsView;
        //public CampaignRewardsView panelStageRewardsView;

        [Tooltip("PnlContinentDisputesDetails.PnlCampaignGuideLabel")]
        public CanvasGroup ContinentDisputesGuidLabelCG;
        [Tooltip("PnlContinentDisputesDetails.PnlGetIntegralGuideBG")]
        public CanvasGroup ContinentDisputesGuidContentCG;
        [Tooltip("PnlGetIntegralGuideBG.PnlGetIntegralGuideScroll.PnlList.PnlLandPointsList")]
        public Transform pnlLandPointsList;
        [Tooltip("PnlGetIntegralGuideBG.PnlGetIntegralGuideScroll.PnlList.PnlPassPointsList")]
        public Transform pnlPassPointsList;
        
    }
}
