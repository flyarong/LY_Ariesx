using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public class BattleEffectAction : MonoBehaviour, IPoolHandler {
        public Animator animator;

        public ParticleSystem particle;

        public float duration;
        
        public bool loop;

        public void Play() {
            if (this.particle != null) {
				particle.Play(true);
            }
            if (this.animator != null) {
                this.animator.enabled = true;
            }
        }

        public void Stop() {
            this.OnInPool();
        }

        public void OnInPool() {
            if (this.particle != null) {
                particle.Stop(true);
            }
            if (this.animator != null) {
                this.animator.enabled = false;
            }
        }

        public void OnOutPool() {
            if (this.particle != null && this.particle.main.playOnAwake) {
                particle.Play(true);
            }
            if (this.animator != null) {
                this.animator.enabled = true;
            }
        }
    }
}
