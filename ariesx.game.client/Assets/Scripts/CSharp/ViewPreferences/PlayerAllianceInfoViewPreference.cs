using UnityEngine;
using UnityEngine.Events;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class PlayerAllianceInfoViewPreference : BaseViewPreference {
        [Tooltip("PnlPlayerAllianceInfo")]
        public Transform pnlPlayerAllianceInfo;
        [Tooltip("BtnBackground")]
        public Button btnBackground;
        [Tooltip("PnlName.TxtName")]
        public TextMeshProUGUI txtName;
        [Tooltip("PnlAvatar")]
        public Image imgAvatar;
        [Tooltip("PnlInflunce.PnlInflunce.TxtInflunce")]
        public TextMeshProUGUI txtInflunce;
        [Tooltip("PnlAlliance.PnlBoard.ImgHead")]
        public Image imgAlliance;
        [Tooltip("PnlAlliance.TxtAllianceName")]
        public TextMeshProUGUI txtAllianceName;
        [Tooltip("PnlAlliance.TxtOfficial")]
        public TextMeshProUGUI txtOfficial;
        [Tooltip("PnlBaseInfo.PnlBoard.TxtFallen")]
        public Transform pnlFallen;
        [Tooltip("PnlBaseInfo.TxtFallen")]
        public TextMeshProUGUI txtFallenAlliance;
        [Tooltip("PnlPlayerAllianceInfo.PnlDesc.TxtDesc")]
        public TextMeshProUGUI txtDesc;
        [Tooltip("PnlOperate.BtnRed")]
        public CustomButton btnRed;
        [Tooltip("PnlOperate.BtnRed.Content.TxtLabel")]
        public TextMeshProUGUI txtRedLabel;
        [Tooltip("PnlOperate.BtnGreen")]
        public CustomButton btnGreen;
        [Tooltip("PnlOperate.BtnGreen.Content.TxtLabel")]
        public TextMeshProUGUI txtGreenLabel;
        [Tooltip("PnlOperate.BtnMessage")]
        public CustomButton btnMessage;
    }
}
