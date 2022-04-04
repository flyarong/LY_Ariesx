using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using TMPro;

namespace Poukoute
{
    public class HouseKeeperViewPreference : BaseViewPreference {
        [Tooltip("UIHouseKeeper.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlNewTabs PanelTabView")]
        public PanelNewTabsView tabView;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlDaily")]
        public Transform pnlDaily;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlBuild")]
        public Transform pnlBuild;
    }
}
