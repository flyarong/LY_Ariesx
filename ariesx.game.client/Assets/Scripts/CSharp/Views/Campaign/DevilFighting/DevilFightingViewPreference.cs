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
    public class DevilFightingViewPreference : BaseViewPreference {
        
       [Tooltip("PnlDevilFighting/PnlOnlyTabs")]
        public PanelOnlyTabsView onlyTabsView;
        
       [Tooltip("PnlDevilFighting/PnlDevilFightingDetails")]
        public Transform pnlDevilFightingDetails;
        
       [Tooltip("PnlDevilFighting/PnlDevilFightingDetails")]
        public CanvasGroup devilFightingDetailsCG;
        
       [Tooltip("PnlDevilFighting/PnlDevilFightingRank")]
        public Transform pnlDevilFightingRank;
        
       [Tooltip("PnlDevilFighting/PnlDevilFightingRank")]
        public CanvasGroup devilFightingRankCG;
    }
}
