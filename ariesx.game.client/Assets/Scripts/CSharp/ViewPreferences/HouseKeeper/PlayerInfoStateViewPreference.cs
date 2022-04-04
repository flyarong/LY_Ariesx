using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute
{
    public class PlayerInfoStateViewPreference : BaseViewPreference {
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList")]
        public Button pnlList;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlHead.PnlBaseInfo.PnlName.TxtName")]
        public TextMeshProUGUI txtName;

        public TextMeshProUGUI txtPlayerId;
        public TextMeshProUGUI txtWorldName;

        [Tooltip("PnlRoleInfo.PnlBoard")]
        public Button btnRoleAvatar;
        [Tooltip("PnlRoleInfo.PnlBoard")]
        public Image imgAvatar;
        [Tooltip("PnlRoleInfo.PnlBoard.TxtFallen")]
        public Transform pnlFallen;
        //[Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlHead.PnlBaseInfo.PnlBoard.TxtChangeAvatar")]
        //public Transform pnlChangeAvatar;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlHead.PnlBaseInfo.TxtFallenAlliance")]
        public TextMeshProUGUI txtFallenAlliance;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlHead.PnlBaseInfo.InputField")]
        public TMP_InputField ifName;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlHead.PnlBaseInfo.BtnEditName")]
        public Button btnEditName;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlHead.PnlBaseInfo.BtnChangeName")]
        public Button btnChangeName;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlHead.PnlBaseInfo.TxtTips")]
        public TextMeshProUGUI txtTips;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlHead.PnlAlliance.TxtAlliance")]
        public TextMeshProUGUI txtAlliance;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlHead.PnlAlliance.TxtRole")]
        public TextMeshProUGUI txtRole;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlHead.PnlAlliance.PnlBoard.ImgAllianceIcon")]
        public Image imgAllianceIcon;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlPower.PnlThings.PnlPowerValue.TxtPowerValue")]
        public TextMeshProUGUI txtPowerValueValue;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlPower.PnlThings.PnlTileCount.TxtTilecountValue")]
        public TextMeshProUGUI txtTileCountValue;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlPower.PnlThings.PnlTileLimit.TxtTileLimitValue ")]
        public TextMeshProUGUI txtTileLimitValue;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlResources.PnlThings.PnlLumber.TxtLumberValue")]
        public TextMeshProUGUI txtLumberValue;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlResources.PnlThings.PnlSteel.TxtSteelValue ")]
        public TextMeshProUGUI txtSteelValue;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlResources.PnlThings.PnlMarble.TxtMarbleValue ")]
        public TextMeshProUGUI txtMarbleValue;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlResources.PnlThings.PnlFood.TxtFoodValue ")]
        public TextMeshProUGUI txtFoodValue;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlTribute.PnlThings.PnlTributeValue.TxtTribute ")]
        public TextMeshProUGUI txtTribute;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlBasicInfo.PnlThings.PnlHeroCount.TxtHeroCountValue")]
        public TextMeshProUGUI txtHeroCountValue;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlBasicInfo.PnlThings.PnlArmyCount.TxtArmyCountValue")]
        public TextMeshProUGUI txtArmyCountValue;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlBasicInfo.PnlThings.PnlStronghold.TxtStrongholdValue")]
        public TextMeshProUGUI txtStrongholdValue;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlDescription.PnlContent.InputField")]
        public TMP_InputField ifDescription;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlDescription.PnlContent.Text")]
        public TextMeshProUGUI txtDescription;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlDescription.PnlContent.BtnEditDesc")]
        public Button btnEditDesc;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlResources.PnlLable.BtnResourcesDetail")]
        public Button btnResourcesDetail;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlTribute.PnlLable.BtnTributeDetail")]
        public Button btnTributeDetail;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlBasicInfo.PnlLable.BtnBasicInfoDetail")]
        public Button btnBasicInfoDetail;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlResources.PnlLable.PnlResourcesDetail")]
        public Transform pnlResourcesDetail;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlTribute.PnlLable.PnlTributeDetail")]
        public Transform pnlTributeDetail;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlBasicInfo.PnlLable.PnlBasicInfoDetail")]
        public Transform pnlBasicInfoDetail;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlResources.PnlThing")]
        public Button btnResInfo;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlDescription.PnlContent.BtnEditDesc.Image")]
        public Image imgEditDesc;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlDescription.PnlContent.btnCancelChangeDesc")]
        public Button btnCancelChangeDesc;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlPlayerInfo.PnlHint")]
        public RectTransform pnlHint;
        [Tooltip("UIHouseKeeper.PnlHouseKeeper.PnlChannel.PnlState.PnlList.PnlPlayerInfo.PnlHint.TxtDes")]
        public TextMeshProUGUI txtHint;
    }
}
