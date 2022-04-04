using UnityEngine;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Poukoute {
    public class BuildInfoViewPreference: BaseViewPreference {
        /* UI Members*/
        [Tooltip("UIBuild.BtnBackground")]
        public Button btnBackground;
        [Tooltip("UIBuild.ImgDarkBackground")]
        public GameObject imgDarkBackground;
        [Tooltip("UIBuild.PnlBuildInfo")]
        public Transform pnlBuildInfo;
        [Tooltip("UIBuild.PnlBuildInfo.PnlContent.PnlTitle.TxtTitle")]
        public TextMeshProUGUI txtTile;
        [Tooltip("UIBuildInfo.PnlBuildInfo.PnlContent.PnlTitle.BtnClose")]
        public Button btnClose;
        [Tooltip("UIBuild.PnlBuildInfo.PnlContent.PnlInfo.PnlHealth.ImgAvatar")]
        public Image imgAvatar;
        [Tooltip("UIBuild.PnlBuildInfo.PnlContent.PnlInfo.PnlHealth.TxtHealth")]
        public TextMeshProUGUI txtHealth;
        [Tooltip("UIBuild.PnlBuildInfo.PnlContent.PnlInfo.PnlHealth.TxtHealthAddition")]
        public TextMeshProUGUI txtHealthAddition;
        [Tooltip("UIBuild.PnlBuildInfo.PnlContent.PnlInfo.PnlResourcesBonus")]
        public Transform pnlResourcesBonus;

        [Tooltip("UIBuild.PnlBuildInfo.PnlDescription")]
        public Transform pnDescription;
        [Tooltip("UIBuild.PnlBuildInfo.PnlContent.PnlDescription.PnlTextContent.TxtDescription")]
        public TextMeshProUGUI txtDescription;

        [Tooltip("UIBuild.PnlBuildInfo.PnlContent.PnlUnlockBuild")]
        public Transform pnlUnlockBuilds;
        [Tooltip("UIBuild.PnlBuildInfo.PnlContent.PnlUnlockBuild.PnlUnlockList")]
        public Transform pnlUnlockBuildList;

        [Tooltip("UIBuild.PnlBuildInfo.PnlContent.PnlUpgradeFailInfo")]
        public Transform pnlUpgradeFailInfo;
        [Tooltip("UIBuild.PnlBuildInfo.PnlContent.PnlUpgradeFailInfo.Imgbackground.TxtUpgradeInfoTitle")]
        public TextMeshProUGUI txtUpgradeInfoTitle;

        [Tooltip("PnlUpgradeRequire")]
        public Transform pnlUpgradeRequire;
        public GameObject[] upgradeResources;
        public UpgradeResourcesItemView[] resourceItemView;

        [Tooltip("UIBuild.PnlBuildInfo.PnlContent.PnlButtons.BtnUpgrade")]
        public CustomButton btnUpgrade;
        [Tooltip("UIBuild.PnlBuildInfo.PnlContent.PnlButtons.PnlUpgradeTimeInfo.PnlUpgradTime.TxtTime")]
        public TextMeshProUGUI txtTime;

        [Tooltip("UIBuild.PnlUnlockBuildInfo")]
        public Transform pnlUnlockBuildingDesc;
        [Tooltip("UIBuild.PnlUnlockBuildInfo.PnlContent.TxtTribute")]
        public TextMeshProUGUI txtTribute;

        public GameObject pnlForceUpgrade;
        public Image imgBuildUpgrade;
        public TextMeshProUGUI txtForceUpgrade;
        public TextMeshProUGUI txtBuildUpgrade;

        public CustomButton btnBuildUpgrade;
        public CustomButton btnFeceUpgrade;
        public CustomButton btnResourceUpgrade;
        public GameObject pnlBuildUpgrade;
        public GameObject pnlResouceBtn;
        public TextMeshProUGUI txtBtnGo;
    }
}
