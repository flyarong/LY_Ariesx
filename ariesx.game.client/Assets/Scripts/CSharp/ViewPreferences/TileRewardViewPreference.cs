using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {
    public class TileRewardViewPreference : BaseViewPreference {
        [Tooltip("UITileRewardBind.PnlTileRewardBind.PnlResources")]
        public Transform pnlTileRewardList;
        [Tooltip("UITileRewardBind.PnlTileRewardBind.PnlDetail HorizontalLayoutGroup")]
        public HorizontalLayoutGroup horizontalLayoutGroup;
        public CustomContentSizeFitter contentSizeFitter;
        [Tooltip("UITileRewardBind.PnlTileRewardBind.PnlNormalRewardParent")]
        public GameObject pnlNormalReward;
        [Tooltip("UITileRewardBind.PnlTileRewardBind.PnlNormalReward CanvasGroup")]
        public CanvasGroup normalRewardCG;
        [Tooltip("UITileRewardBind.PnlTileRewardBind.PnlNormalReward.ImgChest")]
        public Image imgChest;
        [Tooltip("UITileRewardBind.PnlTileRewardBind.PnlNormalReward.TxtChest")]
        public TextMeshProUGUI txtChest;
        [Tooltip("UITileRewardBind.PnlTileRewardBind.PnlNormalReward.TxtChest.TxtChestLevel")]
        public TextMeshProUGUI txtChestLevel;
        [Tooltip("UITileRewardBind.PnlTileRewardBind.PnlLimitRewardParent")]
        public GameObject pnlLimitReward;
        [Tooltip("UITileRewardBind.PnlTileRewardBind.PnlLimitReward CanvasGroup")]
        public CanvasGroup limitRewardCG;
        [Tooltip("UITileRewardBind.PnlTileRewardBind.PnlLimitReward.PnlGem.PnlBackground.Text")]
        public TextMeshProUGUI txtGemCount;
        [Tooltip("UITileRewardBind.PnlTileRewardBind.PnlLimitReward.TxtRemainTime")]
        public TextMeshProUGUI txtRemainTime;
    }
}
