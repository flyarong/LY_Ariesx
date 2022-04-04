using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public class LightweightTipsView : MonoBehaviour {
        public TextMeshProUGUI txtContent;
        public CanvasGroup canvasGroup;

        public void ShowLightweightTips(string content) {
            this.txtContent.text = content;
            this.canvasGroup.alpha = 1;
            AnimationManager.Animate(this.gameObject, "Play");
        }

        public void HideLightweightTips(UnityAction action) {
            AnimationManager.Animate(this.gameObject, "Hide", action);
        }
    }
}