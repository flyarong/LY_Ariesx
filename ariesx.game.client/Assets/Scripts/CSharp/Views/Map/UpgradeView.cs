using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/*** To do ***/
// make static function to change resources slider. 
/*************/

namespace Poukoute {
    public class UpgradeView : BaseView {
        private UpgradeViewModel viewModel;
        private UpgradeViewPreference viewPref;

        

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<UpgradeViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIUpgrade");
            this.viewPref = this.ui.transform.GetComponent<UpgradeViewPreference>();
        }

        public override void PlayShow() {
            base.PlayShow(() => {
            });
        }

        public override void PlayHide() {
            base.PlayHide();
        }


        public void ForceUpgradeAnimation() {
            this.viewPref.pnlForceAni.gameObject.SetActiveSafe(true);
            this.viewPref.pnlTitle.gameObject.SetActiveSafe(true);
            this.viewPref.imgIcon.sprite = RoleManager.GetHighDefinitionRoleAvatar();
            this.viewPref.txtForceAni.text =
                string.Format(LocalManager.GetValue("level"),
                (ForceRewardConf.GetForceLevel(RoleManager.GetForce()) - 1).ToString());
            if (ForceRewardConf.GetForceLevel(RoleManager.GetForce()) - 1 > 0) {
                this.viewPref.txtTitle.text =
                    LocalManager.GetValue("force_state_" +
                    (ForceRewardConf.GetForceLevel(RoleManager.GetForce()) - 1).ToString());
            } else {
                this.viewPref.txtTitle.text = string.Empty;
            }
            //this.viewPref.txtForceLevel.gameObject.SetActiveSafe(false);
            this.viewPref.pnlTitle.SetActiveSafe(false);
            this.viewPref.imgFilledFlashAnimation.fillAmount = 1;
            this.viewPref.imgFill.fillAmount = 1;
            Vector3 position = this.viewPref.pnlForceAni.GetComponent<RectTransform>().anchoredPosition;
            StartCoroutine(TitleMove());
            this.viewPref.objFilledFlashAnimation.SetActiveSafe(true);
            AudioManager.Play("show_power_up", AudioType.Show, AudioVolumn.High, isAdditive: true);
            AnimationManager.Animate(this.viewPref.pnlForceAni, "Show",
                start: this.viewPref.pnlForce.GetComponent
                <RectTransform>().anchoredPosition,
                target: position,
                space: PositionSpace.UI
                , finishCallback: () => {
                    this.viewPref.pnlTitle.SetActiveSafe(true);
                    StartCoroutine(StartFilledAnimation());
                });
        }

        private IEnumerator StartFilledAnimation() {
            yield return YieldManager.GetWaitForSeconds(0.1f);
            StartCoroutine(FillAnimation());
            StartCoroutine(ConsumeFill());
        }

        private IEnumerator TitleMove() {
            Vector3 titlePosition = this.viewPref.
                pnlTitleMove.GetComponent<RectTransform>().anchoredPosition;
            yield return YieldManager.GetWaitForSeconds(0.25f);
            AnimationManager.Animate(this.viewPref.pnlTitle, "Fade");
            AnimationManager.Animate(this.viewPref.pnlTitleMove, "Move", start: titlePosition,
           target: titlePosition + new Vector3(0, 10, 0)
           , finishCallback: () => {
               this.viewPref.pnlTitleMove.GetComponent<RectTransform>().anchoredPosition = titlePosition;
           }, space: PositionSpace.UI);
        }
        private IEnumerator ConsumeFill() {
            yield return YieldManager.GetWaitForSeconds(0.01f);
            this.viewPref.imgFilledFlashAnimation.fillAmount -=
                0.005f + (1 - this.viewPref.imgFilledFlashAnimation.fillAmount) * 0.05f;
            this.viewPref.imgFill.fillAmount -= 0.005f + (1 - this.viewPref.imgFill.fillAmount) * 0.05f;
            if (this.viewPref.imgFill.fillAmount > 0) {
                StartCoroutine(ConsumeFill());
            } else {
                yield return YieldManager.GetWaitForSeconds(0.3f);
                StartCoroutine(ChangeGrade());
                this.viewPref.objAllFlashAnimation.SetActiveSafe(true);
                this.viewPref.objFilledFlashAnimation.SetActiveSafe(false);
                AnimationManager.Animate(this.viewPref.imgFlashAll.gameObject,
                    "Flash", finishCallback: () => {
                        this.viewPref.btnForceAniBack.onClick.AddListener(this.OnForceAniBackClick);
                        StartCoroutine(EedFilledAnimation());
                    });
                AnimationManager.Animate(this.viewPref.pnlForceAni, "TurnBig");
            }
        }

        private IEnumerator EedFilledAnimation() {
            yield return YieldManager.GetWaitForSeconds(0.8f);
            this.viewPref.objEedFilledAnimation.SetActiveSafe(true);
        }

        private IEnumerator ChangeGrade() {
            yield return YieldManager.GetWaitForSeconds(0.45f);
            this.viewPref.objTitleAnimation.SetActiveSafe(true);
            this.viewPref.txtTitle.text =
                LocalManager.GetValue("force_state_" +
                (ForceRewardConf.GetForceLevel(RoleManager.GetForce())).ToString());
            //this.viewPref.txtForceLevel.gameObject.SetActiveSafe(true);
            this.viewPref.txtForceAni.text = string.Format(LocalManager.GetValue("level"), "<color=#1eff73>" +
               (ForceRewardConf.GetForceLevel(RoleManager.GetForce())).ToString());
            AnimationManager.Animate(this.viewPref.txtForceAni.gameObject, "NameShow");
        }

        private IEnumerator FillAnimation() {
            yield return YieldManager.GetWaitForSeconds(0.03f);
            AnimationManager.Animate(this.viewPref.pnlForceAni, "Beat",
                finishCallback: () => {
                    if (this.viewPref.imgFill.fillAmount > 0.3) {

                        StartCoroutine(FillAnimation());
                    }
                });
        }

        private void OnForceAniBackClick() {
            this.PlayHide();
            this.viewModel.ForceAniEndAction.InvokeSafe();
            this.viewPref.objEedFilledAnimation.SetActiveSafe(false);
            this.viewPref.objAllFlashAnimation.SetActiveSafe(false);
            this.viewPref.objTitleAnimation.SetActiveSafe(false);
            this.viewPref.imgIconFilledAnimation.SetActiveSafe(true);
            this.viewModel.ShowForceView = true;
            this.viewModel.CanShowForceAni = false;
            this.viewPref.btnForceAniBack.onClick.RemoveAllListeners();
            this.viewPref.pnlForceAni.gameObject.SetActiveSafe(false);
            this.viewPref.pnlTitle.gameObject.SetActiveSafe(false);
        }

      
    }
}
