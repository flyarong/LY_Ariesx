using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using Protocol;
using System;
using com.adjust.sdk;

namespace Poukoute {
    public class AdjustSdk : MonoBehaviour {
        private static AdjustSdk self;
        private BuildModel buildModel;

        string index;

        void Awake() {
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            if (Adjust.Instance == null) {
                Instantiate(UnityEngine.Resources.Load<GameObject>("Adjust"));
            }
            TriggerManager.Regist(Trigger.Login, Login);
            TriggerManager.Regist(Trigger.FinishFte, CompleteTutorial);
        }

        public void CompletedRegistration() {
            AdjustEvent CompletedRegistration = new AdjustEvent("8gbgt7");
            Adjust.trackEvent(CompletedRegistration);
        }

        public void Firstcharge() {
            AdjustEvent Firstcharge = new AdjustEvent("rabw3o");
            Adjust.trackEvent(Firstcharge);
        }

        public void Login() {
            AdjustEvent Login = new AdjustEvent("mzxgor");
            Adjust.trackEvent(Login);
        }


        public void Revenue() {
            AdjustEvent Revenue = new AdjustEvent("tg186c");
            Adjust.trackEvent(Revenue);
        }

        public void TownhallLevelUp(string buildingName) {
            if (buildingName.CustomEquals(ElementName.townhall)) {
                if (this.buildModel.buildingDict[ElementName.townhall].Level == 2) {
                    this.AchievedLevel2();
                } else if (this.buildModel.buildingDict[ElementName.townhall].Level == 3) {
                    this.AchievedLevel3();
                }
            }
        }

        private void AchievedLevel2() {
            AdjustEvent AchievedLevel2 = new AdjustEvent("g3pw39");
            Adjust.trackEvent(AchievedLevel2);
        }

        private void AchievedLevel3() {
            AdjustEvent AchievedLevel3 = new AdjustEvent("v4zqav");
            Adjust.trackEvent(AchievedLevel3);
        }

        private void CompleteTutorial() {
            AdjustEvent CompleteTutorial = new AdjustEvent("9obko3");
            Adjust.trackEvent(CompleteTutorial);
        }
    }
}
