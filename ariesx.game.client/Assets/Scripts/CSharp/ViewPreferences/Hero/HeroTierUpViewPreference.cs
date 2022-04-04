using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Poukoute
{
    public class HeroTierUpViewPreference : BaseViewPreference {
        [Tooltip("UITierUp.ImgAbove Button")]
        public Button btnClose;
        [Tooltip("UITierUp.PnlHeroInfo")]
        public Transform pnlHeroInfo;
        public VerticalLayoutGroup heroInfoVerticalLayoutGroup;
        [Tooltip("UITierUp.PnlHeroInfo.PnlAttributes")]
        public Transform pnlAttributes;
        [Tooltip("UITierUp.PnlHeroInfo.PnlName.Text")]
        public TextMeshProUGUI txtName;
        public Transform animName;
        [Tooltip("UITierUp.PnlHeroInfo.PnlFragment.Slider")]
        public CustomSlider sldFragment;
        public Image imgFill;
        [Tooltip("UITierUp.PnlHeroInfo.PnlFragment.Slider.TxtAmount")]
        public TextMeshProUGUI txtFragment;
        [Tooltip("UITierUp.PnlHeroInfo.PnlStars")]
        public RectTransform pnlStars;
        [Tooltip("UITierUp.PnlHeroInfo.PnlFragment")]
        public Transform pnlFragment;
        [Tooltip("UITierUp.PnlHeroInfo.PnlFragment.ImgUpgrade")]
        public Transform Upgrade;
        public Image imgUpgrade;
        public Image imgFlash;
    }
}
