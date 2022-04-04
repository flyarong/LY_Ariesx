using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

namespace Poukoute {
    public class FBLogin {
        public static AccountInfo Info {
            get {
                return new AccountInfo() {
                    accessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString,
                    userId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId
                };
            }
        }

        public static void Init() {
            FB.Init(OnInitComplete);
        }

        private static void OnInitComplete() {
            if (!FB.IsInitialized) {
                Debug.LogError("Facebook sdk initialized faild.");
                AccountManager.IsFBSucceed = false;
            } else {
                AccountManager.IsFBSucceed = true;
            }
        }

        public static void Login(OnBindDelegate handleResult) {
            if (PlayerPrefs.HasKey("FBUserId") && PlayerPrefs.HasKey("FBAccessToken")) {
                FB.LogInWithReadPermissions(
                    new List<string>() { "public_profile", "email", "user_friends" },
                    (result) => {
                        if (result == null) {
                            Debug.LogError("Facebook login failed");
                            return;
                        }
                        PlayerPrefs.SetString("FBUserId", result.AccessToken.UserId);
                        PlayerPrefs.SetString("FBAccessToken", result.AccessToken.UserId);
                        handleResult.Invoke("facebook");
                    }
                );
            } else {
                handleResult.Invoke("facebook");
            }
        }
    }
}