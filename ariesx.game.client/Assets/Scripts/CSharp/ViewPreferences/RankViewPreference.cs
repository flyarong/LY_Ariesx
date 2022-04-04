using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class RankViewPreference : BaseViewPreference {
        [Tooltip("UIRank.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIRank.PnlRank.PnlTab PanelTabView")]
        public PanelNewTabsView tabView;
        [Tooltip("UIRank.PnlRank.PnlTab.BtnClose")]
        public Button btnClose;
        [Tooltip("UIRank.PnlRank.PnlChannel")]
        public RectTransform channelRectTransform;
        [Tooltip("UIRank.PnlRank.PnlChannel.PnlPersonalRank")]
        public Transform pnlPersonal;
        [Tooltip("UIRank.PnlRank.PnlChannel.PnlAllianceRank")]
        public Transform pnlAlliance;
        [Tooltip("UIRank.PnlRank.PnlChannel.PnlSiegeRank")]
        public Transform pnlSiegeRank;

        [Tooltip("UIRank.PnlRank.PnlChannel.PnlSiegeRank")]
        public Transform pnlMapSNRank;

        [Tooltip("UIRank.PnlRank.PnlOwnRankInfo")]
        public Transform pnlOwnRankInfo;
        public CanvasGroup ownRankInfo;

        [Tooltip("UIRank.PnlOccupationIntro")]
        public GameObject occupationIntro;
        public CanvasGroup occupationIntroCG;
        public CustomButton btnOccupationBG;
        public Button btnOccupationClose;
    }
}
