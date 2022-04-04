using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {
    public class AllianceSubordinateStatusViewPreference : BaseViewPreference {
        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlSubordinateStatus.PnlContent CustomScrollRect")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlSubordinateStatus.PnlContent.PnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup listVerticalLayoutGroup;
        public CustomContentSizeFitter listContentSizeFitter;
        public RectTransform listRectTransform;
    }
}
