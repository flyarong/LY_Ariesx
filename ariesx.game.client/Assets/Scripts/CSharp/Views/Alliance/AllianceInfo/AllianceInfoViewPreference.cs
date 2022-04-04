using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceInfoViewPreference : BaseViewPreference {
        [Tooltip("UIAllianceInfo.BtnBackground")]
        public CustomButton btnBackground;
        [Tooltip("UIAllianceInfo.PnlAllianceInfo.PnlHead")]
        public Button btnClose;
        [Tooltip("UIAllianceInfo.PnlAllianceInfo.PnlInfoBtnList")]
        public CustomButton btnJoin;
        public CustomButton btnMember;
        [Tooltip("UIAllianceInfo.PnlAllianceInfo.PnlCoolDown")]
        public CanvasGroup coolDownCG;
        public TextMeshProUGUI txtCoolDownTime;
        [Tooltip("UIAllianceInfo.PnlAllianceInfo.PnlHead.TxtName")]
        public CustomButton btnInfo;
        public TextMeshProUGUI txtAllianceName;
        [Tooltip("UIAllianceInfo.PnlAllianceInfo.PnlHead.TxtDesc")]
        public TextMeshProUGUI txtDesc;
        [Tooltip("UIAllianceInfo.PnlAllianceInfo.PnlHead.PnlLogo Image")]
        public Image imgAllianceEmblem;
        public TextMeshProUGUI txtInflunce;
        public TextMeshProUGUI txtMembers;
        public TextMeshProUGUI txtLanguage;

        [Tooltip("UIAllianceInfo.PnlAllianceInfo.PnlHead.TxtLevel")]
        public TextMeshProUGUI txtAllianceLevel;
        [Tooltip("UIAllianceInfo.PnlAllianceInfo.PnlHead.PnlAllianceExp")]
        public Transform pnlAllianceExp;
        public Slider sliderAllianceExp;
        public TextMeshProUGUI txtExperience;
        [Tooltip("UIAllianceInfo.PnlAllianceInfo.PnlResources")]
        //public Transform pnlResources;
        public TextMeshProUGUI txtLumber;
        public TextMeshProUGUI txtMarble;
        public TextMeshProUGUI txtSteel;
        public TextMeshProUGUI txtFood;

    }
}
