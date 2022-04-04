using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public class BattlePrerenderView : MonoBehaviour {

        [SerializeField]
        private GameObject imgMap;
        [SerializeField]
        private GameObject imgVS;

        public void Play(UnityAction callback, UnityAction afterShowCallback) {
            this.imgVS.GetComponent<CanvasGroup>().alpha = 1;
            AnimationManager.Animate(this.imgVS, "Show", finishCallback: () => {
                UIManager.Instance.ShowBattleRenderBackground();
                StartCoroutine(this.ShowFakeBackground(afterShowCallback));
                callback.InvokeSafe();
            });
        }

        private void HideVS() {
            AnimationManager.Animate(this.imgVS, "Hide", finishCallback: () => {
                UIManager.SetBattleAboveUIVisible(false);
            });
        }

        private IEnumerator ShowFakeBackground(UnityAction afterShowCallback) {
            yield return YieldManager.EndOfFrame;
            AnimationManager.Animate(this.imgMap, "Show", finishCallback: () => {
                afterShowCallback();
                HideVS();
            });
        }
    }
}
