using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute
{
    public class DemonShadowViewPeference : BaseViewPreference
    {
        [Tooltip("PnlHoldGroundKing.PnlOnlyTabs")]
        public PanelOnlyTabsView onlyTabsView;
        [Tooltip("PnlHoldGroundKing.PnlDemonShadowDetails")]
        public CanvasGroup pnlDemonShadowDetails;
        [Tooltip("PnlHoldGroundKing.PnlDemonShadowHistory")]
        public CanvasGroup pnlDemonShadowHistory;
    }
}