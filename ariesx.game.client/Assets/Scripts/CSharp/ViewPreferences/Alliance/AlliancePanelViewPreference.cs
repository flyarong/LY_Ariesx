using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class AlliancePanelViewPreference : BaseViewPreference {
        public CustomButton btnInfo;
        public CustomButton btnLeaveOrDissolve;
        public TextMeshProUGUI txtLeaveOrDissolve;
        //public CustomButton btnLeave;
        public Image imgLogo;
        [Tooltip("Alliance Name")]
        public TextMeshProUGUI txtName;
        public TextMeshProUGUI txtLevel;
        public Transform pnlAlyExperience;
        public Slider alyExperienceSli;
        public TextMeshProUGUI txtAlyExperience;
        public TextMeshProUGUI txtDesc;
        public TextMeshProUGUI txtInflunce;
        public TextMeshProUGUI txtMembers;
        public TextMeshProUGUI txtLanguage;

        public Transform pnlTributeBonus;
        public TextMeshProUGUI txtTributeBonus;

        public Transform pnlResource;
        public TextMeshProUGUI txtLumber;
        public TextMeshProUGUI txtMarble;
        public TextMeshProUGUI txtSteel;
        public TextMeshProUGUI txtFood;
        public CustomButton btnMessage;
        public CustomButton btnSetting;
        public CustomButton btnMembersList;
    }
}
