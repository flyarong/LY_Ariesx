using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class MapTopHUDViewPreference : BaseViewPreference {
        [Tooltip("UIMapTopHUD.PnlTop")]
        public RectTransform topRectTransform;
        [Tooltip("UIMapTopHUD.PnlTop.PnlPlayerInfo")]
        public Transform pnlPlayerInfo;
        [Tooltip("UIMapTopHUD.PnlTop.PnlPlayerInfo Button")]
        public Button btnPlayerInfo;
        [Tooltip("UIMapTopHUD.PnlTop.PnlPlayerInfo.PnlForce")]
        public Transform pnlForce;
        [Tooltip("UIMapTopHUD.PnlTop.PnlPlayerInfo.PnlForce.ImgCircleSliderFill")]
        public Image imgCircleSliderFill;
        [Tooltip("UIMapTopHUD.PnlTop.PnlPlayerInfo.PnlForce.TxtLevel")]
        public TextMeshProUGUI txtForceLevel;

        [Tooltip("UIMapTopHUD.PnlTop.PnlPlayerInfo.TxtName")]
        public TextMeshProUGUI txtPlayerName;
        [Tooltip("UIMapTopHUD.PnlTop.PnlPlayerInfo.PnlForce.ImgCitcleSliderFill.ImgAvatar")]
        public Image imgPlayerAvatar;

        [Tooltip("UIMapTopHUD.PnlTop.PnlPlayerInfo.PnlTerrain")]
        public Transform pnlTerrain;
        [Tooltip("UIMapTopHUD.PnlTop.PnlPlayerInfo.PnlTerrain")]
        public TextMeshProUGUI txtTerrainInfo;
        [Tooltip("UIMapTopHUD.PnlTop.PnlPlayerInfo.PnlTerrain Button")]
        public Button btnTerrain;
        [Tooltip("UIMapTopHUD.PnlTop.PnlPlayerInfo.PnlTerrain.ImgIcon")]
        public Transform imgTerrain;


        [Tooltip("UIMapTopHUD.PnlTop.PnlResource")]
        public Transform pnlResource;
        //[Tooltip("UIMapTopHUD.PnlTop.PnlResource GridLayoutGroup")]
        //public GridLayoutGroup resourceGrid;

        [Tooltip("UIMapTopHUD/PnlTop/PnlResource/PnlLumber")]
        public GameObject pnlLumber;

        [Tooltip("UIMapTopHUD/PnlTop/PnlResource/PnlLumber")]
        public MapResourceItemView lumberItemView;

        [Tooltip("UIMapTopHUD/PnlTop/PnlResource/PnlMarble")]
        public GameObject pnlSteel;

        [Tooltip("UIMapTopHUD/PnlTop/PnlResource/PnlMarble")]
        public MapResourceItemView steelItemView;

        [Tooltip("UIMapTopHUD/PnlTop/PnlResource/PnlSteel")]
        public GameObject pnlMarble;

        [Tooltip("UIMapTopHUD/PnlTop/PnlResource/PnlSteel")]
        public MapResourceItemView marbleItemView;

        [Tooltip("UIMapTopHUD/PnlTop/PnlResource/PnlFood")]
        public GameObject pnlFood;

        [Tooltip("UIMapTopHUD/PnlTop/PnlResource/PnlFood")]
        public MapResourceItemView foodItemView;


        [Tooltip("UIMapTopHUD.PnlTop.PnlResource.PnlGold")]
        public GameObject pnlGold;
        [Tooltip("UIMapTopHUD.PnlTop.PnlResource.PnlGem")]
        public GameObject pnlGem;
        [Tooltip("UIMapTopHUD.PnlTop.PnlResource.PnlGem")]
        public GameObject pnlGemAnim;
        [Tooltip("UIMapTopHUD.PnlTop.PnlResource.PnlGem Button")]
        public Button btnBuyGem;
        [Tooltip("UIMapTopHUD.PnlTop.PnlResource.PnlGem.ImgIcon")]
        public Transform imgGem;
        [Tooltip("UIMapTopHUD.PnlStar")]
        public Transform pnlStar;
        
       [Tooltip("UIMapTopHUD/PnlTop/PnlPlayerInfo/PnlIcon")]
        public Transform pnlIcon;
        
       [Tooltip("UIMapTopHUD/PnlTop/PnlPlayerInfo/PnlIcon/PnlForce/ImgCitcleSliderFill/ImgIconFilledAnimation")]
        public GameObject imgIconFilledAnimation;
        
       [Tooltip("UIMapTopHUD/PnlTop/PnlPlayerInfo/PnlState/BtnFallen")]
        public CustomButton btnFallen;
        
       [Tooltip("UIMapTopHUD/PnlTop/PnlPlayerInfo/PnlState/BtnNoviceState")]
        public CustomButton btnNovice;
        
       [Tooltip("UIMapTopHUD/PnlTop/PnlPlayerInfo/PnlState/BtnMouthCard")]
        public CustomButton btnMonthCard;
        
       [Tooltip("UIMapTopHUD/PnlTop/PnlPlayerInfo/PnlState/BtnLoginReward")]
        public CustomButton btnLoginReward;
        
       [Tooltip("UIMapTopHUD/PnlTop/PnlPlayerInfo/PnlState/BtnLoginReward/ImgNotice")]
        public Transform imgNotice;
        
       [Tooltip("UIMapTopHUD/PnlTop/PnlPlayerInfo/PnlState/BtnLoginReward/ImgNotice/TxtNumber")]
        public TextMeshProUGUI txtNoticeNumber;
    }
}
