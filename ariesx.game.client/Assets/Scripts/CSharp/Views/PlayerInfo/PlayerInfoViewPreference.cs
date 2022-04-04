using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class PlayerInfoViewPreference : BaseViewPreference {
        [Tooltip("UIPlayerInfo.BtnBackGround")]
        public CustomButton btnBackground;
        [Tooltip("UIPlayerInfo.PnlPlayerInfo.PnlNewTabs")]
        public PanelNewTabsView tabView;

        [Tooltip("UIPlayerInfo.BtnBackGround.PnlChannel.PnlForce")]
        public Transform pnlForce;
        [Tooltip("UIPlayerInfo.BtnBackGround.PnlChannel.PnlState")]
        public Transform pnlState;
    }
}