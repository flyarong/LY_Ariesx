using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using TMPro;

namespace Poukoute {
    public class CampaignViewPreference : BaseViewPreference {
        [Tooltip("UICampaign.BtnBackground")]
        public Button btnBackground;

        [Tooltip("UICampaign.PnlCampaign")]
        public Transform pnlCampaign;

        [Tooltip("UICampaign.PnlCampaign.PnlTitle.BtnClose")]
        public CustomButton  btnClose;

        [Tooltip("UICampaign.PnlCampaign.PnlTop.PnlCampaigns ")]
        public RectTransform CampaignsRT;
        public CustomScrollRect scrollRect;
        [Tooltip("UICampaign.PnlCampaign.PnlTop.PnlCampaigns.PnlCampaignList")]
        public Transform pnlList;
        public RectTransform listRectTransform;

        [Tooltip("UICampaign.PnlCampaign.PnlTop.PnlArrow")]
        public Transform pnlArrow;
        public RectTransform arrowRT;
        [Tooltip("UICampaign.PnlCampaign.PnlTop.PnlBanner")]
        public CanvasGroup bannerCanvasGroup;
        [Tooltip("UICampaign.PnlCampaign.PnlTop.PnlBanner")]
        public RawImage imgBanner;
        [Tooltip("UICampaign.PnlCampaign.PnlTop.PnlBanner")]
        public RawImage imgBanners;
        [Tooltip("UICampaign.PnlCampaign.PnlTop.PnlTips")]
        public Transform pnlImgLoading;
        [Tooltip("UICampaign.PnlCampaign.PnlTop.TxtDuration")]
        public TextMeshProUGUI txtDuration;

        public CustomButton btnShowCampaignRule;

    }
}
