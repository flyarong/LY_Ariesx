using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class AnimatorPoolMember : MonoBehaviour, IPoolHandler {
        [SerializeField]
        private Animator animator;

        public void OnInPool() {
            animator.enabled = false;
        }

        public void OnOutPool() {
            animator.enabled = true;
        }

        public void Destroy() {
            PoolManager.RemoveObject(this.gameObject);
        }
    }
}
