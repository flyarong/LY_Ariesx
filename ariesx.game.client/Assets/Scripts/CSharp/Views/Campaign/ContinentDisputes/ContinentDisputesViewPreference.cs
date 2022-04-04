using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class ContinentDisputesViewPreference: BaseViewPreference {
        [Tooltip("PnlContinentDisputes.PnlOnlyTabs")]
        public PanelOnlyTabsView onlyTabsView;
        [Tooltip("PnlContinentDisputes.PnlContinentDisputesDetails")]
        public RectTransform pnlContinentDisputesDetails;
        public CanvasGroup continentDisputesDetails;
        [Tooltip("PnlContinentDisputes.PnlContinentDisputesRanking")]
        public RectTransform pnlContinentDisputesRanking;
        public CanvasGroup continentDisputesRanking;

        public RectTransform pnlContinentDisputesStatistics;
        public CanvasGroup pnlcontinentDisputesStatisticsCG;
    }
}
