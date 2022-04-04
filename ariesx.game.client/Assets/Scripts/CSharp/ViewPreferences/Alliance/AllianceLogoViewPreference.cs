using UnityEngine;
using UnityEngine.Events;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceLogoViewPreference : BaseViewPreference {
        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlLogo.ScrollView.PnlLogoGrid")]
        public Transform pnlLogoGrid;
        public GridLayoutGroup logoGridLayoutGroup;
        public RectTransform logoRectTransform;
    }
}
