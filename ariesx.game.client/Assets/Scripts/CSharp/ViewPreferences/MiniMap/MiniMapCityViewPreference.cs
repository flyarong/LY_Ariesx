using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class MiniMapCityViewPreference : BaseViewPreference {
        [Tooltip("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlContent.PnlCities")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlContent.PnlCities.PnlCityList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup verticalLayoutGroup;
        public CustomContentSizeFitter listCustomContentSizeFitter;
        public RectTransform listRectTransform;
    }
}
