using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {
    public class MapTileViewPreference : BaseViewPreference {
        // Tile overview
        [Tooltip("PnlOverViewBind")]
        public Transform pnlOverviewBind;
        [Tooltip("PnlOverview")]
        public Transform pnlOverview;
        public RectTransform overViewRT;
        [Tooltip("PnlMainInfo")]
        public Transform pnlMainInfo;
        public VerticalLayoutGroup mainInfoVerticalLG;
        public ContentSizeFitter mainInfoContentSF;
        [Tooltip("PnlMainInfo.TxtInfo")]
        public TextMeshProUGUI txtInfo;
        [Tooltip("TxtCoordinate")]
        public TextMeshProUGUI txtCoordinate;
        [Tooltip("PnlDefenderPower")]
        public Transform pnlDefendersPower;
        [Tooltip("PnlPlayerInfo.ImgAlliance")]
        public Image imgAllianceIcon;
        [Tooltip("PnlPlayerInfo.ImgAlliance.ImgBG")]
        public Image imgAllianceIconBG;
        [Tooltip("PnlDefenderPower.TxtPower")]
        public TextMeshProUGUI txtDefenderPower;
        [Tooltip("PnlDefenderPower.TxtPower")]
        public TextMeshProUGUI txtDefenderCount;
        [Tooltip("PnlTributeAddition")]
        public Transform pnlTributeAddition;
        [Tooltip("PnlTributeAddition.TxtAddition")]
        public TextMeshProUGUI txtTributeAddition;
        [Tooltip("PnlDefendersRecover")]
        public Transform pnlDefendersRecover;
        [Tooltip("PnlRecoverTime.TxtTime")]
        public TextMeshProUGUI txtRecoverTime;
        [Tooltip("BtnDetail")]
        public Button btnDetail;
        [Tooltip("PnlEndurance")]
        public Transform pnlEndurance;
        [Tooltip("PnlEndurance.Slider")]
        public Slider sldEndurance;
        [Tooltip("Slider.FillArea.TxtAmount")]
        public TextMeshProUGUI txtEndurance;
        [Tooltip("Slider.FillArea.FillContent.Fill")]
        public Image imgEndurance;

        [Tooltip("PnlProductions")]
        public Transform pnlProductions;
        [Tooltip("PnlProductions.PnlProductionList")]
        public Transform pnlProductionList;

        [Tooltip("PnlAllianceMarkInfo")]
        public Transform pnlAllianceMarkInfo;
        [Tooltip("PnlTroopOverview CanvasGroup")]
        public CanvasGroup troopOverviewCG;
        [Tooltip("PnlTroopOverview Avatar")]
        public Image imgTroopOtherAvatar;
        [Tooltip("PnlTroopInfoBind")]
        public Transform pnlTroopBind;
        [Tooltip("PnlTroopInfo")]
        public Transform pnlTroopInfo;
        [Tooltip("PnlTroopInfo.BtnDetail")]
        public Button btnTroopDetail;
        [Tooltip("PnlMainInfo")]
        public Transform pnlTroopMainInfo;
        [Tooltip("PnlUp")]
        public Transform pnlTroopInfoUp;
        [Tooltip("PnlPlayerName.Text")]
        public TextMeshProUGUI txtTroopName;
        [Tooltip("PnlDown")]
        public Transform pnlTroopInfoDown;
        [Tooltip("PnlDown.PnlOrigin.ImgBackground.TxtNumber")]
        public TextMeshProUGUI txtOrigin;
        [Tooltip("PnlDown.PnlTarget.ImgBackground.TxtNumber")]
        public TextMeshProUGUI txtTarget;
        [Tooltip("PnlTerrainBind")]
        public Transform pnlTerrainBind;
        public CanvasGroup terrainBindCG;
        [Tooltip("PnlTerrain.PnlInfo.TxtCoordinate")]
        public TextMeshProUGUI txtTerrainCoordinate;
        [Tooltip("PnlTerrain.PnlDescription.TxtDescription")]
        public TextMeshProUGUI txtTerrainDescription;
        [Tooltip("PnlAddAllianceMark")]
        public Transform pnlAddAllianceMark;
        [Tooltip("PnlAddAllianceMark CanvasGroup")]
        public CanvasGroup pnlAddAllianceMarkCG;
        [Tooltip("PnlAllianceMark")]
        public Transform pnlAllianceMark;
        [Tooltip("PnlAllianceMark.PnlInfo.TxtInfo")]
        public TextMeshProUGUI txtAllianceMarkInfo;
        [Tooltip("PnlAllianceMark.PnlMarkName")]
        public TMP_InputField ifMarkName;
        [Tooltip("PnlAllianceMark.PnlButtons.BtnConfirm")]
        public Button btnConfirm;
        [Tooltip("PnlAllianceMark.PnlButtons.BtnCancel")]
        public Button btnCancel;
        //[Tooltip("PnlTile.PnlMarchBind")]
        //public Transform pnlMarchDetail;       
        [Tooltip("PnlMainInfo.TxtInfo")]
        public TextMeshProUGUI txtMarkInfo;
        [Tooltip("TxtCoordinate")]
        public TextMeshProUGUI txtMarkCoordinate;
        [Tooltip("PnlPlayerInfo")]
        public Transform pnlPlayerInfo;
        [Tooltip("PnlPlayerInfo.ImgRelation")]
        public Image imgPlayerInfo;
        [Tooltip("PnlPlayerInfo Button")]
        public Button btnPlayerInfo;
        [Tooltip("PnlPlayerInfo.TxtAllianceName")]
        public TextMeshProUGUI txtAlliance;
        [Tooltip("PnlPlayerInfo.TxtFiledAllianceInfo")]
        public TextMeshProUGUI txtFieldAllianceInfo;
        [Tooltip("PnlPlayerInfo.TxtPlayerName")]
        public TextMeshProUGUI txtPlayer;
        [Tooltip("PnlPlayerInfo.TxtFallenInfo")]
        public TextMeshProUGUI txtFallenInfo;
        [Tooltip("PnlPlayerInfo.TxtFreeMan")]
        public TextMeshProUGUI txtFreeMan;
        [Tooltip("PnlPlayerInfo.TxtFreshProtection")]
        public Transform pnlFreshProtect;
        [Tooltip("PnlPlayerInfo.TxtAvoidPortect")]
        public Transform pnlAvoidProtect;
        [Tooltip("PnlPlayerInfo.TxtProtectTime")]
        public TextMeshProUGUI txtProtectTime;

        [Tooltip("PnlButtons CanvasGroup")]
        public CanvasGroup tileButtonsCG;
        [Tooltip("PnlLeft")]
        public Transform pnlLeft;
        [Tooltip("PnlLeft.PnlButtons")]
        public Transform pnlLeftButtons;
        [Tooltip("PnlRight")]
        public Transform pnlRight;
        [Tooltip("PnlRight.PnlButtons")]
        public Transform pnlRightButtons;
        [Tooltip("BtnMove.PnlContent.TxtTip")]
        public Transform pnlMoveTip;
        [Tooltip("PnlRetreat")]
        public Transform pnlRetreat;
        [Tooltip("BtnRetreat")]
        public CustomButton btnRetreat;
        [Tooltip("PnlBelow")]
        public Transform pnlBelow;
        [Tooltip("PnlBottom")]
        public Transform pnlBottom;
        public CanvasGroup bottomCG;
        [Tooltip("PnlLabel.TxtTroop")]
        public TextMeshProUGUI txtTroopLabel;
        [Tooltip("PnlBottom.PnlBlank")]
        public Transform pnlBottomBlank;
        [Tooltip("PnlOverViewBind.PnlCampaignPrompting")]
        public Transform pnlCampaignPromptingList;
        public CanvasGroup pnlCampaignPromptingCG;
        public Transform pnlCapturePointsList;
    }
}
