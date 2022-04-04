using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace Poukoute {
    public class CustomContentSizeFitter : ContentSizeFitter {
        public UnityEvent onSetLayoutVertical = new UnityEvent();
        public UnityEvent onSetLayoutHorizontal = new UnityEvent();

        public override void SetLayoutVertical() {
            base.SetLayoutVertical();
            this.onSetLayoutVertical.Invoke();
            StartCoroutine(this.RemoveLayoutEvent());
        }

        public override void SetLayoutHorizontal() {
            base.SetLayoutHorizontal();
            this.onSetLayoutHorizontal.Invoke();
            StartCoroutine(this.RemoveLayoutEvent());
        }

        IEnumerator RemoveLayoutEvent() {
            //yield return new WaitForEndOfFrame();
            yield return YieldManager.EndOfFrame;
            this.onSetLayoutVertical.RemoveAllListeners();
            this.onSetLayoutHorizontal.RemoveAllListeners();
        }
    }
}