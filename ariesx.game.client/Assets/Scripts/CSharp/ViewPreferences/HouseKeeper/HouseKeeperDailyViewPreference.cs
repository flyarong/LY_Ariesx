using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute
{
    public class HouseKeeperDailyViewPreference : BaseViewPreference {

        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlDaily.PalList")]
        public Transform pnlDailyList;
        public CustomContentSizeFitter contentSizeFitter;
        public CustomVerticalLayoutGroup customVerticalLayoutGroup;
        public RectTransform rectTransform;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlDaily")]
        public Transform pnlDaily;
        public CustomScrollRect customScrollRect;
        public Button btnDaily;
    }
}
