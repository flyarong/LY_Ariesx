using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using Protocol;

namespace Poukoute {
    public class CustomSlider : CustomBaseSlider {
        public float targetValue;
        [SerializeField]
        private float currentValue;
        public new float value {
            get {
                return this.currentValue;
            }
            set {
                base.value = Mathf.Min(value, this.maxValue);
                if (this.currentValue != value) {
                    this.currentValue = value;
                    this.onValueChanged.Invoke(this.currentValue);
                }
            }
        }
        private UnityAction callback;
        private float changing = 0f;
        public bool isChanging = false;
        private bool inertia = true;
        private float inertiaAdd = 0.02f;
        private float lerp = 3f;

        public class CustomSliderEvent : UnityEvent<float> { }

        public new CustomSliderEvent onValueChanged = new CustomSliderEvent();

        protected override void Awake() {
            base.Awake();
            this.currentValue = base.value;
        }

        private void UpdateAction() {
            if (this.isChanging) {
                if (this.inertia && ((this.value - this.targetValue) / this.changing).Abs() > 0.05f) {
                    this.value = Mathf.Lerp(
                        this.value, this.targetValue, this.lerp * Time.unscaledDeltaTime
                    );
                    return;
                }
                if (!this.inertia && (this.targetValue - this.value).Abs() > inertiaAdd.Abs()) {
                    this.value += inertiaAdd;
                    return;
                }

                this.value = this.targetValue;
                this.isChanging = false;
                callback.InvokeSafe();
                UpdateManager.Unregist(UpdateInfo.CustomSlider, this.UpdateAction);
            }

        }


        public void ChangeTo(float value, UnityAction callback = null, bool inertia = true,
            float duration = 1.4f) {
            UpdateManager.Regist(UpdateInfo.CustomSlider, this.UpdateAction);
            this.callback = callback;
            this.targetValue = Mathf.Max(0, value);
            this.changing = this.targetValue - this.value;
            this.isChanging = true;
            this.inertia = inertia;
            if (!this.inertia) {
                this.inertiaAdd = (targetValue - this.value) / duration * 0.0167f;
            }
        }

        public void StopChanging() {
            this.value = this.targetValue;
            this.isChanging = false;
            UpdateManager.Unregist(UpdateInfo.CustomSlider, this.UpdateAction);
        }
    }
}
