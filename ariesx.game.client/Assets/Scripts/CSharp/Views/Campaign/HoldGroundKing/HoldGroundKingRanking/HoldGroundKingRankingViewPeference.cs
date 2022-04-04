using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Poukoute {
    public class HoldGroundKingRankingViewPeference: BaseViewPreference {
        
       [Tooltip("PnlHoldGroundKingRank/PnlRewardsList/PnlFirstRewardSample")]
        public DevilFightingRewardSampleView firstRankReward;
        
       [Tooltip("PnlHoldGroundKingRank/PnlRewardsList/PnlSecondRewardSample")]
        public DevilFightingRewardSampleView secondRankReward;
        
       [Tooltip("PnlHoldGroundKingRank/PnlRewardsList/PnlThirdRewardSample")]
        public DevilFightingRewardSampleView thirdRankReward;
        
       [Tooltip("PnlHoldGroundKingRank/PnlRewardsList/PnlFirstRewardSample")]
        public CanvasGroup firstRankRewardCanvasGroup;
        
       [Tooltip("PnlHoldGroundKingRank/PnlRewardsList/PnlSecondRewardSample")]
        public CanvasGroup secondRankRewardCanvasGroup;
        
       [Tooltip("PnlHoldGroundKingRank/PnlRewardsList/PnlThirdRewardSample")]
        public CanvasGroup thirdRankRewardCanvasGroup;
        
       [Tooltip("PnlHoldGroundKingRank/PnlRewardsList/PnlBtnShowDetail")]
        public CustomButton pnlBtnShowDetail;
        
       [Tooltip("PnlHoldGroundKingRank/PnlDetail/BtnShowDetail")]
        public CustomButton btnShowDetail;
        
       [Tooltip("PnlHoldGroundKingRank/PnlRankList/PnlContent/PnlOwnRankList")]
        public Transform pnlOwnRankList;
        
       [Tooltip("PnlHoldGroundKingRank/PnlRankList/PnlOwnPlayerRanking/TxtOwnRank")]
        public TextMeshProUGUI txtOwnRank;
        
       [Tooltip("PnlHoldGroundKingRank/PnlRankList/PnlOwnPlayerRanking/TxtOwnName")]
        public TextMeshProUGUI txtOwnName;
        
       [Tooltip("PnlHoldGroundKingRank/PnlRankList/PnlOwnPlayerRanking/TxtOwnPoint")]
        public TextMeshProUGUI txtPoint;
    }
}
