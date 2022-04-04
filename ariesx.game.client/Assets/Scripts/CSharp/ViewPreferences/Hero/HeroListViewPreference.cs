using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class HeroListViewPreference : BaseViewPreference {
        [Tooltip("UIHero.PnlHero.PnlContent.PnlHeroList.ScrollRect")]
        public CustomScrollRect scrollRect;
        [Tooltip("UIHero.PnlHero.PnlContent.PnlHeroList.ScrollRect.PnlList")]
        public Transform pnlList;
        public CustomVerticalLayoutGroup verticalLayoutGroup;

        [Tooltip("UIHero.PnlHero.PnlContent.PnlHeroList.PnlTotal")]
        public Transform pnlTotal;
        [Tooltip("UIHero.PnlHero.PnlContent.PnlHeroList.PnlTotal.TxtTotal")]
        public TextMeshProUGUI txtTotal;
        [Tooltip("UIHero.PnlHero.PnlContent.PnlHeroList.PnlTotal.BtnOrder")]
        public CustomButton btnOrder;
        [Tooltip("UIHero.PnlHero.PnlContent.PnlHeroList.PnlTotal.BtnOrder.PnlContent.Text")]
        public TextMeshProUGUI txtOrderLabel;

        [Tooltip("PnlHeroList.ScrollRect.PnlSelectHero")]
        public CanvasGroup selectedHero;
        [Tooltip("PnlHeroList.ScrollRect.PnlSelectHero RectTransform")]
        public RectTransform selectedHeroRT;
        [Tooltip("PnlHeroList.ScrollRect.PnlSelectHero.PnlAvatar.PnlHeroBig")]
        public HeroHeadView selectHeroHeadView;
        [Tooltip("PnlHeroList.ScrollRect.PnlSelectHero.PnlButtons.BtnInfo")]
        public CustomButton btnHeroDetail;
        [Tooltip("PnlHeroList.ScrollRect.PnlList.PnlSelectHero.BtnInfo.PnlUpgradeContent.TxtGold")]
        public TextMeshProUGUI txtUpgradCost;
        [Tooltip("PnlHeroList.ScrollRect.PnlList.PnlSelectHero.BtnInfo.PnlUpgradeContent")]
        public Transform pnlHeroUpgrad;
        [Tooltip("PnlHeroList.ScrollRect.PnlList.PnlSelectHero.BtnInfo.PnlContrnt")]
        public Transform pnlHeroDetail;
        [Tooltip("PnlHeroList.ScrollRect.PnlList.PnlSelectHero.SelectHeroBG")]
        public RectTransform selectHeroBG;
    }
}
