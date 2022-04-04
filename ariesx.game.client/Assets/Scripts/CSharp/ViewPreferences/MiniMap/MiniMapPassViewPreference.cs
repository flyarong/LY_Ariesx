using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
	public class MiniMapPassViewPreference : BaseViewPreference {
        [Tooltip("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlContent.PnlPasses")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlContent.PnlPasses.PnlPassList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup verticalLayoutGroup;
        public CustomContentSizeFitter listCustomContentSizeFitter;
        public RectTransform listRectTransform;
    }
}
