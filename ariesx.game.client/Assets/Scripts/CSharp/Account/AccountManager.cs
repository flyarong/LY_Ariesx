using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

namespace Poukoute{

    public class AccountInfo {
        public string accessToken;
        public string userId;
    }

    public delegate void OnBindDelegate(string method);

    public class AccountManager : MonoBehaviour{
        void Awake() {
            self = this;
            FBLogin.Init();
        }

        private bool isFBSucceed = false;
        private bool isGCSucceed = false;

        private Dictionary<string, InitDelegate> initDelegateDict = new Dictionary<string, InitDelegate>();

        public static bool IsFBSucceed {
            get {
                return Instance.isFBSucceed;
            } set {
                if (Instance.isFBSucceed != value) {
                    Instance.isFBSucceed = value;
                    if (Instance.initDelegateDict.ContainsKey("facebook")) {
                        Instance.initDelegateDict["facebook"].Invoke();
                    }
                }
            }
        }

        public static bool IsGCSucceed {
            get {
                return Instance.isGCSucceed;
            } set {
                if (Instance.isGCSucceed != value) {
                    Instance.isGCSucceed = value;
                    if (Instance.initDelegateDict.ContainsKey("gamecenter")) {
                        Instance.initDelegateDict["gamecenter"].Invoke();
                    }
                }
            }
        }

        public static void AddLoginDelegate(string method, InitDelegate OnStateChange) {
            Instance.initDelegateDict[method] = OnStateChange;
        }
        
        private static AccountManager self;
        public static AccountManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("LoginManager is not initialized.");
                }
                return self;
            }
        }

        public static void Login(string method, OnBindDelegate handleResult) {
            switch (method) {
                case "facebook":
                    FBLogin.Login(handleResult);
                    break;
                default:
                    break;
            }
        }

        public static AccountInfo GetToken(string method) {
            switch (method) {
                case "facebook":
                    return FBLogin.Info;
                default:
                    Debug.LogError("wrong method");
                    return new AccountInfo();
            }
        }
    }
}
