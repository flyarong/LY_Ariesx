using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using ProtoBuf;
using Protocol;

namespace Poukoute {

    public class ScreenEffectManager : MonoBehaviour {
        private static ScreenEffectManager self;
        public static ScreenEffectManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("ScreenEffectManager is not initialized.");
                }
                return self;
            }
        }
        private ScreenEffectViewModel viewModel;

        void Awake() {
            self = this;
            this.viewModel = PoolManager.GetObject<ScreenEffectViewModel>(this.transform);
        }

        static public void SetHighlightFrame(Transform nextTrans, UnityAction AfterCallBack,Vector2 offsetMin,Vector2 offsetMax) {
            Instance.viewModel.SetHighlightFrame(nextTrans, AfterCallBack, offsetMin, offsetMax);
        }

        static public void EndImmediately() {
            Instance.viewModel.EndImmediately();
        }
    }
}
