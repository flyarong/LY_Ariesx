using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {

    public class BattleEffectPositionView : MonoBehaviour {

        [HideInInspector]
        public Transform effectRoot;
        [HideInInspector]
        public Vector3 offset;
        [HideInInspector]
        public string heroId = string.Empty;
        private const string pattern = "Battle/Skills/{0}/{1}";
        [HideInInspector]
        private const float destroyOffset = 0.05f;

        [HideInInspector]
        public List<BattleEffectAction> effectList = new List<BattleEffectAction>();
        [HideInInspector]
        public List<Transform> rootList = new List<Transform>();

        [HideInInspector]
        public float failedStopDelay = 0;
        [HideInInspector]
        public float victorStopDelay = 0;

        public Transform Root {
            get {
                if (this.effectRoot == null) {
                    return this.transform;
                } else {
                    return this.effectRoot;
                }
            }
            set {
                if (value != null) {
                    this.transform.position = value.position;
                }
                this.effectRoot = value;
            }
        }

        void Awake() {
            this.offset = this.transform.position - this.Root.position;
        }

        public void Play() {
            foreach (BattleEffectAction effect in this.effectList) {
                effect.Play();
            }
        }

        public void Stop(float delay) {
            base.StartCoroutine(this.DelayStop(delay));
        }

        public void Clear() {
            foreach (BattleEffectAction effectAction in this.effectList) {
                if (effectAction == null) {
                    continue;
                }
                effectAction.Stop();
                PoolManager.RemoveObject(effectAction.gameObject, PoolType.Battle);
            }
            this.effectList.Clear();
        }

        private IEnumerator DelayStop(float delay) {
            yield return YieldManager.GetWaitForSeconds(delay);
            foreach (BattleEffectAction effect in this.effectList) {
                PoolManager.RemoveObject(effect.gameObject, PoolType.Battle);
            }
        }

        public GameObject Create(string path, Transform root = null) {
            if (root == null) {
                root = this.Root;
            }
            GameObject effectObj = PoolManager.GetObject(path, root);
            if (effectObj != null) {
                BattleEffectAction effectAction = effectObj.GetComponent<BattleEffectAction>();
                effectObj.transform.position = root.position;
                if (!effectAction.loop) {
                    this.StartCoroutine(this.RemoveEffect(
                        effectAction.duration + destroyOffset,
                        effectAction
                    ));
                }
                effectAction.Play();
                this.effectList.Add(effectAction);
            }
            return effectObj;
        }

        private IEnumerator RemoveEffect(float second, BattleEffectAction effectAction) {
            yield return YieldManager.GetWaitForSeconds(second);
            if (effectAction != null) {
                this.effectList.Remove(effectAction);
                PoolManager.RemoveObject(effectAction.gameObject, PoolType.Battle);
            }
        }
    }
}
