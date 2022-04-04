using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class HoldGroundKingStatisticsViewPeference: BaseViewPreference {
        [Tooltip("PnlHoldGroundKingStatistics.PnlScrollContent")]
        public Transform pnlPointsStatisticsList;
        public CustomScrollRect scrollRect;
        public CustomVerticalLayoutGroup verticalLayoutGroup;
    }

}
