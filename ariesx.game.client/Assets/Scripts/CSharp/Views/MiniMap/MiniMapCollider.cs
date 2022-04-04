using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Protocol;

namespace Poukoute {
    public class MiniMapCollider : MonoBehaviour {
        public class ColliderEvent : UnityEvent { }
        public ColliderEvent onEnterState = new ColliderEvent();

        void OnTriggerEnter2D(Collider2D collider) {
            this.onEnterState.InvokeSafe();
        }
    }
}
