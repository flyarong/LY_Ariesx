using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class AllianceCreateViewPreference : BaseViewPreference {
        [Tooltip("PnlCreate.PnlName TMP_InputField")]
        public TMP_InputField ifName;
        [Tooltip("PnlCreate.PnlName.Placeholder")]
        public TextMeshProUGUI txtNamePlaceholder;
        [Tooltip("PnlCreate.PnlLogo.PnlBoard.Image")]
        public Image imgLogo;
        [Tooltip("PnlCreate.PnlLogo.BtnLogo")]
        public CustomButton btnEditLogo;
        [Tooltip("PnlCreate.PnlDesc TMP_InputField")]
        public TMP_InputField ifDescription;
        [Tooltip("PnlCreate.PnlDesc.Placeholder")]
        public TextMeshProUGUI txtDescPlaceholder;

        [Tooltip("PnlCreate.PnlCondition.PnlType")]
        public TypeChooseView typeChoose;
        [Tooltip("PnlCreate.PnlCondition.PnlInflunce")]
        public TypeChooseView influnceChoose;

        [Tooltip("PnlCreate.PnlConfirm")]
        public Transform pnlConfirm;
        [Tooltip("PnlCreate.PnlConfirm.TxtCost")]
        public TextMeshProUGUI txtCost;
        [Tooltip("PnlCreate.PnlConfirm.BtnCreate")]
        public CustomButton btnCreate;
        [Tooltip("PnlCreate.PnlConfirm.BtnCreate.PnlContent.PnlCooldown")]
        public Transform pnlCooldown;
        [Tooltip("PnlCreate.PnlConfirm.BtnCreate.PnlContent.PnlCooldown.TxtCooldown")]
        public TextMeshProUGUI txtCooldownTime;
        [Tooltip("PnlCreate.PnlLanguage.PnlChangelLanguage.BtnChange")]
        public CustomButton btnChangeLanguage;
        [Tooltip("PnlCreate.PnlLanguage.PnlChangelLanguage.TxtLanguage")]
        public TextMeshProUGUI txtLanguage;
        [Tooltip("PnlCreate.PnlCondition.PnlInflunce.PnlType.BtnLeft")]
        public Button btnInflunceLeftBtn;
        [Tooltip("PnlCreate.PnlCondition.PnlInflunce.PnlType.BtnLeft.ImgLeft")]
        public Image imgInflunceLeftArrow;
        [Tooltip("PnlCreate.PnlCondition.PnlInflunce.PnlType.BtnRight")]
        public Button btnInflunceRightBtn;
        [Tooltip("PnlCreate.PnlCondition.PnlInflunce.PnlType.BtnLeft.ImgRight")]
        public Image imgInfluncelRightArrow;
        [Tooltip("PnlCreate.PnlCondition.PnlInflunce.PnlType.BtnLeft.ImgRight")]
        public TextMeshProUGUI txtInflunce;
    }
}
