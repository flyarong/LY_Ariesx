using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {

    public class BattleHealthSimulator : MonoBehaviour {
        public Transform root;
        public Camera camera;
        void Awake() {
            this.camera = GameObject.FindGameObjectWithTag("BattleCamera").GetComponent<Camera>();
        }

        void Update() {
            this.transform.position = (Vector2)this.camera.WorldToScreenPoint(root.transform.position);
            //Debug.LogError(this.transform.position);
        }
    }
}
