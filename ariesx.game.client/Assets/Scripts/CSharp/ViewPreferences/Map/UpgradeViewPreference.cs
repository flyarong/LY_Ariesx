using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class UpgradeViewPreference : BaseViewPreference {
        [Tooltip("UIMapTopHUD/PnlUpgrade/PnlForceAni")]
        public GameObject pnlForceAni;

        [Tooltip("UIMapTopHUD/PnlUpgrade")]
        public GameObject pnlUpGrade;

        [Tooltip("UIMapTopHUD/PnlUpgrade/PnlForceAni/ImgFill")]
        public Image imgFill;

        [Tooltip("UIMapTopHUD/PnlUpgrade/PnlForceAni/ImgFlashAll")]
        public Image imgFlashAll;

        [Tooltip("UIMapTopHUD/PnlUpgrade/PnlTitle")]
        public GameObject pnlTitle;

        [Tooltip("UIMapTopHUD/PnlUpgrade/PnlTitle/pnlMove")]
        public GameObject pnlTitleMove;

        [Tooltip("UIMapTopHUD/PnlUpgrade/PnlForceAni/ImgFill/ImgIcon/TxtForceAni")]
        public TextMeshProUGUI txtForceAni;

        [Tooltip("UIMapTopHUD/PnlUpgrade/BtnBackground")]
        public CustomButton btnForceAniBack;

        [Tooltip("UIMapTopHUD/PnlUpgrade/PnlForceAni/ImgFill/ImgFilledAnimation")]
        public GameObject objEedFilledAnimation;

        [Tooltip("UIMapTopHUD/PnlUpgrade/PnlForceAni/ImgFlashAllAnimation")]
        public GameObject objAllFlashAnimation;

        [Tooltip("UIMapTopHUD/PnlUpgrade/PnlForceAni/ImgFill/ImgFilledFlashAnimation")]
        public GameObject objFilledFlashAnimation;

        [Tooltip("UIMapTopHUD/PnlUpgrade/PnlTitle/pnlMove/ImgTitleAnimation")]
        public GameObject objTitleAnimation;

        [Tooltip("UIMapTopHUD/PnlUpgrade/PnlForceAni/ImgFill/ImgFilledFlashAnimation")]
        public Image imgFilledFlashAnimation;

        [Tooltip("UIMapTopHUD/PnlUpgrade/PnlTitle/pnlMove/TxtTitle")]
        public TextMeshProUGUI txtTitle;

        public GameObject imgIconFilledAnimation;

        public Transform pnlForce;

        public Image imgIcon;

    }
}
