using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Poukoute {
    public class HoldGroundKingViewPeference: BaseViewPreference {
        
       [Tooltip("PnlHoldGroundKing/PnlOnlyTabs")]
        public PanelOnlyTabsView onlyTabsView;
        
       [Tooltip("PnlHoldGroundKing/PnlHoldGroundKingDetails")]
        public RectTransform pnlHoldGroundKingDetails;
        
       [Tooltip("PnlHoldGroundKing/PnlHoldGroundKingDetails")]
        public CanvasGroup holdGroundKingDetails;
        
       [Tooltip("PnlHoldGroundKing/PnlHoldGroundKingRank")]
        public RectTransform pnlHoldGroundKingRanking;
        
       [Tooltip("PnlHoldGroundKing/PnlHoldGroundKingRank")]
        public CanvasGroup holdGroundKingRanking;
        
       [Tooltip("PnlHoldGroundKing/PnlHoldGroundKingStatistics")]
        public CanvasGroup holdGroundKingStatistics;
    }
    
}
