using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class ScreenEffectViewModel : BaseViewModel, IViewModel {
        private ScreenEffectView view;

        void Awake() {
            this.view = this.gameObject.AddComponent<ScreenEffectView>();
        }

        public void Show() {
            this.view.Show();
        }

        public void Hide() {
            this.view.Hide();
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        public void SetHighlightFrame(Transform nextTrans, UnityAction AfterCallBack,Vector2 offsetMin,Vector2 offsetMax) {
            this.view.SetHighlightFrame(nextTrans, AfterCallBack,offsetMin,offsetMax);
        }

        public void EndImmediately() {
            this.view.EndImmediately();
        }
        /***********************************/
    }
}
