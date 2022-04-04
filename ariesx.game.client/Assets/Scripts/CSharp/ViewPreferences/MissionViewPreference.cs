using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;

namespace Poukoute {
    public class MissionViewPreference : BaseViewPreference {
        [Tooltip("UIMission.BtnBackground")]
        public Button btnBackground;

        [Tooltip("UIMission.PnlMission")]
        public Transform pnlMission;
        [Tooltip("UIMission.PnlMission.PnlTab")]
        public PanelNewTabsView tabView;
        [Tooltip("UIMission.PnlMission.PnlPanel.PnlDrama")]
        public Transform pnlDrama;
        [Tooltip("UIMission.PnlMission.PnlPanel.PnlDailyTask")]
        public Transform pnlDailyTask;
    }
}
