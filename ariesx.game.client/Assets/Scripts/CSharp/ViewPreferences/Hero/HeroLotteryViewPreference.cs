using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Poukoute {
    public class HeroLotteryViewPreference : BaseViewPreference {
        [Tooltip("UIHero.PnlHero.PnlContent.PnlLotteryList.ScrollRect.GridLottery")]
        public Transform grdLottery;
        [Tooltip("UIHero.PnlHero.PnlContent.PnlLotteryList.ScrollRect.GridLottery")]
        public CustomContentSizeFitter contentSizeFitter;
        [Tooltip("UIHero.PnlHero.PnlContent.PnlLotteryList.ScrollRect.GridLottery")]
        public CustomGridLayoutGroup gridLayoutGroup;
        [Tooltip("UIHero.PnlHero.PnlContent.PnlLotteryList.PnlTips.TxtTip")]
        public TextMeshProUGUI txtTip;
    }
}
