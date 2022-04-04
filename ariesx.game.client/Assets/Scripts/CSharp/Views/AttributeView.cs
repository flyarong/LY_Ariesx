using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Protocol;

namespace Poukoute {
    public class AttributeView : BaseView {
        #region UI Elements
        public Image imgIcon;
        public TextMeshProUGUI txtLable;
        public TextMeshProUGUI txtNumber;
        public TextMeshProUGUI txtAddNumber;
        public Transform pnlTip;
        public TextMeshProUGUI txtTip;
        public Button btnTip;
        public CanvasGroup canvasGroup;
        #endregion
        [HideInInspector]
        public int addValue;
        [HideInInspector]
        public float value;
        public UnityAction<AttributeView> OnTipShow = null;

        private bool isShowTip = false;
        public bool IsShowTip {
            get {
                return this.isShowTip;
            }
            set {
                if (this.isShowTip != value) {
                    this.isShowTip = value;
                    Vector2 position = this.txtLable.transform.GetComponent<RectTransform>().anchoredPosition;
                    if (this.pnlTip == null) {
                        return;
                    }
                    if (this.isShowTip) {
                        this.pnlTip.gameObject.SetActive(true);
                        AnimationManager.Animate(this.pnlTip.gameObject, "Show", Vector2.right * 130,
                            new Vector2(130, 45), null);
                    } else {
                        AnimationManager.Animate(this.pnlTip.gameObject, "Hide",
                            new Vector2(130, 45), Vector2.right * 130, () => {
                                this.pnlTip.gameObject.SetActive(true);
                            });
                    }
                    // this.onVisible.Invoke();
                }
            }
        }

        public string Name {
            get {
                return this.txtLable.text;
            }
            set {
                if (this.txtLable.text != value) {
                    this.txtLable.text = value;
                }
            }
        }

        public void Show(bool show) {
            canvasGroup.alpha = show ? 1 : 0;
        }

        public Sprite Icon {
            get {
                return this.imgIcon.sprite;
            }
            set {
                if (this.imgIcon.sprite != value) {
                    this.imgIcon.sprite = value;
                    if (value == null && this.imgIcon.gameObject.activeSelf) {
                        this.imgIcon.gameObject.SetActiveSafe(false);
                    } else if (!this.imgIcon.gameObject.activeSelf) {
                        this.imgIcon.gameObject.SetActiveSafe(true);
                        // To do: Handle the img size.
                    }
                }
            }
        }

        public string Value {
            get {
                return this.txtNumber.text;
            }
            set {
                if (this.txtNumber.text != value) {
                    this.txtNumber.text = value;
                }
            }
        }

        public string AddValue {
            get {
                return this.txtAddNumber.text;
            }
            set {
                if (this.txtAddNumber.text != value) {
                    this.txtAddNumber.text = value;
                }
            }
        }

        //public bool EnableClick {
        //    set {
        //        // this.GetComponent<Image>().raycastTarget = value;
        //    }
        //}

        public bool NeedShowTip {
            set {
                if (this.btnTip != null) {
                    this.btnTip.onClick.RemoveAllListeners();
                    if (value) {
                        //Debug.LogError("Set Tip");
                        this.btnTip.onClick.AddListener(this.OnClick);
                    }
                }

            }
        }

        public string Tip {
            set {
                if (this.txtTip != null) {
                    this.txtTip.text = value;
                }
            }
        }

        private void OnClick() {
            this.OnTipShow.InvokeSafe(this);
            this.IsShowTip = !this.IsShowTip;
        }
    }
}
