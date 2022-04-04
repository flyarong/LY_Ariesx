using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class TributeAirShipView : MonoBehaviour {
        private BoxCollider2D tributeAirCollider;

        void Awake() {
            this.tributeAirCollider = this.transform.parent.GetComponent<BoxCollider2D>();
        }

        public void OnStep1Start() {
            this.tributeAirCollider.enabled = false;
        }

        public void OnStep3Start() {
            this.tributeAirCollider.enabled = true;
        }

        public void OnStep4Start() {
            this.tributeAirCollider.enabled = false;
        }

        public void OnStep4End() {
            Destroy(this.transform.parent.gameObject);
        }
    }
}
