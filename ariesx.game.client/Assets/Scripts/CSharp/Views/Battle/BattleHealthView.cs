using Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class BattleHealthView : MonoBehaviour {
        //private GameObject ui;
        public CustomSlider sldHealthBg;
        public Slider sldHealth;
        public CustomSlider sldShieldBg;
        public Slider sldShield;
        public GameObject halo;

        void Awake() {
            //this.sldHealth.onValueChanged.AddListener(
            //    this.OnHealthValueChange
            //);

            //this.sldShield.onValueChanged.AddListener(
            //    this.OnShieldValueChange
            //);

            this.sldShieldBg.gameObject.SetActiveSafe(false);
            this.ResetShield();
        }

        public int GetHealth() {
            return Mathf.RoundToInt(this.sldHealth.value);
        }

        public void SetHealth(int value) {
            this.sldHealthBg.value =
            this.sldHealthBg.maxValue =
            this.sldHealth.value =
            this.sldHealth.maxValue = value;
        }

        public void AddShield(int amount) {
            this.sldShieldBg.gameObject.SetActiveSafe(true);
            this.sldShield.maxValue += amount;
            this.sldShieldBg.maxValue += amount;
            this.sldShield.value = this.sldShield.value + amount;
            this.sldShieldBg.value = this.sldShieldBg.value + amount;
        }

        // To do: remove sheld;
        public void RemoveShield(int amount, UnityAction callback) {
            this.sldShield.value += amount;
            this.StartCoroutine(this.RemoveShieldBg(amount, callback));
        }

        public IEnumerator RemoveShieldBg(int amount, UnityAction callback) {
            yield return YieldManager.GetWaitForSeconds(0.2f);
            this.sldShieldBg.ChangeTo(this.sldShield.value, () => {
                if (Mathf.RoundToInt(this.sldShieldBg.value) == 0) {
                    callback.InvokeSafe();
                    this.sldShieldBg.gameObject.SetActiveSafe(false);
                    this.ResetShield();
                }
            }, inertia: false, duration: 0.4f);
        }

        public void RemoveHealth(int amount, UnityAction callback) {
            if (this.halo.gameObject != null)
                this.halo.gameObject.SetActive(true);
            this.sldHealth.value += amount;
            this.StartCoroutine(this.RemoveHealthBg(amount, callback));
        }

        private IEnumerator RemoveHealthBg(int amount, UnityAction callback) {
            yield return YieldManager.GetWaitForSeconds(0.2f);
            this.sldHealthBg.ChangeTo(this.sldHealthBg.value + amount, () => {
                this.halo.gameObject.SetActive(false);
                callback.InvokeSafe();
            }, inertia: false, duration: 0.4f);

        }

        private void ResetShield() {
            this.sldShieldBg.maxValue = 0;
            this.sldShieldBg.value = 0;
            this.sldShield.maxValue = 0;
            this.sldShield.value = 0;
        }
    }
}
