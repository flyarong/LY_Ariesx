using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Poukoute {
    public enum ButtonType {
        Default,
        Close
    }

    public class CustomButton : Button, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler, IPointerClickHandler {
        private bool isPressing;
        private UnityEvent m_onPressing = null;
        public ButtonType m_btnType = ButtonType.Default;
        public UnityEvent onPressing {
            get {
                if (this.m_onPressing == null) {
                    m_onPressing = new UnityEvent();
                }
                return this.m_onPressing;
            }
        }

        public bool needExit = true;
        public bool interactable {
            get {
                return base.interactable;
            }
            set {
                if (base.interactable != value) {
                    base.interactable = value;
                    this.OnInteractableChange();
                }
            }
        }

        private bool grayable = false;
        public bool Grayable {
            get {
                return this.grayable;
            }
            set {
                if (this.grayable != value) {
                    this.grayable = value;
                    this.OnGrayableChange();
                }
            }
        }

        public string Text {
            set {
                this.pnlContent.transform.Find("Text").
                    GetComponent<TextMeshProUGUI>().text = value;
            }
        }

        public UnityEvent onSubmit = new UnityEvent();
        private float pressingTime;

        // UI
        public Transform pnlContent;
        private Image imgButton;
        private Outline outlineButton;
        private Color outlineColor;

        protected override void Awake() {
            base.Awake();
            this.grayable = false;
            this.pnlContent = this.transform.Find("PnlContent");
            if (pnlContent != null) {
                this.imgButton = this.pnlContent.GetComponent<Image>();
                Transform text = this.pnlContent.transform.Find("Text");
                if (text != null) {
                    this.outlineButton = text.GetComponent<Outline>();
                }
                if (this.outlineButton != null) {
                    this.outlineColor = this.outlineButton.effectColor;
                }
            }
        }

        private void UpdateAction() {
            if (this.isPressing) {
                pressingTime += Time.unscaledDeltaTime;
                if (pressingTime > 0.6f) {
                    if (this.m_onPressing != null) {
                        this.isPressing = false;
                        this.m_onPressing.Invoke();
                    }
                }
            }
        }

        private void Press() {
            this.onClick.Invoke();
        }

        private void OnInteractableChange() {
            if (this.interactable) {
                this.imgButton.material = null;
                if (this.outlineButton != null) {
                    this.outlineButton.effectColor = this.outlineColor;
                }
            } else {
                this.imgButton.material = PoolManager.GetMaterial(MaterialPath.matGray);
                if (this.outlineButton != null) {
                    this.outlineButton.effectColor = Color.grey;
                }
            }
        }

        private void OnGrayableChange() {
            this.interactable = true;
            if (this.grayable) {
                this.imgButton.material = PoolManager.GetMaterial(MaterialPath.matGray);
                if (this.outlineButton != null) {
                    this.outlineButton.effectColor = Color.grey;
                }
                Transform imgIconTrans = this.imgButton.transform.Find("Image");
                if (imgIconTrans != null) {
                    imgIconTrans.GetComponent<Image>().material = PoolManager.GetMaterial(MaterialPath.matGray);
                }
            } else {
                this.imgButton.material = null;
                if (this.outlineButton != null) {
                    this.outlineButton.effectColor = this.outlineColor;
                }
                Transform imgIconTrans = this.imgButton.transform.Find("Image");
                if (imgIconTrans != null) {
                    imgIconTrans.GetComponent<Image>().material = null;
                }
            }
            //Debug.LogError("OnGrayableChange " + this.grayable + " " + this.interactable);
        }

        public override void OnPointerClick(PointerEventData eventData) {
            if (this.interactable) {
                if (this.pnlContent != null) {
                    AnimationManager.Animate(this.pnlContent.gameObject,
                        "Click", this.Press, loop: false, isReverse: true);
                } else {
                    this.onClick.Invoke();
                }
                if (this.m_btnType == ButtonType.Default) {
                    AudioManager.Play(
                        AudioPath.actPrefix + "button",
                        AudioType.Action,
                        AudioVolumn.High
                    );
                } else {
                    AudioManager.Play(
                        AudioPath.actPrefix + "ui_hide",
                        AudioType.Action,
                        AudioVolumn.High
                    );
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData) {
            if (!this.isPressing) {
                this.isPressing = true;
                UpdateManager.Regist(UpdateInfo.CustomButton, this.UpdateAction);
            }
            pressingTime = 0;
            if (this.interactable && this.pnlContent != null) {
                AnimationManager.Animate(this.pnlContent.gameObject, "Click", null);
            }
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData) {
            this.isPressing = false;
            UpdateManager.Unregist(UpdateInfo.CustomButton, this.UpdateAction);
            this.pressingTime = 0;
            base.OnPointerUp(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData) {
            //this.isPointInside = true;
            if (this.isPressing && this.interactable && this.pnlContent != null) {
                AnimationManager.Animate(this.pnlContent.gameObject, "Click", null);
            }
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData) {
            //this.isPointInside = false;
            if (this.isPressing && this.interactable && this.pnlContent != null) {
                AnimationManager.Animate(this.pnlContent.gameObject,
                    "Click", null, loop: false, isReverse: true);
            }
            base.OnPointerExit(eventData);
        }
    }
}
