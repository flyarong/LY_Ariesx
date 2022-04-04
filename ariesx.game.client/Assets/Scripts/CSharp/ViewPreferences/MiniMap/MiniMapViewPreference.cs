using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class MiniMapViewPreference : BaseViewPreference {
        [Tooltip("UIMiniMap.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIMiniMap.PnlMiniMap")]
        //public Transform pnlMiniMap;
        public RectTransform miniMapRectTransform;
        [Tooltip("UIMiniMap.PnlMiniMap.PnlTitle.BtnClose")]
        public Button btnClose;
        [Tooltip("UIMiniMap.PnlMiniMap.PnlTitle.BtnUnfold")]
        public Button btnUnfold;

        [Tooltip("UIMiniMap.PnlMiniMap.PnlContent.PnlMap")]
        public Transform pnlMap;
        [Tooltip("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo")]
        //public Transform pnlInfo;
        public RectTransform infoRectTransform;

        [Tooltip("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlTab")]
        public PanelOnlyTabsView tabView;
        [Tooltip("UIMiniMap.PnlArrow")]
        public Transform pnlArrow;
    }
}
