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
    public class DevilFightingRankViewPreference : BaseViewPreference {
        
       [Tooltip("PnlDevilFightingRank/PnlRewardsList/PnlBtnShowDetail")]
        public Button btnRankRewardsDetail;
        
       [Tooltip("PnlDevilFightingRank/PnlRewardsList/PnlFirstRewardSample")]
        public DevilFightingRewardSampleView firstRankReward;
        
       [Tooltip("PnlDevilFightingRank/PnlRewardsList/PnlSecondRewardSample")]
        public DevilFightingRewardSampleView secondRankReward;
        
       [Tooltip("PnlDevilFightingRank/PnlRewardsList/PnlThirdRewardSample")]
        public DevilFightingRewardSampleView thirdRankReward;
        
       [Tooltip("PnlDevilFightingRank/PnlRewardsList/PnlFirstRewardSample")]
        public CanvasGroup firstRankRewardCG;
        
       [Tooltip("PnlDevilFightingRank/PnlRewardsList/PnlSecondRewardSample")]
        public CanvasGroup secondRankRewardCG;
        
       [Tooltip("PnlDevilFightingRank/PnlRewardsList/PnlThirdRewardSample")]
        public CanvasGroup thirdRankRewardCG;
        
       [Tooltip("PnlDevilFightingRank/PnlDetail/BtnShowDetail")]
        public CustomButton btnShowMoreDetail;
        
       [Tooltip("PnlDevilFightingRank/PnlRankList/PnlContent")]
        public CustomScrollRect scrollRect;
        
       [Tooltip("PnlDevilFightingRank/PnlRankList/PnlContent/PnlList")]
        public Transform pnlList;
        
       [Tooltip("PnlDevilFightingRank/PnlRankList/PnlContent/PnlList")]
        public CustomVerticalLayoutGroup verticalLayoutGroup;
        
       [Tooltip("PnlDevilFightingRank/PnlRankList/PnlContent/PnlList")]
        public RectTransform rectTransform;
        
       [Tooltip("PnlDevilFightingRank/PnlRankList/PnlContent/PnlList")]
        public CustomContentSizeFitter contentSizeFitter;
        
       [Tooltip("PnlDevilFightingRank/PnlRankList/PnlOwnPlayerRanking/TxtOwnRank")]
        public TextMeshProUGUI txtOwnRank;
        
       [Tooltip("PnlDevilFightingRank/PnlRankList/PnlOwnPlayerRanking/TxtOwnName")]
        public TextMeshProUGUI txtOwnName;
        
       [Tooltip("PnlDevilFightingRank/PnlRankList/PnlOwnPlayerRanking/TxtOwnPoint")]
        public TextMeshProUGUI txtPoint;
    }
}
