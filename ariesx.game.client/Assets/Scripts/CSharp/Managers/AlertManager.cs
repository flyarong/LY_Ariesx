using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ProtoBuf;

namespace Poukoute {
    public class AlertManager : MonoBehaviour {

        public static AlertManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("AlertManager is not initialized.");
                }
                return self;
            }
        }
        private static AlertManager self;



        void Awake() {
            self = this;

        }

        
    }
}
