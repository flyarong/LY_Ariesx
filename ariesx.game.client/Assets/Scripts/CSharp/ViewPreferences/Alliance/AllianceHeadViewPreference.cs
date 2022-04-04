using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceHeadViewPreference : BaseViewPreference {
        public CustomButton btnDissolve;
        public CustomButton btnLeave;
        public CustomButton btnMessage;
        public CustomButton btnSetting;
        public CustomButton btnJoin;
        public CustomButton btnInfo;
        public Transform pnlCoolDown;
        public TextMeshProUGUI txtCoolDownTime;
        public Image imgLogo;
        [Tooltip("Alliance Name")]
        public TextMeshProUGUI txtName;
        //public TextMeshProUGUI txtLevel;
        public Transform pnlAlyExperience;
        public Slider alyExperienceSli;
        public TextMeshProUGUI txtAlyExperience;
        public TextMeshProUGUI txtDesc;
        public TextMeshProUGUI txtInflunce;
        public TextMeshProUGUI txtMembers;
        public Transform pnlResource;
        public TextMeshProUGUI txtLumber;
        public TextMeshProUGUI txtMarble;
        public TextMeshProUGUI txtSteel;
        public TextMeshProUGUI txtFood;
    }
}
