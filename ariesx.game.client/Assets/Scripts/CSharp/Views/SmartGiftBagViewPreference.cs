using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class SmartGiftBagViewPreference : BaseViewPreference {

        [Tooltip("UISmartGiftBag/PnlPay/PnlHead/BtnClose")]
        public Button btnClose;
        [Tooltip("UISmartGiftBag/BtnBackground")]
        public CustomButton background;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/BtnPay")]
        public Button btnPay;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/BtnPay/Txt")]
        public TextMeshProUGUI txtPrice;
        [Tooltip("UISmartGiftBag/PnlPay/PnlHead/TxtTitle")]
        public TextMeshProUGUI txtTitle;
        [Tooltip("UISmartGiftBag/PnlPay/PnlTip")]
        public GameObject pnlTip;
        [Tooltip("UISmartGiftBag/PnlPay/PnlTip/TxtNoGem")]
        public TextMeshProUGUI txtTip;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/ImgHero/TxtNum")]
        public TextMeshProUGUI txtNum;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/ImgHero/TxtHero")]
        public TextMeshProUGUI txtHerName;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/ImgHero")]
        public Image imgHero;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/ImgHero/PnlStars")]
        public Transform pnlStars;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/ImgHero/SliFragment")]
        public Slider sliFragment;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/ImgHero/SliFragment/TxtAmount")]
        public TextMeshProUGUI txtFragment;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/ImgHero/SliFragment/FillArea/Fill")]
        public Image imgFill;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/ImgHero/SliFragment/ImgFlash")]
        public Image imgFlash;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/ImgHero/SliFragment/ImgMaxLevel")]
        public Image imgMaxLevel;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/ImgHero/SliFragment/ImgUpgrade")]
        public Image imgTierUp;
        public Image[] heroRarity;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/TxtPayed")]
        public TextMeshProUGUI txtPayed;
        [Tooltip("UISmartGiftBag/PnlPay/PnlDetail/ImgHero/ImgAniHero")]
        public Image ImgAniHero;
    }
}