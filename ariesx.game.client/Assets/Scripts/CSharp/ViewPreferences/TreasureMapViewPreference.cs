using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
	public class TreasureMapViewPreference : BaseViewPreference {
        [Tooltip("UITreasure.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UITreasure.PnlTreasure")]
        public Transform pnlTreasure;
        [Tooltip("UITreasure.PnlTreasure.PnlReward.PnlHeroAndGem.PnlGem")]
        public Transform pnlGem;
        [Tooltip("UITreasure.PnlTreasure.PnlReward.PnlHeroAndGem.PnlGem.Text")]
        public TextMeshProUGUI txtGemAmount;
        [Tooltip("UITreasure.PnlTreasure.PnlReward.PnlResources")]
        public Transform pnlResources;
    }
}
