using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class PayRewardViewPreference : BaseViewPreference {
        [Tooltip("BtnBackground")]
        public CustomButton btnBackground;

        [Tooltip("PnlInfo")]
        public Image imgBackground;
        [Tooltip("PnlInfo.BtnJump")]
        public CustomButton btnJump;
        [Tooltip("PnlInfo.BtnCollect")]
        public CustomButton btnCollect;
        [Tooltip("PnlInfo.BtnClose")]
        public CustomButton btnClose;
        [Tooltip("PnlInfo.BtnInfo")]
        public CustomButton btnInfo;

        [Tooltip("PnlInfo.PnlShowHero")]
        public Transform pnlShowHero;
        [Tooltip("PnlInfo.PnlShowHero.mgTitle")]
        public Image imgShowHero;
        [Tooltip("PnlInfo.PnlShowHero.ImgTitle.TxtPrice")]
        public TextMeshProUGUI txtHeroPrice;
        [Tooltip("PnlInfo.PnlShowResource")]
        public Transform pnlShowResource;
        [Tooltip("PnlInfo.PnlShowResource.mgTitle")]
        public Image imgShowResource;
        [Tooltip("PnlInfo.PnlShowResource.ImgTitle.TxtPrice")]
        public TextMeshProUGUI txtResourcePrice;

        [Tooltip("PnlInfo.PnlResources")]
        public Transform pnlResources;
    }
}