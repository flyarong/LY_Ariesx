using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using TMPro;

namespace Poukoute {
    public class CampaignRewardViewPreference : BaseViewPreference {
        [Tooltip("UICampaignReward.BtnBackground")]
        public Button btnBackground;

        [Tooltip("UICampaignReward.PnlCampaignReward.PnlTitle.BtnClose")]
        public CustomButton btnClose;

        [Tooltip("UICampaignReward.PnlCampaignReward.PnlTaskScrollRect CustomScrollRect")]
        public CustomScrollRect scrollRect;
        [Tooltip("UICampaignReward.PnlCampaignReward.PnlTaskScrollRect.PnlList")]
        public Transform pnlList;
        public RectTransform rectTransform;
    }
}
