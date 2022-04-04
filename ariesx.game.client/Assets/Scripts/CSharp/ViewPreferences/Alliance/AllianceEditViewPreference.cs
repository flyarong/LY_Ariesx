using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class AllianceEditViewPreference : BaseViewPreference {
        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlEdit.PnlEdit.PnlDesc TMP_InputField")]
        public TMP_InputField ifDescription;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlEdit.PnlEdit.PnlDesc.Placeholder")]
        public TextMeshProUGUI txtPlaceholder;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlEdit.PnlEdit.PnlLogo.PnlBoard.Image Image")]
        public Image imgLogo;
        public Button btnImageLogo;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlEdit.PnlEdit.PnlLogo.BtnView CustomButton")]
        public CustomButton btnEditLogo;

        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlEdit.PnlEdit.PnlCondition.PnlType TypeChooseView")]
        public TypeChooseView typeChoose;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlEdit.PnlEdit.PnlCondition.PnlInflunce TypeChooseView")]
        public TypeChooseView influnceChoose;

        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlEdit.PnlEdit.PnlButtons.BtnConfirm Button")]
        public Button btnOk;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlContent.PnlEdit.PnlEdit.PnlButtons.BtnCancel Button")]
        public Button btnCancel;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlEdit.PnlLanguage.PnlChangelLanguage.BtnChange")]
        public CustomButton btnChangeLanguage;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlEdit.PnlLanguage.PnlChangelLanguage.TxtLanguage")]
        public TextMeshProUGUI txtAllianceLanguage;
        //public Image[] imgInflunceChooses;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlEdit.PnlInflunce.PnlType.BtnLeft")]
        public Button btnInflunceLeftBtn;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlEdit.PnlInflunce.PnlType.BtnLeft.ImgLeft")]
        public Image imgInflunceLeftArrow;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlEdit.PnlInflunce.PnlType.BtnRight")]
        public Button btnInflunceRightBtn;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlEdit.PnlInflunce.PnlType.BtnLeft.ImgRight")]
        public Image imgInfluncelRightArrow;
        [Tooltip("UIAllianceWindows.PnlWindows.PnlEdit.PnlInflunce.PnlType.TxtType")]
        public TextMeshProUGUI txtInflunce;
    }
}
