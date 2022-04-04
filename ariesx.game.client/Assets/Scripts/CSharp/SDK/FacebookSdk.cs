using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Protocol;

namespace Poukoute {
    public class FacebookSdk : MonoBehaviour {
        private BuildModel buildModel;

        void Awake() {
            if (FB.IsInitialized) {
                FB.ActivateApp();
            } else {
                FB.Init(() => {
                    this.InitCallback();
                });
            }
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            TriggerManager.Regist(Trigger.FirstLogin, CompletedRegistration);
            TriggerManager.Regist(Trigger.FinishFte, CompletedTutorial);
        }

        private void InitCallback() {
            if (FB.IsInitialized) {
#if !UNITY_EDITOR
                    FB.ActivateApp();
#endif
            } else {
                Debug.LogError("Failed to Initialize the Facebook SDK");
            }
        }

        public void AchievedLevel(string building) {
            if (building.CustomEquals(ElementName.townhall)) {
                this.LogAppEvent(AppEventName.AchievedLevel,
                    (float)this.buildModel.buildingDict[building].Level);
            }
        }

        public void CompletedRegistration() {
            this.LogAppEvent(AppEventName.CompletedRegistration);
        }

        public void CompletedTutorial() {
            this.LogAppEvent(AppEventName.CompletedTutorial);
        }

        private void LogAppEvent(string eventName, float? valueToSum = null) {
#if !UNITY_EDITOR
           // if (VersionConst.IsOnline()) {
                FB.LogAppEvent(eventName);
           // }
#endif
        }
    }
}
