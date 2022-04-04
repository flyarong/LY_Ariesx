using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceCreateOrJoinViewPreference : BaseViewPreference {
        [Tooltip("UIAllianceCreateOrJoin.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIAllianceCreateOrJoin.PnlCreateOrJoin")]
        public Transform pnlCreateOrJoin;
        public CanvasGroup createOrJoinCG;
        public GameObject createOrJoin;
        [Tooltip("UIAllianceCreateOrJoin.PnlCreateOrJoin.PnlTab PanelTabView")]
        public PanelNewTabsView tabView;
        [Tooltip("UIAllianceCreateOrJoin.PnlCreateOrJoin.PnlSearchHead")]
        public Transform pnlSearchHead;

        [Tooltip("UIAllianceCreateOrJoin.PnlCreateOrJoin.PnlContent.PnlSearch")]
        public Transform pnlSearch;
        [Tooltip("UIAllianceCreateOrJoin.PnlCreateOrJoin.PnlContent.PnlCreate")]
        public Transform pnlCreate;
        [Tooltip("UIAllianceCreateOrJoin.PnlCreateOrJoin.PnlContent.PnlUnlockInfo")]
        public GameObject unlockInfo;
        public CanvasGroup allianceUnlockedCG;
        public Button btnUnlockedClose;
    }
}
