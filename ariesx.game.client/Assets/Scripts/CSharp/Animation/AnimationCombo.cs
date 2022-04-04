using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {

    public class AnimationCombo : MonoBehaviour {
        public Dictionary<string, AnimationParam> animationDict =
            new Dictionary<string, AnimationParam>();
        [HideInInspector]
        public UnityEvent onEnd = new UnityEvent();
        public AnimationParam GetAnimation(string name) {
            AnimationParam animationParam = null;
            if (this.animationDict.TryGetValue(name, out animationParam)) {
                return animationParam;
            } else {
                // Debug.LogWarning("There is no such animtion {0} in {1}", name, this.gameObject.name);
                return null;
            }
        }

        void Awake() {
            foreach (AnimationParam item in this.GetComponents<AnimationParam>()) {
                this.animationDict[item.animationName] =  item;
            }
        }

        //void OnEnable() {
        //    AnimationManager.Animate(this.gameObject, "Show", null);
        //}

        //void OnDisable() {
        //    AnimationManager.Animate(this.gameObject, "Hide", null);
        //}


        public void OnEnd() {
            this.onEnd.Invoke();
        }
    }
}
