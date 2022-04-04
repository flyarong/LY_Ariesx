using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class UpgradeViewModel : BaseViewModel {

        private MapTopHUDViewModel parent;
        private UpgradeView view;

        public bool CanShowForceAni {
            get {
                return this.parent.CanShowForceAni;
            }
            set {
                this.parent.CanShowForceAni = value;
            }
        }

        public bool ShowForceView {
            get {
                return this.parent.ShowForceView;
            }
            set {
                this.parent.ShowForceView = value;
            }
        }

        public UnityAction ForceAniEndAction {
            get {
                return this.parent.ForceAniEndAction;
            }
            set {
                this.parent.ForceAniEndAction = value;
            }
        }

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapTopHUDViewModel>();
            this.view = this.gameObject.AddComponent<UpgradeView>();
        }

        public void Show(UnityAction finishBackAction) {
            if (!this.CanShowForceAni) {
                finishBackAction.InvokeSafe();
                return;
            }
            this.ForceAniEndAction = null;
            this.ForceAniEndAction += finishBackAction;
            if (!this.view.IsVisible) {
                this.view.PlayShow();
                StartCoroutine(this.DelayPlayUpGradeAnimation());
            }
        }

        private IEnumerator DelayPlayUpGradeAnimation() {
            yield return YieldManager.EndOfFrame;
            this.view.ForceUpgradeAnimation();
        }

        public void Hide() {
            this.view.PlayHide();
        }
    }
}
