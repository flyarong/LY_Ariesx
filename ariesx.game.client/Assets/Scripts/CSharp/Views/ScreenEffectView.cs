using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {

    public class ScreenEffectView : BaseView {
        private static ScreenEffectView self;
        private ScreenEffectViewPreference viewPref;
        private Material imgFrameMat;
        private UnityAction onAreaClick;

        void Awake() {
            self = this;
            this.ui = GameObject.Find("UIScreenEffect");
            this.viewPref = this.ui.transform.GetComponent<ScreenEffectViewPreference>();
            this.viewPref.canvas.worldCamera =
               GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
            this.viewPref.btnScreenClickable.onClick.AddListener(this.OnBtnScreenClickableClick);
            this.imgFrameMat = this.viewPref.imgFrame.material;
        }

        public void SetHighlightFrame(Transform nextTrans, 
            UnityAction AfterCallBack,Vector2 offsetMin, Vector2 offsetMax) {
            Transform nextOrigin = nextTrans.parent;
            int nextIndex = nextTrans.GetSiblingIndex();
            RectTransform nextRectTrans = nextTrans.GetComponent<RectTransform>();
            Transform originParent = nextTrans.parent;
            GameObject replaceObj = PoolManager.GetObject(PrefabPath.pnlFteEmpty, originParent);
            RectTransform replaceRectTrans = replaceObj.GetComponent<RectTransform>();
            LayoutElement replaceLayoutElement = replaceObj.GetComponent<LayoutElement>();
            replaceObj.transform.SetSiblingIndex(nextTrans.GetSiblingIndex());
            this.onAreaClick += () => {
                PoolManager.RemoveObject(replaceObj);
                nextTrans.SetParent(nextOrigin);
                nextTrans.localScale = Vector3.one;
                nextTrans.SetSiblingIndex(nextIndex);
                AfterCallBack.InvokeSafe();
            };
            GameHelper.CopyRectTransform(nextRectTrans, replaceRectTrans);
            replaceLayoutElement.preferredHeight = nextRectTrans.sizeDelta.y;
            nextTrans.SetParent(this.viewPref.pnlShow);
            //GameObject pnlScreenEffectRect = PoolManager.GetObject(PrefabPath.pnlFteRect, this.viewPref.pnlClickable);
            //RectTransform rectTransform = pnlScreenEffectRect.GetComponent<RectTransform>();
            //CustomButton btnRect = pnlScreenEffectRect.GetComponent<CustomButton>();
            this.viewPref.imgFrame.gameObject.SetActiveSafe(true);
            this.viewPref.imgFrame.transform.SetParent(nextTrans);
            this.viewPref.imgFrame.GetComponent<CanvasGroup>().alpha = 1;
            this.viewPref.imgBlack.GetComponent<CanvasGroup>().alpha = 1;
            RectTransform rectImgFrame = this.viewPref.imgFrame.GetComponent<RectTransform>();
            rectImgFrame.offsetMin = offsetMin;
            rectImgFrame.offsetMax = offsetMax;
            //this.viewPref.btnScreenClickable.gameObject.SetActiveSafe(true);
            this.viewPref.imgBlack.gameObject.SetActiveSafe(true);
            AnimationManager.Animate(this.viewPref.imgFrame.gameObject, "Focus",
                () => {
                    //this.viewPref.btnScreenClickable.gameObject.SetActiveSafe(false);
                    StartCoroutine(this.DelayEnd());
                });
            //btnRect.pnlContent = nextTrans;
            //btnRect.onClick.RemoveAllListeners();
            //btnRect.onClick.AddListener(() => {
            //   onAreaClick.InvokeSafe();
            //    GameHelper.ClearChildren(this.viewPref.pnlClickable);
            // });
            //rectTransform.position = nextTrans.position;
            //rectTransform.sizeDelta = nextRectTrans.rect.size;
            //this.onAreaMove.AddListener(() => {
            //    nextRectTrans.position =
            //    rectTransform.position = replaceObj.transform.position;
            //});
        }

        private void OnBtnScreenClickableClick() {
            AnimationManager.Stop(this.viewPref.imgFrame.gameObject);
            // this.viewPref.btnScreenClickable.gameObject.SetActiveSafe(false);
            StartCoroutine(this.DelayEnd());
        }

        private IEnumerator DelayEnd() {
            yield return YieldManager.GetWaitForSeconds(0.3f);
            AnimationManager.Animate(this.viewPref.imgFrame.gameObject, "Fade");
            AnimationManager.Animate(this.viewPref.imgBlack.gameObject, "Fade", () => {
                this.viewPref.imgFrame.transform.SetParent(this.viewPref.pnlShow);
                this.viewPref.imgFrame.gameObject.SetActiveSafe(false);
                this.viewPref.imgBlack.gameObject.SetActiveSafe(false);
                this.onAreaClick.InvokeSafe();
                this.onAreaClick = null;
            });
        }

        public void EndImmediately() {
            this.viewPref.imgFrame.transform.SetParent(this.viewPref.pnlShow);
            this.viewPref.imgFrame.gameObject.SetActiveSafe(false);
            this.viewPref.imgBlack.gameObject.SetActiveSafe(false);
            this.onAreaClick.InvokeSafe();
            this.onAreaClick = null;
        }
    }
}
