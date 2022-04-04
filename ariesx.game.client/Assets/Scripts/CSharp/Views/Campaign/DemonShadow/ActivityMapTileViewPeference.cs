using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class ActivityMapTileViewPeference : BaseViewPreference {

        [Tooltip("UIActivityMapTile/PnlTile/PnlBossViewInfo/BtnDetail")]
        public CustomButton btnDetail;

        [Tooltip("UIActivityMapTile/PnlTile/PnlBossViewInfo/PnlMainInfo/PnlEndtime/TxtTime")]
        public TextMeshProUGUI txtEndtime;

        [Tooltip("UIActivityMapTile/PnlTile/PnlBossViewInfo/PnlMainInfo/PnlBossInfo/TxtBossName")]
        public TextMeshProUGUI txtBossName;

        [Tooltip("UIActivityMapTile/PnlTile/PnlBossViewInfo/PnlMainInfo/PnlBossInfo/TxtBossLevel")]
        public TextMeshProUGUI txtBossLevel;

        [Tooltip("UIActivityMapTile/PnlTile/PnlBossViewInfo/PnlMainInfo/PnlBossInfo/TxtBossFightingValue")]
        public TextMeshProUGUI txtBossFighting;

        [Tooltip("UIActivityMapTile/PnlTile/PnlBossViewInfo/PnlMainInfo/PnlRemainingBlood/SliRemainingBlood")]
        public Slider sliRemainingBlood;

        [Tooltip("UIActivityMapTile/PnlTile/PnlBossViewInfo/PnlMainInfo/PnlRemainingBlood/SliRemainingBlood/TxtAmount")]
        public TextMeshProUGUI txtSliAmount;

        [Tooltip("UIActivityMapTile/PnlRankRewardBg/PnlRankBg/PnlFirstRankSample")]
        public ActivityMapRankView pnlFirstRankSample;

        [Tooltip("UIActivityMapTile/PnlRankRewardBg/PnlRankBg/PnlSecondRankSample")]
        public ActivityMapRankView pnlSecondRankSample;

        [Tooltip("UIActivityMapTile/PnlRankRewardBg/PnlRankBg/PnlThirdRankSample")]
        public ActivityMapRankView pnlThirdRankSample;

        [Tooltip("UIActivityMapTile/PnlRankRewardBg/PnlRankBg/TxtOwnRank/TxtOwnRank")]
        public TextMeshProUGUI txtOwnRank;

        [Tooltip("UIActivityMapTile/PnlRankRewardBg/PnlRankBg/TxtOwnRank/TxtOwnName")]
        public TextMeshProUGUI txtOwnName;

        [Tooltip("UIActivityMapTile/PnlRankRewardBg/PnlRankBg/TxtOwnRank/TxtOwnHarm")]
        public TextMeshProUGUI txtOwnHarm;

        [Tooltip("UIActivityMapTile/PnlRankRewardBg/PnlGetReward/GetRewardList")]
        public Transform rewardList;

        [Tooltip("UIActivityMapTile/PnlTile")]
        public Transform pnlBossViewInfo;

        [Tooltip("UIActivityMapTile/PnlRankRewardBg")]
        public Transform pnlDemonRankRewardBg;
        public CanvasGroup demonRankRewardBgCanvasGroup;

    }
}
