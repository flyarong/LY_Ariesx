using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.adjust.sdk;

namespace Poukoute {
    public class SdkManager : MonoBehaviour {
        private static SdkManager self;
        //private static bool initialized = false;
        public static SdkManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("SdkManager is not initialized.");
                }
                return self;
            }
        }

        //private LyCSSdk lyCSSdk;
        private AdjustSdk adjustSdk;
        private FacebookSdk facebookSdk;
        //private LYVoiceSdk lyVoiceSdk;
        //private LYVoiceCallback lyVoiceCallback;
        //private LyDataSdk lyDataSdk;
        //private LySdk lySdk;

        void Awake() {
            self = this;
            PoolManager.GetObject<LyCSSdk>(this.transform);
            PoolManager.GetObject<LYVoiceSdk>(this.transform);
            PoolManager.GetObject<LYVoiceCallback>(this.transform);
            PoolManager.GetObject<LyDataSdk>(this.transform);
            this.adjustSdk = PoolManager.GetObject<AdjustSdk>(this.transform);
            this.facebookSdk = PoolManager.GetObject<FacebookSdk>(this.transform);
           // this.lySdk = PoolManager.GetObject<LySdk>(this.transform);
        }

        public static void AchievedLevel(string buildingName) {
            Instance.facebookSdk.AchievedLevel(buildingName);
            Instance.adjustSdk.TownhallLevelUp(buildingName);
        }
    }
}
