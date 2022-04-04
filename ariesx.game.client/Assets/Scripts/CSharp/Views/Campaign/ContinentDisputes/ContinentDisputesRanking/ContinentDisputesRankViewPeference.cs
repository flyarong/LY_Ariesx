using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class ContinentDisputesRankViewPeference: BaseViewPreference {
        [Tooltip("PnlContinentDisputesRank.PnlRewardsList")]
        public DevilFightingRewardSampleView firstRankReward;
        public DevilFightingRewardSampleView secondRankReward;
        public DevilFightingRewardSampleView thirdRankReward;

        public CanvasGroup firstRankRewardCanvasGroup;
        public CanvasGroup secondRankRewardCanvasGroup;
        public CanvasGroup thirdRankRewardCanvasGroup;
        public CustomButton pnlBtnShowDetail;
        [Tooltip("PnlContinentDisputesRank.PnlRewardsList.PnlDetail")]
        public CustomButton btnShowDetail;
        [Tooltip("PnlContinentDisputesRank.PnlAllianceRankingList.PnlAllianceRankingContent.PnlOwnAllianceRankList")]
        public Transform pnlOwnAllianceRankList;
        [Tooltip("PnlContinentDisputesRank.PnlAllianceRankingList.PnlAllianceRankingBase")]
        public TextMeshProUGUI txtOwnAllianceRank;
        public TextMeshProUGUI txtOwnAllianceName;
        public TextMeshProUGUI txtstateName; 
        public TextMeshProUGUI txtPoint;
        public Transform imgAllianceBG;
        public Image imgAlliance;



    }
}
