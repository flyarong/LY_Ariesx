using UnityEngine;
using UnityEngine.Events;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class AllianceApplyView : BaseView {
        private AllianceApplyViewModel viewModel;
        /* UI Members*/
        private TMP_InputField ifDescription;
        private TextMeshProUGUI txtContentLimit;
        private CustomButton btnSend;
        private CustomButton btnCancel;
        private AllianceApplyViewPreference viewPref;

        /********************************************************/
        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<AllianceApplyViewModel>();
            this.ui = UIManager.GetUI("UIAllianceWindows.PnlWindows.PnlApply");
            this.viewPref = this.ui.transform.GetComponent<AllianceApplyViewPreference>();
            this.viewPref.ifDescription.characterLimit =
                LocalConst.GetLimit(LocalConst.ALLIANCE_APPLY);
            this.viewPref.ifDescription.onValueChanged.AddListener(this.OnIfDescriptionValueChange);
            
            this.viewPref.btnCancel.onClick.AddListener(this.OnBtnCancelClick);
            this.viewPref.btnSend.onClick.AddListener(this.OnBtnSendClick);
        }


        private void OnIfDescriptionValueChange(string value) {
            this.viewPref.txtContentLimit.text =
                string.Concat(value.Length, "/", LocalConst.GetLimit(LocalConst.ALLIANCE_APPLY));
        }

        private void OnBtnCancelClick() {
            this.viewModel.HideWindow();
        }

        private void OnBtnSendClick() {
            this.viewModel.Description = this.viewPref.ifDescription.text;
            this.viewModel.ApplyWithMessage();
        }

        protected override void OnVisible() {
            this.viewPref.ifDescription.text = string.Empty;
            //this.viewPref.txtContentLimit.text = string.Concat("0", "/", LocalConst.GetLimit(LocalConst.ALLIANCE_APPLY));
            this.OnIfDescriptionValueChange(
                LocalManager.GetValue(LocalHashConst.apply_alliance_default));
        }
    }
}
