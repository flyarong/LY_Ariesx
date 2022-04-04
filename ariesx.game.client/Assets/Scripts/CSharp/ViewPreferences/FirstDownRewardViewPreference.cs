using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class FirstDownRewardViewPreference : BaseViewPreference {
        [Tooltip("UIFirstDownReward.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIFirstDownReward.PnlRewardList.PnlHead.BtnClose")]
        public Button btnClose;
        [Tooltip("UIFirstDownReward.PnlRewardList")]
        public GameObject pnlFieldReward;
        public CanvasGroup FieldRewardCG;
        [Tooltip("UIFirstDownReward.PnlRewardList.PnlContent")]
        public GameObject pnlGetFieldReward;
        public CanvasGroup GetFieldRewardCG;
        [Tooltip("UIFirstDownReward.PnlRewardList.PnlContent.ScrollView")]
        public CustomScrollRect customScrollRect;
        [Tooltip("UIFirstDownReward.PnlRewardList.PnlContent.ScrollView.PnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup verticalLayoutGroup;

        [Tooltip("UIFirstDownReward.PnlGetReward.PnlDetail.PnlLandRewardItem")]
        public FirstDownRewardItemView rewardItemView;
        public GameObject imgHalo;
    }
}