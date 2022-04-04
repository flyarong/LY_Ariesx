using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class BuildEditViewPreference : BaseViewPreference {
        [Tooltip("UIBuildEditor.PnlButtons")]
        public Transform pnlButtons;
        [Tooltip("UIBuildEditor.PnlButtons.BtnOK")]
        public CustomButton btnOK;
        [Tooltip("UIBuildEditor.PnlButtons.BtnCancel")]
        public CustomButton btnCancel;
        [Tooltip("UIBuildEditor.PnlButton.BtnClose")]
        public CustomButton btnClose;
 
    }
}
