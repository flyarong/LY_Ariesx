using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using Protocol;
using TMPro;

namespace Poukoute {
    public class MapResourceItemView: MonoBehaviour {
        #region UI elments
        [SerializeField]
        private Image resourceImg;
        [SerializeField]
        private CustomSlider slider;
        [SerializeField]
        private TextMeshProUGUI txtAmount;
        [SerializeField]
        private Image imgFill;
        [SerializeField]
        private Image imgFullHalo;
        #endregion

        private Resource resource;
        private float percent = 0;
        private float amount = 0;
        private float targetAmount = 0;
        private float costAmount = 0;

        public Resource Resource {
            get {
                return this.resource;
            }
            set {
                this.resource = value;
                this.OnNameChange();
            }
        }

        public float Amount {
            get {
                return this.amount;
            }
            set {
                this.amount = value;
                this.OnAmountChange();
            }
        }

        public float Value {
            get {
                return this.slider.value;
            }
            set {
                this.slider.value = value;
            }
        }

        public float TargetAmount {
            get {
                return this.targetAmount;
            }
            set {
                this.targetAmount = value;
                this.OnTargetAmountChange();
            }
        }

        public float CostAmount {
            get {
                return this.costAmount;
            }
            set {
                if (this.costAmount != value) {
                    this.costAmount = value;
                    this.OnAmountChange();
                }
            }
        }

        public float LeftAmount {
            get {
                return this.amount - this.costAmount;
            }
        }

        //private bool isFull;
        //public bool IsFull {
        //    get {
        //        return this.isFull;
        //    }
        //    set {
        //        if (isFull != value) {
        //            isFull = value;
        //            //this.onResourceFullEvent.InvokeSafe();
        //        }
        //    }
        //}

        //private UnityEvent onResourceFullEvent = new UnityEvent();
        //public UnityEvent OnResourceFullEvent {
        //    get {
        //        this.onResourceFullEvent.RemoveAllListeners();
        //        return this.onResourceFullEvent;
        //    }
        //}

        void Awake() {
            this.slider.onValueChanged.AddListener(this.OnSliderValueChange);
        }

        public GameObject GetResourceImg() {
            return this.resourceImg.gameObject;
        }

        private void OnNameChange() {
            string path = SpritePath.resourceIconPrefix + this.resource.ToString().ToLower();
            this.resourceImg.sprite = ArtPrefabConf.GetSprite(path);
            this.costAmount = 0;
            this.Amount = RoleManager.GetResource(this.resource);
            this.targetAmount = this.Amount;
        }

        private void OnAmountChange() {
            this.slider.StopChanging();
            this.slider.maxValue = RoleManager.GetResourceLimit(this.resource);
            this.slider.value = this.LeftAmount;
        }

        private void OnTargetAmountChange() {
            this.slider.ChangeTo(this.TargetAmount);
        }

        private void OnSliderValueChange(float value) {
            int valueInteger = Mathf.RoundToInt(value);
            //if (resource == Resource.Lumber) {
            //    Debug.LogError(resource + " " + value);
            //}
            if (valueInteger.ToString().CustomEquals(this.txtAmount.text)) {
                return;
            }
            //Debug.LogError("OnSliderValueChange " + resource + " " + value);
            if (resource == Resource.Gem) {
                this.txtAmount.text = valueInteger.ToString();
            } else {
                this.txtAmount.text = GameHelper.GetFormatNum(valueInteger);
            }
            this.percent = value / this.slider.maxValue;
            string imgFillTail = string.Empty;
            if (this.percent < 0.75) {
                imgFillTail = "green";
            } else {
                imgFillTail = (this.percent < 0.9f) ? "orange" : "red";
            }

            this.imgFill.sprite =
                ArtPrefabConf.GetSprite(SpritePath.resouceSliderPrefix, imgFillTail);
            this.imgFullHalo.gameObject.SetActiveSafe(this.percent >= 1);
        }
    }
}
