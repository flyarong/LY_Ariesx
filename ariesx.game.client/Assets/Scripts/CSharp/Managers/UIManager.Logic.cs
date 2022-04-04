using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {
    public partial class UIManager : MonoBehaviour {
        public static void ShowLightweightTips(string content, Transform transform, Vector3 offset) {
            Transform lightTips = transform.Find("PnlLightweightTips");
            if (lightTips == null) {
                lightTips = PoolManager.GetObject(PrefabPath.pnlLightweightTips, transform).transform;
            }
            LightweightTipsView lightweightTipsView = lightTips.GetComponent<LightweightTipsView>();
            lightweightTipsView.ShowLightweightTips(content);
            lightTips.localPosition += offset;
        }

        public static void HideLightweightTips(Transform transform) {
            Transform lightTips = transform.Find("PnlLightweightTips");
            if (lightTips != null) {
                LightweightTipsView lightweightTipsView =
                    lightTips.GetComponent<LightweightTipsView>();
                lightweightTipsView.HideLightweightTips(() => {
                    PoolManager.RemoveObject(lightTips.gameObject);
                });
            }
        }

        public static void ShowConfirm(
            string title, 
            string content,
            UnityAction onYes, 
            UnityAction onNo = null,
            string tips = "",
            string notice = "",
            bool canHide = true,
            TextAlignmentOptions txtTipsAlignment = TextAlignmentOptions.Center,
            string txtYes = default(string),
            string txtNo = default(string))
        {
            Instance.alertMessageWithTips.ShowConfirm(title, onYes, txtTipsAlignment, 
                onNo, content, tips, notice, canHide, txtYes, txtNo);
        }

        public static void ShowAlert(string content,
            string tips = "", string notice = "", string btnInfoLabel = "Ok", UnityAction onInfo = null) {
            Instance.alertMessageWithTips.ShowAlert(content, tips, notice, btnInfoLabel, onInfo);
        }

        public static void HideAlertPnl() {
            Instance.alertMessageWithTips.HideAlertPnl();
        }

        public static void ShowTip(string text, TipType type, float waitTime = 2f) {
            Instance.gameGlobalTips.ShowTip(text, type, waitTime);
        }

        public static void ShowWifiAlert() {
            Instance.gameGlobalTips.ShowWifiAlert();
        }

        public static void HideWifiAlert() {
            Instance.gameGlobalTips.HideWifiAlert();
        }

        public static void ShowNetCircle() {
            Instance.gameGlobalTips.ShowNetCircle();
        }

        public static void HideNetCircle() {
            Instance.gameGlobalTips.HideNetCircle();
        }

        public static void ShowLoading() {
            Instance.uiSplash.transform.Find("Loading").Find("PnlLogo").
                Find("ImgLogo" + LocalManager.Language).gameObject.SetActive(true);
            Instance.sldProgress.value = 0.1f;
        }

        public static void UpdateProgress(float progress, float duration = 1.5f, UnityAction callback = null) {
            progress = Mathf.Max(Instance.sldProgress.value, progress);
            if (duration == 0) {
                Instance.sldProgress.value = progress;
            } else {
                Instance.sldProgress.ChangeTo(progress, inertia: false, duration: duration, callback: callback);
            }
        }
        
        private void UpdateProgressText(float value) {
            Instance.txtProgress.text = Mathf.Min(100, Mathf.RoundToInt(value * 100)) + "%";
        }

        public static void HideLoading() {
            Destroy(Instance.uiSplash);
        }
    }
}
