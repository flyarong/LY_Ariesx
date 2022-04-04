using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class ContinentDisputesStatisticsViewPeference: BaseViewPreference {
        [Tooltip("PnlContinentDisputes.PnlContinentDisputesStatistics")]
        public Transform pnlPointsStatisticsList;
        public CustomScrollRect scrollRect;
        public CustomVerticalLayoutGroup verticalLayoutGroup;
    }

}
