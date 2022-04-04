using Protocol;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class MapMarkViewPreference : BaseViewPreference {
        [Tooltip("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlContent.PnlMarks")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIMiniMap.PnlMiniMap.PnlContent.PnlInfo.PnlContent.PnlMarks.PnlMarkList")]
        public Transform pnlList;
        public GridLayoutGroup gridLayoutGroup;
    }
}
