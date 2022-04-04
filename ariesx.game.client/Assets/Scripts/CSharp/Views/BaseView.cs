using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using Protocol;

/*
 1. 实现playing的互斥性，全局只能有一个UI执行show或者hide，
 若要同时hide和show，则执行HideImmediately或者ShowImmediately。
 2. 实现playing或者hiding的时候，有最上层的防误点mask。
 3. 实现UI分组，高级组永远不会被低级组覆盖。
 */

namespace Poukoute {
    public class BaseView : MonoBehaviour {
        [HideInInspector]
        public GameObject ui;
        [HideInInspector]
        public Canvas canvas;
        [HideInInspector]
        public CanvasScaler canvasScaler;
        [HideInInspector]
        public UIGroup group = UIGroup.Normal;
        // callback
        public UnityAction beforeShowCallback;
        public UnityAction afterShowCallback;
        public UnityAction beforeHideCallback;
        public UnityAction afterHideCallback;

        [HideInInspector]
        public BaseViewPreference baseViewPreference;

        private GameObject playingObj;
        private static bool isPlaying;
        public static bool IsPlaying {
            get {
                return isPlaying;
            }
            set {
                if (isPlaying != value) {
                    isPlaying = value;
                    UIManager.SetMaskVisible(value);
                }
            }
        }

        private bool isVisible = false;
        public bool IsVisible {
            get {
                return this.isVisible;
            }
            protected set {

                if (this.isVisible != value) {
                    this.isVisible = value;
                    if (this.isVisible) {
                        this.OnVisible();
                    } else {
                        this.OnInvisible();
                    }
                }
            }
        }

        //private bool isFakeBackground = false;
        private bool isUIInited = false;
        public bool IsUIInit {
            get {
                return this.isUIInited;
            }
        }
        private bool isSetRender = false;

        public void InitUI() {
            if (!this.isUIInited) {
                this.isUIInited = true;
                this.OnUIInit();
                this.baseViewPreference = this.ui.GetComponent<BaseViewPreference>();
                this.canvas = this.ui.GetComponent<Canvas>();
                this.canvasScaler = this.ui.GetComponent<CanvasScaler>();
                if (this.canvas != null && this.canvasScaler != null) {
                    this.SetCamera();
                    this.canvasScaler.matchWidthOrHeight = UIManager.AdaptiveParam;
                }
            }
        }

        public virtual void SetCamera() {
            GameHelper.SetCanvasCamera(this.canvas);
        }

        public virtual void PlayShow(UnityAction action, bool needHideBack, float delay = 0) {
            this.InitUI();
            if (this.isVisible || BaseView.isPlaying) {
                return;
            }
            GameObject uiObject = this.baseViewPreference.showObj == null ?
                                  this.ui : this.baseViewPreference.showObj;
            IsPlaying = true;
            this.beforeShowCallback.InvokeSafe();
            this.beforeShowCallback = null;
            this.ShowBase(needHideBack);
            action.InvokeSafe();
            if (this.baseViewPreference.showObj == null) {
                this.baseViewPreference.showObj = this.ui;
            } else {
                UIManager.SetUIVisible(uiObject, true);
            }
            this.playingObj = uiObject;

            AnimationManager.Animate(uiObject, "Show", delay, () => {
                this.afterShowCallback.InvokeSafe();
                this.afterShowCallback = null;
                IsPlaying = false;
            });
        }

        public virtual void PlayShow() {
            this.PlayShow(null, false);
        }

        public virtual void PlayShow(float delay) {
            this.PlayShow(null, false, delay);
        }

        public virtual void PlayShow(UnityAction action) {
            this.PlayShow(action, false);
        }

        public virtual void PlayShow(bool needHideBack) {
            this.PlayShow(null, needHideBack);
        }

        public virtual void PlayHide(UnityAction action = null) {
            if (!this.isUIInited || !this.isVisible || BaseView.isPlaying) {
                if (BaseView.isPlaying) {
                    Debug.LogWarning("Is Playing");
                }
                return;
            }

            GameObject uiObject = this.baseViewPreference.showObj == null ?
                                  this.ui : this.baseViewPreference.showObj;

            this.beforeHideCallback.InvokeSafe();
            this.beforeHideCallback = null;
            action.InvokeSafe();
            IsPlaying = true;
            if (uiObject == null) {
                uiObject = this.ui;
            }
            if (this.isSetRender) {
                this.isSetRender = false;
                UIManager.ShowFakeBack(false);
            }
            AnimationManager.Animate(uiObject, "Hide", () => {
                IsPlaying = false;
                if (uiObject != this.ui) {
                    UIManager.SetUIVisible(uiObject, false);
                }
                this.HideBase();
                this.afterHideCallback.InvokeSafe();
                this.afterHideCallback = null;
            });
        }

        public virtual void PlayHide() {
            this.PlayHide(null);
        }

        public virtual void Show(bool needHideBack = false, UnityAction callback = null) {
            if (this.isVisible) {
                return;
            }
            this.InitUI();
            callback.InvokeSafe();
            this.beforeShowCallback.InvokeSafe();
            this.beforeShowCallback = null;
            this.ShowBase(needHideBack);
            this.afterShowCallback.InvokeSafe();
            this.afterShowCallback = null;
        }

        public virtual void Hide(UnityAction callback = null) {
            if (!this.isUIInited || !this.isVisible) {
                return;
            }
            callback.InvokeSafe();
            this.beforeHideCallback.InvokeSafe();
            this.beforeHideCallback = null;

            if (this.isSetRender) {
                this.isSetRender = false;
                UIManager.ShowFakeBack(false);
            }
            this.HideBase();
            this.afterHideCallback.InvokeSafe();
            this.afterHideCallback = null;
        }

        private void ShowBase(bool needHideBack) {
            UIManager.ShowUIAtTop(this.ui, this);
            if (needHideBack) {
                this.isSetRender = true;
                UIManager.ShowFakeBack(true);
            }
            this.IsVisible = true;
        }

        private void HideBase() {
            UIManager.HideUI(this.ui, this);
            this.IsVisible = false;
        }

        public void StopAnimation() {
            if (this.playingObj != null) {
                AnimationManager.Finish(this.playingObj, false);
            }
        }

        public virtual void HideImmediatly(UnityAction action) {
            //this.InitUI();
            if (!this.isUIInited || !this.isVisible) {
                return;
            }
            action.InvokeSafe();
            this.StopAnimation();
            IsPlaying = false;
            this.Hide();
        }

        protected virtual void OnUIInit() { }

        protected virtual void OnVisible() { }

        protected virtual void OnInvisible() { }
    }
}
