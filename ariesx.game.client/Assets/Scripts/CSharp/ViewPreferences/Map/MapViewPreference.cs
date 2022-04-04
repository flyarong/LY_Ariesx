using Poukoute;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class MapViewPreference : BaseViewPreference {
        // PnlBottom
        [Tooltip("UIMap.PnlButtons")]
        public Transform pnlButtons;
        [Tooltip("PnlButtons CanvasGroup")]
        public CanvasGroup cgBottom;
        [Tooltip("PnlButtonPanel")]
        public Transform pnlButtonPanel;

        [Tooltip("BtnShowBtnsPanel")]
        public CustomButton btnShowBtnsPnl;
        [Tooltip("BtnShowBtnsPanel.ImgNotice")]
        public Transform pnlButtonPanelNotice;
        [Tooltip("BtnShowBtnsPanel.PnlContent.ImgIcon")]
        public RectTransform imgShowBtnsPnl;
        [Tooltip("PnlBottom.PnlLeft.BtnMail")]
        public CustomButton btnMail;
        [Tooltip("PnlBottom.PnlLeft.BtnRank")]
        public CustomButton btnRank;
        [Tooltip("PnlLeft.BtnHouseKeeper")]
        public CustomButton btnHouseKeeper;
        [Tooltip("PnlLeft.BtnPay")]
        public CustomButton btnPay;
        [Tooltip("PnlLeft.BtnAlliance")]
        public CustomButton btnAlliance;
        [Tooltip("PnlLeft.BtnTest")]
        public CustomButton btnTest;
        [Tooltip("PnlBottom.PnlLeft.BtnAlliance.PnlContent.ImgNotice")]
        public Transform pnlAllianceNotice;
        //[Tooltip("PnlBottom.PnlLeft.BtnLyVoice")]
        //public Transform pnlLyVoice;
        [Tooltip("PnlBottom.PnlLeft.BtnLyVoice Button")]
        public CustomButton btnLyVoice;

        [Tooltip("PnlButtons.PnlRightBottom")]
        public Transform pnlRightBottom;
        [Tooltip("PnlRightBottom.PnlLottery")]
        public Transform pnlLottery;
        [Tooltip("PnlRightBottom.PnlLottery.BtnHero")]
        public CustomButton btnHero;
        [Tooltip("BtnHero.Content.ImgNewFreePoint")]
        public Transform imgNewFreePoint;
        [Tooltip("BtnHero.Content.ImgNewFreePoint.TxtAmount")]
        public TextMeshProUGUI txtFreeNumber;
        [Tooltip("BtnHero.Content.ImgNewPoint")]
        public Transform imgNewPoint;
        [Tooltip("BtnHero.Content.ImgNewPoint.TxtAmount")]
        public TextMeshProUGUI txtNewNumber;
        [Tooltip("BtnHero.Content.ImgLevelUpPoint")]
        public Transform imgLevelUpPoint;
        [Tooltip("BtnHero.Content.ImgLevelUpPoint.TxtAmount")]
        public TextMeshProUGUI txtLevelUpNumber;
        [Tooltip("BtnChest.Content.PnlNotice")]
        public CustomButton btnFreeChest;
        public Transform pnlFreeChestWhole;
        [Tooltip("BtnChest")]
        public Transform pnlFreeChestContent;
        
       [Tooltip("UIMap/PnlButtons/PnlRightBottom/PnlLottery/PnlChestNum")]
        public Transform pnlChestNum;
        
       [Tooltip("UIMap/PnlButtons/PnlRightBottom/PnlLottery/PnlChestNum/Slider")]
        public Slider sliderChestNum;
        
       [Tooltip("UIMap/PnlButtons/PnlRightBottom/PnlLottery/PnlChestNum/Slider/ImgFlash")]
        public Image imgFlash;
        
       [Tooltip("UIMap/PnlButtons/PnlRightBottom/PnlLottery/PnlChestNum/Slider/TxtAmount")]
        public TextMeshProUGUI txtChestNum;
        
       [Tooltip("UIMap/PnlButtons/PnlRightBottom/PnlLottery/PnlChestNum/ImgChest")]
        public Transform pnlChestIcon;
        [Tooltip("BtnMiniMap")]
        public CustomButton btnMiniMap;
        [Tooltip("BtnAutoBattle")]
        public CustomButton btnAutoBattle;
        [Tooltip("BtnAutoBattle")]
        public TextMeshProUGUI txtAutoBattle;
        [Tooltip("BtnSelectSever")]
        public CustomButton btnSelectSever;
        [Tooltip("PnlButtons.PnlRightUp")]
        public Transform pnlRightUp;
        [Tooltip("PnlRightUp.PnlCampaign")]
        public Transform pnlCampaign;
        [Tooltip("PnlRightUp.PnlCampaign.BtnCampaign")]
        public CustomButton btnCampaign;
        [Tooltip("PnlRightUp.PnlCampaign.BtnCampaign")]
        public CampaignBtnView CampaignBtnView;
        [Tooltip("PnlRightUp.PnlStore")]
        public Transform pnlStore;
        [Tooltip("PnlRightUp.PnlStore.BtnStore")]
        public StoreBtnView storeBtnView;
        [Tooltip("PnlRightUp.PnlStore.BtnStore")]
        public CustomButton btnStore;
        [Tooltip("PnlRightUp.PnlPayReward")]
        public Transform pnlPayReward;
        [Tooltip("PnlRightUp.PnlPayReward.BtnPayReward")]
        public CustomButton btnPayReward;


        [Tooltip("PnlButtons.PnlLeftBottom")]
        public Transform pnlLeftBottom;
        [Tooltip("PnlButtons.PnlLeftBottom.BtnJump CustomButton")]
        public CustomButton btnJump;
        
       [Tooltip("UIMap/PnlButtons/PnlLeftBottom/PnlTask")]
        public Transform pnlTask;
        
       [Tooltip("UIMap/PnlButtons/PnlLeftBottom/PnlTask/BtnJump/PnlContent")]
        public Transform pnlTaskInfo;
        
       [Tooltip("UIMap/PnlButtons/PnlLeftBottom/PnlTask/BtnJump/PnlContent/ImgJump")]
        public Image imgJump;
        
       [Tooltip("UIMap/PnlButtons/PnlLeftBottom/PnlTask/BtnJump/PnlContent")]
        public Image imgJumpBG;
        [Tooltip("PnlButtons.PnlLeftBottom.BtnJump.PnlContent.TxtDescription")]
        public TextMeshProUGUI txtTaskContent;
        [Tooltip("PnlButtons.PnlLeftBottom.BtnTask CustomButton")]
        public CustomButton btnTask;
        public Image imgTaskIcon;
        public TextMeshProUGUI txtTaskStatus;
        [Tooltip("PnlButtons.PnlLeftBottom.ImgNewPoint")]
        public Transform pnlTaskNewPoint;
        [Tooltip("PnlButtons.PnlLeftBottom.ImgNewPoint.TxtAmount")]
        public TextMeshProUGUI txtTaskRewardCount;
        [Tooltip("PnlLeftBottom.BtnBuild")]
        public CustomButton btnBuild;
        [Tooltip("PnlLeftBottom.BtnBuild.PnlBuildContent")]
        public Transform pnlBuildContent;
        [Tooltip("PnlLeftBottom.BtnHouseKeeper")]
        public CustomButton btnBottomHouseKeeper;
        [Tooltip("PnlLeftBottom.BtnHouseKeeper.PnlNotice")]
        public Transform pnlHousekeeperNotice;
        [Tooltip("PnlLeftBottom.BtnHouseKeeper.PnlHouseKeeperContent")]
        public Transform pnlHouseKeeperContent;
        [Tooltip("PnlLeftBottom.BtnHouseKeeper.PnlHouseKeeperTips")]
        public Transform pnlHouseKeeperTips;
        [Tooltip("PnlLeftBottom.BtnChat")]
        public CustomButton btnChat;
        [Tooltip("PnlLeftBottom.BtnChat.PnlChatInfo.TxtChatPlayer")]
        public TextMeshProUGUI txtChatPlayer;
        [Tooltip("PnlLeftBottom.BtnChat.PnlChatInfo.TxtChatInfo")]
        public TextMeshProUGUI txtChatInfo;

        [Tooltip("PnlButtons.PnlLeftUp")]
        public Transform pnlLeftUp;
        [Tooltip("PnlButtons.PnlLeftUp.BtnFallen")]
        public Transform pnlFallen;
        [Tooltip("PnlButtons.PnlLeftUp.BtnFallen Button")]
        public CustomButton btnFallen;

        [Tooltip("PnlButtons.TxtSN")]
        public TextMeshProUGUI txtSN;
        [Tooltip("UIMap.PnlQueue")]
        public Transform pnlQueue;
        [Tooltip("UIMap.PnlQueue.PnlContent.BtnFold.PnlContent.ImgIcon")]
        public RectTransform imgShowQueue;
    }
}
