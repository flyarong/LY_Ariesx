using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using TMPro;

namespace Poukoute {
    [DisallowMultipleComponent]
    public class CampaignPreheatViewPreference : BaseViewPreference {
        public TextMeshProUGUI txtCampaignDesc;

        [Tooltip("PnlPreheat.PnlCampaignCountdown.PnlCountDownHour")]
        public Transform pnlCountDownHour;
        public CanvasGroup countDownHourCG;
        public CountDownHourView countDownHourView;
        [Tooltip("PnlPreheat.PnlCampaignCountdown.PnlCountDownday")]
        public Transform pnlCountDownDay;
        public CanvasGroup countDownDayCG;
        public CountDownDayView countDownDayView;

        [Tooltip("PnlPreheat.PnlCampaignRewards CustomButton")]
        public Button BtnCampaignRewards;
        [Tooltip("PnlPreheat.PnlCampaignRewards.PnlContent.PnlRewardItem")]
        public Transform pnlRewardsList;

        [Tooltip("PnlPreheat.BtnCampaignRule CustomButton")]
        public CustomButton btnCampaignRule;
    }
}
