using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class FteViewPreference : BaseViewPreference {
        [Tooltip("UIFte Canvas")]
        public Canvas canvase;
        [Tooltip("UIFte CanvasScaler")]
        public CanvasScaler canvaseScaler;

        [Tooltip("UIFte.Background")]
        public Transform background;
        [Tooltip("UIFte.PnlComic")]
        public Transform pnlComic;
        [Tooltip("UIFte.PnlComic.Background CustomClick")]
        public CustomClick clickComic;
        [Tooltip("UIFte.PnlChat")]
        public Transform pnlChat;
        [Tooltip("UIFte.PnlChat.Image")]
        public Image chatImage;
        [Tooltip("UIFte.PnlChat CustomClick")]
        public CustomClick chatClick;
        [Tooltip("UIFte.PnlChat CustomClick")]
        public CustomClick clickChat;
        [Tooltip("UIFte.PnlChat.PnlContent")]
        public Transform pnlChatContent;
        [Tooltip("UIFte.PnlChat.PnlContent.PnlText")]
        public Transform pnlText;
        [Tooltip("UIFte.PnlChat.PnlContent.PnlText.Text")]
        public TextMeshProUGUI txtChat;
        [Tooltip("UIFte.PnlChat.PnlContent.PnlRightText")]
        public Transform pnlRightText;
        [Tooltip("UIFte.PnlChat.PnlContent.PnlRightText.Text")]
        public TextMeshProUGUI txtRightChat;
        [Tooltip("UIFte.PnlChat.PnlContent.PnlText.BtnGo")]
        public CustomButton btnChat;
        [Tooltip("UIFte.PnlChat.PnlContent.PnlText.BtnGo RectTransform")]
        public RectTransform chatBtnGoRT;
        [Tooltip("UIFte.PnlChat.PnlContent.PnlImg.ImgSelf")]
        public Image imgSelf;
        [Tooltip("UIFte.PnlChat.PnlContent.PnlImg.ImgRight")]
        public Image imgRight;
        [Tooltip("UIFte.PnlArrow")]
        public Transform pnlArrow;
        [Tooltip("UIFte.PnlArrow")]
        public RectTransform arrowRT;
        [Tooltip("UIFte.PnlArrow")]
        public GameObject arrowImg;
        [Tooltip("UIFte.PnlShow")]
        public Transform pnlShow;
        [Tooltip("UIFte.PnlClickable")]
        public Transform pnlClickable;
        [Tooltip("UIFte.PnlDark")]
        public Transform pnlDark;

        [Tooltip("UIFte.PnlBanner")]
        public Transform pnlBanner;
        [Tooltip("UIFte.PnlBanner")]
        public CanvasGroup bannerCanvasGroup;
        [Tooltip("UIFte.PnlUp")]
        public Transform pnlBannerUp;
        [Tooltip("UIFte.PnlBelow")]
        public Transform pnlBannerBelow;

        [Tooltip("UIFte.PnlCloud")]
        public Transform pnlCloud;
        [Tooltip("UIFte.PnlDesc")]
        public Transform pnlDesc;
        [Tooltip("UIFte.PnlDesc.Text")]
        public TextMeshProUGUI txtDesc;
    }
}
