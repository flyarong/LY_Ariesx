using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute
{
    public class DemonShadowHistoryViewPeference : BaseViewPreference {
        [Tooltip("PnlContinentDisputes.PnlContinentDisputesStatistics")]
        public Transform pnlPointsStatisticsList;
        public CustomScrollRect scrollRect;
        public CustomVerticalLayoutGroup verticalLayoutGroup;
    }
}