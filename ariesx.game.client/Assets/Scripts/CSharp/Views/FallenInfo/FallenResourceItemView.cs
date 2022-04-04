using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class ResourceValueChange : UnityEvent<float, Resource> { }

    public class FallenResourceItemView : MonoBehaviour {
        private Transform ui;
        private Image resourceImg;
        private Slider slider;
        private TextMeshProUGUI txtMax;

        private float maxValue = -1;
        private float resourceValue = -1;
        private ResourceValueChange onResourceChange = new ResourceValueChange();
        public ResourceValueChange OnResourceChange {
            get {
                this.onResourceChange.RemoveAllListeners();
                return this.onResourceChange;
            }
        }

        public float ResourceValue {
            get {
                return this.resourceValue;
            }
            set {
                if (this.resourceValue != value) {
                    this.resourceValue = value;
                    this.OnResourceValueChange();
                }
            }
        }

        public float SliderValue {
            get {
                return this.slider.value;
            }
            set {
                if (this.slider.value != value) {
                    this.slider.value = value;
                }
            }
        }

        public float MaxValue {
            get {
                return this.maxValue;
            }
            set {
                if (this.maxValue != value) {
                    this.maxValue = value;
                    this.OnMaxValueChange();
                }
            }
        }

        public Resource Resource {
            get; set;
        }

        private void Awake() {
            this.ui = this.transform;
            this.resourceImg = this.ui.Find("ImgResource").GetComponent<Image>();
            this.slider = this.ui.Find("Slider").GetComponent<Slider>();
            this.slider.onValueChanged.RemoveAllListeners();
            this.slider.onValueChanged.AddListener(this.OnSliderValueChange);
            this.slider.value = 0;
            this.slider.minValue = 0;
            this.txtMax = this.ui.Find("TxtMax").GetComponent<TextMeshProUGUI>();
        }

        private void SetResourceImage() {
            this.resourceImg.sprite = ArtPrefabConf.GetSprite(
                SpritePath.resourceIconPrefix, this.Resource.ToString().ToLower());
        }

        private void OnResourceValueChange() {
            this.onResourceChange.Invoke(this.ResourceValue, this.Resource);

        }

        private void OnMaxValueChange() {
            this.slider.maxValue = this.MaxValue;
            this.txtMax.text = string.Concat(LocalManager.GetValue(LocalHashConst.fallen_pay_max),
                GameHelper.GetFormatNum((long)this.maxValue));
            this.SetResourceImage();
        }

        private void OnSliderValueChange(float value) {
            this.ResourceValue = value;
        }
    }

}
