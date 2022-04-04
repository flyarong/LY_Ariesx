using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class PersonalRankViewPreference : BaseViewPreference {
        [Tooltip("UIRank.PnlRank.PnlChannel.PnlPersonalRank customScrollRect")]
        public CustomScrollRect customScrollRect;
        [Tooltip("UIRank.PnlRank.PnlChannel.PnlPersonalRank.PnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup customVerticalLayoutGroup;
        public CanvasGroup unlockedWorldRankCG;
    }
}
