using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceRankViewPreference : BaseViewPreference {
        [Tooltip("UIRank.PnlRank.PnlChannel.PnlAllianceRank CustomScrollRect")]
        public CustomScrollRect customScrollRect;
        [Tooltip("UIRank.PnlRank.PnlChannel.PnlAllianceRank.PnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup customVerticalLayoutGroup;
    }
}
