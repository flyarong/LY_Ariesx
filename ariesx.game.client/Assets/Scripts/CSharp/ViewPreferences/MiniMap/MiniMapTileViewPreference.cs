using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class MiniMapTileViewPreference : BaseViewPreference {
        [Tooltip("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlContent.PnlTiles")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlContent.PnlTiles.PnlCityList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup verticalLayoutGroup;
    }
}
