using UnityEngine;
using System.Collections;

namespace Poukoute {
    public class CustomFixPosition : MonoBehaviour {
        public bool isFix;
        private Vector3 originOffset;
        public Transform fixParent;

        void Awake() {
            this.originOffset = this.transform.position - this.fixParent.position;
        }

        void LateUpdate() {
            if (this.isFix) {
                this.transform.position = this.fixParent.position + this.originOffset;
            }
        }
    }
}