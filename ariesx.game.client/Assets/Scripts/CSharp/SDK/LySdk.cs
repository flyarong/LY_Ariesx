using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Poukoute {
    public class LoginSuccessArgument : EventArgs {
        private string code;
        public LoginSuccessArgument(string code) {
            this.code = code;
        }
        public string Code {
            get { return this.code; }
        }
    }
    public class LoginCloseArgument : EventArgs {
        public LoginCloseArgument() {
            
        }
    }
    public class LySdk : MonoBehaviour {
        private static AndroidJavaObject JO;
        public static LySdk self;
        public delegate void LoginSuccessHandler(object sender, LoginSuccessArgument e);
        public event LoginSuccessHandler LoginSuccessEvent;
        public delegate void LoginCloseHander(object sender, LoginCloseArgument e);
        public event LoginCloseHander LoginCloseEvent;

        private void Awake() {
            self = this;
            Debug.Log("LysdkAwake");
#if NOLYLOGIN
#elif !UNITY_EDITOR && UNITY_ANDROID
            Debug.Log("com.unity3d.player.UnityPlayer");
            AndroidJavaClass JC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            JO = JC.GetStatic<AndroidJavaObject>("currentActivity");
            JO.Call("init");
#endif
        }

        public static void Login() {
#if NOLYLOGIN
#elif !UNITY_EDITOR && UNITY_ANDROID
            Debug.Log("dologin");
            JO.Call("login");
#endif

        }

        public void SetToken(string uid, string token) {
#if NOLYLOGIN
#elif !UNITY_EDITOR && UNITY_ANDROID
            JO.Call("setToken", uid, token);
#endif
        }

        public void onLoginSuccess(string code) {
            Debug.Log("********************************************************************************************onLoginSuccess");
            Dictionary<string, object> dict =
                        (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(code);
            RoleManager.Code = (string)(System.Object)dict["code"];

            LoginSuccessArgument loginSuccessArgument = new LoginSuccessArgument((string)(System.Object)dict["code"]);
            Debug.Log(loginSuccessArgument.Code);
            this.LoginSuccessEvent(this, loginSuccessArgument);
        }

        public void onLoginClose(string code) {
            Debug.Log("********************************************************************************************onLoginClose");
            LoginCloseArgument loginCloseArgument = new LoginCloseArgument();
            Debug.Log(loginCloseArgument);
            this.LoginCloseEvent(this, loginCloseArgument);
        }

        public void onLogout() {
            GameManager.RestartGame();
        }

        public static void Pay(string amount, string user_id, string product_id, string channel_uid,
                    string app_order_id, string product_name) {
#if NOLYLOGIN
#elif !UNITY_EDITOR && UNITY_ANDROID
            JO.Call("pay", amount, user_id, product_id, channel_uid, app_order_id, product_name);
            //return JO.Call<int>("Add", a, b);
#endif
        }

        public static void ShowUserCenter() {
#if NOLYLOGIN
#elif !UNITY_EDITOR && UNITY_ANDROID
            JO.Call("showUserCenter");
#endif
        }
    }
}
