using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class OccupationRankViewPreference : BaseViewPreference {
        [Tooltip("UIRank.PnlRank.PnlChannel.PnlSiegeRank.PnlContent CustomScrollRect")]
        public CustomScrollRect customScrollRect;
        [Tooltip("UIRank.PnlRank.PnlChannel.PnlSiegeRank.PnlContent.PnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup customVerticalLayoutGroup;

        [Tooltip("UIRank.PnlRank.PnlChannel.PnlSiegeRank.BtnIntro")]
        public Button btnIntro;
    }
}
