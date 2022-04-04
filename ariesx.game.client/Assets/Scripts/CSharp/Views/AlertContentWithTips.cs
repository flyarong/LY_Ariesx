using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Poukoute;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class AlertContentWithTips : MonoBehaviour {
        #region UI element
        [SerializeField]
        private Button btnBackground;
        [SerializeField]
        private Transform pnlAlert;
        [SerializeField]
        private CanvasGroup alertCanvasGroup;
        [SerializeField]
        private TextMeshProUGUI txtTitle;
        [SerializeField]
        private Transform pnlContent;
        [SerializeField]
        private TextMeshProUGUI txtContent;
        [SerializeField]
        private Transform pnlTips;
        [SerializeField]
        private TextMeshProUGUI txtTips;
        [SerializeField]
        private HorizontalLayoutGroup buttonsHorLayoutGroup;
        [SerializeField]
        private CustomButton btnYes;
        [SerializeField]
        private TextMeshProUGUI txtBtnYes;
        [SerializeField]
        private Image imgBtnYes;
        [SerializeField]
        private CustomButton btnNo;
        [SerializeField]
        private TextMeshProUGUI txtBtnNo;
        [SerializeField]
        private Image imgBtnNo;
        [SerializeField]
        private CustomButton btnInfo;
        [SerializeField]
        private TextMeshProUGUI txtInfoTitle;
        [SerializeField]
        private Transform pnlNotice;
        [SerializeField]
        private TextMeshProUGUI txtNotice;
        #endregion

        TextAlignmentOptions txtTipsAlignment = TextAlignmentOptions.Center;
        private string Content {
            set {
                bool hasContent = !value.CustomIsEmpty();
                this.pnlContent.gameObject.SetActiveSafe(hasContent);
                if (hasContent) {
                    this.txtContent.text = value;
                }
            }
        }

        private string Tips {
            set {
                bool hasTips = !value.CustomIsEmpty();
                this.pnlTips.gameObject.SetActiveSafe(hasTips);
                if (hasTips) {
                    this.txtTips.alignment = this.txtTipsAlignment;
                    this.txtTips.text = value;
                }
            }
        }

        private string Notice {
            set {
                bool hasNotice = !value.CustomIsEmpty();
                this.pnlNotice.gameObject.SetActiveSafe(hasNotice);
                if (hasNotice) {
                    this.txtNotice.text = value;
                }
            }
        }

        public void ShowConfirm(string title, UnityAction onYes, TextAlignmentOptions txtTipsAlignment,
            UnityAction onNo = null, string content = "",
            string tips = "", string notice = "",
            bool canHide = true, string txtYes = default(string),
            string txtNo = default(string)) {
            this.txtTipsAlignment = txtTipsAlignment;
            UIManager.SetUICanvasGroupEnable(this.alertCanvasGroup, true);
            AnimationManager.Animate(this.pnlAlert.gameObject, "Show", null);
            this.btnBackground.gameObject.SetActiveSafe(true);
            this.btnBackground.onClick.RemoveAllListeners();
            if (canHide) {
                this.btnBackground.onClick.AddListener(this.HideAlertPnl);
            }
            this.txtTitle.text = title;
            this.SetAlertTextDetail(content, tips, notice);

            this.btnInfo.gameObject.SetActiveSafe(false);
            if (txtYes.CustomIsEmpty()) {
                this.txtBtnYes.gameObject.SetActive(false);
                this.imgBtnYes.gameObject.SetActive(true);
            } else {
                this.txtBtnYes.gameObject.SetActive(true);
                this.imgBtnYes.gameObject.SetActive(false);
                this.txtBtnYes.text = txtYes;
            }
            this.btnYes.gameObject.SetActiveSafe(true);
            this.btnYes.onClick.RemoveAllListeners();
            this.btnYes.onClick.AddListener(
                () => {
                    this.HideAlertPnl();
                    onYes.InvokeSafe();
                }
            );
            if (onNo != null) {
                if (txtNo.CustomIsEmpty()) {
                    this.txtBtnNo.gameObject.SetActive(false);
                    this.imgBtnNo.gameObject.SetActive(true);
                } else {
                    this.txtBtnNo.gameObject.SetActive(true);
                    this.imgBtnNo.gameObject.SetActive(false);
                    this.txtBtnNo.text = txtNo;
                }
                this.btnNo.gameObject.SetActiveSafe(true);
                this.btnNo.onClick.RemoveAllListeners();
                this.btnNo.onClick.AddListener(
                    () => {
                        this.HideAlertPnl();
                        onNo.InvokeSafe();
                    }
                );
            } else {
                this.btnNo.gameObject.SetActiveSafe(false);
            }
            this.ResetButtonsLayout();
        }

        public void ShowAlert(string content, string tips = "", string notice = "",
            string btnInfoLabel = "ok", UnityAction onInfo = null) {
            UIManager.ShowUI(this.pnlAlert.gameObject);
            AnimationManager.Animate(this.pnlAlert.gameObject, "Show", null);
            this.btnBackground.gameObject.SetActiveSafe(true);
            this.btnBackground.onClick.RemoveAllListeners();
            this.btnBackground.onClick.AddListener(this.HideAlertPnl);
            this.txtTitle.text = LocalManager.GetValue(LocalHashConst.notice_title_warning);
            this.SetAlertTextDetail(content, tips, notice);

            this.btnNo.gameObject.SetActiveSafe(false);
            this.btnYes.gameObject.SetActiveSafe(false);
            this.btnInfo.gameObject.SetActiveSafe(true);
            this.txtInfoTitle.text = btnInfoLabel;
            this.btnInfo.onClick.RemoveAllListeners();
            this.btnInfo.onClick.AddListener(() => {
                onInfo.InvokeSafe();
                this.HideAlertPnl();
            });
            this.ResetButtonsLayout();
        }

        private bool isHidingAlert = false;
        public void HideAlertPnl() {
            if (!this.isHidingAlert) {
                this.isHidingAlert = true;
                AnimationManager.Animate(this.pnlAlert.gameObject, "Hide", () => {
                    UIManager.SetUICanvasGroupEnable(this.alertCanvasGroup, false);
                    this.btnBackground.gameObject.SetActiveSafe(false);
                    this.isHidingAlert = false;
                });
            }
        }

        private void ResetButtonsLayout() {
            if (this.btnInfo.gameObject.activeSelf) {

            }
            this.buttonsHorLayoutGroup.spacing =
                this.btnInfo.gameObject.activeSelf ? 40 : 100;
            this.buttonsHorLayoutGroup.CalculateLayoutInputHorizontal();
            this.buttonsHorLayoutGroup.CalculateLayoutInputVertical();
            this.buttonsHorLayoutGroup.SetLayoutHorizontal();
            this.buttonsHorLayoutGroup.SetLayoutVertical();
        }

        private void SetAlertTextDetail(string content, string tips, string notice) {
            this.Content = content;
            this.Tips = tips;
            this.Notice = notice;
        }
    }
}
