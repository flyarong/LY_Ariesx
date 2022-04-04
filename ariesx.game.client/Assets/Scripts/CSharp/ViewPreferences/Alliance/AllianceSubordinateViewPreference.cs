using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceSubordinateViewPreference : BaseViewPreference {
        [Tooltip("UIAlliance.PnlAlliance.PnlSubordnateHoldler.PnlSubordnate CustomScrollRect")]
        public CustomScrollRect subordnateScrollRect;
        [Tooltip("UIAlliance.PnlAlliance.PnlSubordnateHoldler.PnlSubordnate.PnlList")]
        public Transform pnlList;
        [Tooltip("UIAlliance.PnlAlliance.PnlSubordnateHoldler.PnlSubordnate.PnlList CustomVerticalLayoutGroup")]
        public CustomVerticalLayoutGroup listVerticalLayoutGroup;

        [Tooltip("UIAlliance.PnlAlliance.PnlListSort")]
        public AllianceSortViewPreference subordinateStatusPre;
    }
}
