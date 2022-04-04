using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class DailyRewardNoRewardViewPeference: BaseViewPreference {
        [PathAttribute]
        public CustomButton btnBackground;
        public CustomButton btnClose;
        public Transform pnlHero;
        public Transform pnlResouceList;
        public Image imgHero;
        public TextMeshProUGUI txtHeroNumber;
        public TextMeshProUGUI txtHeroName;
        public TextMeshProUGUI txtTitle;
        public CustomButton btnReceiveds;
        public GameObject[] objRarity;
        public Button btnHeroClick;

    }
}
