using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class LyLoginViewModel : MonoBehaviour {
        private static LyLoginViewModel self;
        private LoginViewModel parent;

        public static LyLoginViewModel Instance {
            get {
                if (self == null) {
                    Debug.LogError("LyLoginViewModel is not initialized");
                }
                return self;
            }
        }

        private void Awake() {
            self = this;
            this.parent = this.transform.parent.GetComponent<LoginViewModel>();
        }

        public void Login() {
            LySdk.Login();
            LySdk.self.LoginSuccessEvent += new LySdk.LoginSuccessHandler(this.GetToken);
            LySdk.self.LoginCloseEvent += new LySdk.LoginCloseHander(this.OnLoginClose);
        }

        public void OnLoginClose(object sender, LoginCloseArgument loginArgument) {
            Debug.Log("**********************OnLoginClose");
            this.parent.SetReturnLoginView();
        }

        public void GetToken(object sender, LoginSuccessArgument loginArgument) {
            Debug.Log("eeeeeeeeeeeeeee");
            Debug.Log(loginArgument);
            Debug.Log(loginArgument.Code);
            string url = string.Concat(VersionConst.url, "api/client/accounts/login_with_longyuan");
            StartCoroutine(NetManager.SendHttpMessage(url, this.LyLoginAck,
               new string[] { "code", loginArgument.Code }));

            //string url = string.Concat(VersionConst.url, "api/client/accounts/bind_prepare");
            //StartCoroutine(NetManager.SendHttpMessage(FBurl, this.BindReq,
            //   new string[] { "username", RoleManager.Udid, "channel", "longyuan", "code", code }));
        }

        private void LyLoginAck(WWW www) {
            Debug.Log("***************************************************************");
            Debug.Log(www.error);
            Debug.Log(www.text);
            if (www.error != null) {
                UIManager.ShowTip(LocalManager.GetValue("facebook_error_seververifyfaile"), TipType.Error);
            } else {
                Dictionary<string, object> dict =
                    (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(www.text);
                if (dict.ContainsKey("errors")) {
                    Debug.Log("FBACKERROR************************************************************************************************************");
                    Debug.Log((string)(System.Object)dict["errors"]);
                    UIManager.ShowTip(LocalManager.GetValue("facebook_error_seververifyfaile"), TipType.Error);

                } else {
                    Debug.Log("FBACKOK*********************************************************************************************************************************");
                    RoleManager.LoginToken = (string)(System.Object)dict["login_token"];
                    Debug.LogError(RoleManager.LoginToken);
                    RoleManager.SDKToken = (string)(System.Object)dict["token"];
                    if (!RoleManager.SDKToken.CustomIsEmpty()) {
                        LySdk.self.SetToken(RoleManager.Udid, RoleManager.SDKToken);
                    }
                    Debug.Log("Token*********************************" + RoleManager.SDKToken);
                    this.parent.WorldReq();
                    //this.UserId = (string)(System.Object)dict["user_id"];
                    //string FBurl = string.Concat(VersionConst.url, "api/client/accounts/bind_prepare");
                    //StartCoroutine(NetManager.SendHttpMessage(FBurl, this.BindReq,
                    //   new string[] {"username",RoleManager.Udid, "channel", "longyuan", "code", RoleManager.Code }));
                }

            }
        }
    }
}
