using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.adjust.sdk;

namespace Poukoute {
    public class AdvanceSdkManager : MonoBehaviour {
        private static AdvanceSdkManager self;
        //private static bool initialized = false;
        public static AdvanceSdkManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("AdvanceSdkManager is not initialized.");
                }
                return self;
            }
        }

#if LONGYUAN
        private LySdk lySDK;
#elif GOOGLEPLAY
        private GooglePlay googlePlay;
#endif
        //private LySdk lySdk;

        void Awake() {
            self = this;
#if LONGYUAN
            this.lySDK = PoolManager.GetObject<LySdk>(this.transform);
            //this.lySDK = this.gameObject.AddComponent<LySdk>();
#elif GOOGLEPLAY
            //this.googlePlay = this.gameObject.AddComponent<GooglePlay>();
            this.googlePlay = PoolManager.GetObject<GooglePlay>(this.transform);
#endif
        }
    }
}
