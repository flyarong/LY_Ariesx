using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Poukoute {
    public class BindView : MonoBehaviour {

        public Button btnFB;
        //private string appId = "bc4455c1f7a9c0f7";
        public TextMeshProUGUI txtFB;
        public Button btnYes;
        public Button btnNo;
        public Transform uiFBChoosenlBoard;
        public Transform pnlOldList;
        public Transform pnlNewList;
        //private string facebookId = null;
        private string UserId;
        // public static LyLogin self;
        private LoginViewModel loginViewModel;

        void Awake() {
            btnFB.onClick.AddListener(this.OnBtnWorldClick);
            this.txtFB.text = "用户中心";
            //self = this;

        }
        public void OnBtnWorldClick() {
            LySdk.ShowUserCenter();
        }

        public void Login(LoginViewModel login) {
            LySdk.Login();
            LySdk.self.LoginSuccessEvent += new LySdk.LoginSuccessHandler(this.GetToken);
            this.loginViewModel = login;
        }

        public void GetToken(object sender, LoginSuccessArgument loginArgument) {
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
                    RoleManager.SDKToken = (string)(System.Object)dict["token"];
                    Debug.Log("Token*********************************" + RoleManager.SDKToken);
                    this.loginViewModel.WorldReq();
                    //this.UserId = (string)(System.Object)dict["user_id"];
                    //string FBurl = string.Concat(VersionConst.url, "api/client/accounts/bind_prepare");
                    //StartCoroutine(NetManager.SendHttpMessage(FBurl, this.BindReq,
                    //   new string[] {"username",RoleManager.Udid, "channel", "longyuan", "code", RoleManager.Code }));
                }

            }
        }
        private void BindReq(WWW www) {
            Debug.Log(www.error + "*********************************************************");
            if (www.error != null) {
                UIManager.ShowTip(LocalManager.GetValue("facebook_error_seververifyfaile"), TipType.Error);
            } else {

                Dictionary<string, object> dict =
                    (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(www.text);
                if (dict.ContainsKey("login_token")) {
                    Debug.Log("NEW***********************************************************************************************************************************************");
                    RoleManager.LoginToken = (string)(System.Object)dict["login_token"];
                    RoleManager.SDKToken = (string)(System.Object)dict["token"];
                    PlayerPrefs.SetString("loginToken", RoleManager.LoginToken);
                    //PlayerPrefs.SetString("facebookId", UserId);
                    PlayerPrefs.SetString("sdkToken", RoleManager.SDKToken);

                    Debug.Log("Token*********************************" + RoleManager.SDKToken);
                    LySdk.self.SetToken(RoleManager.GetRoleId(), RoleManager.SDKToken);
                    // Debug.Log((string)dict["messeges"]);
                    Destroy(txtFB.GetComponent<LocalizedTextMeshPro>());
                    txtFB.text = LocalManager.GetValue("facebook_alreadybound");
                    Destroy(btnFB);
                } else {
                    Debug.Log("OLD*********************************************************************************************************************************************");
                    Dictionary<string, object> dict1 = (Dictionary<string, object>)(System.Object)dict["account_extra"];
                    Dictionary<string, object> dict2 = (Dictionary<string, object>)(System.Object)dict1["longyuan"];
                    this.UserId = (string)(System.Object)dict2["user_id"];
                    ShowChoose((List<System.Object>)dict["raw_account_roles"], (List<System.Object>)dict["target_account_roles"]);
                }
            }
        }
        private void AckBind(WWW www) {
            Debug.Log("bind***************************************************************************"); Debug.Log(www);
            Dictionary<string, object> dict =
                   (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(www.text);
            RoleManager.LoginToken = (string)(System.Object)dict["login_token"];
            RoleManager.SDKToken = (string)(System.Object)dict["token"];
            PlayerPrefs.SetString("loginToken", RoleManager.LoginToken);
            //PlayerPrefs.SetString("facebookId", UserId);
            PlayerPrefs.SetString("sdkToken", RoleManager.SDKToken);
            //LySdk.self.SetToken(RoleManager.GetRoleId(), RoleManager.LoginToken);
            GameManager.RestartGame();

        }
        private void ShowChoose(List<object> newList, List<object> oldList) {
            foreach (Dictionary<string, object> world in oldList) {
                GameObject Item = PoolManager.GetObject(PrefabPath.fBRoleItem, pnlOldList);
                FBItemPreference ItemView = Item.GetComponent<FBItemPreference>();
                ItemView.txtName.text = (string)world["role_name"];
                ItemView.txtSever.text = (string)world["world_name"];
                ItemView.txtLv.text = (world["level"]).ToString();
            }
            foreach (Dictionary<string, object> world in newList) {
                GameObject Item = PoolManager.GetObject(PrefabPath.fBRoleItem, pnlNewList);
                FBItemPreference ItemView = Item.GetComponent<FBItemPreference>();
                ItemView.txtName.text = (string)world["role_name"];
                ItemView.txtSever.text = (string)world["world_name"];
                ItemView.txtLv.text = (world["level"]).ToString();
            }
            uiFBChoosenlBoard.GetComponent<CanvasGroup>().alpha = 1;
            uiFBChoosenlBoard.GetComponent<CanvasGroup>().blocksRaycasts = true;
            btnYes.onClick.AddListener(OnClickYes);
            btnNo.onClick.AddListener(OnClickNo);
        }

        private void OnClickYes() {
            Debug.Log("YES**********************************************************************************************");
            string url = string.Concat(VersionConst.url, "api/client/accounts/bind");
            StartCoroutine(NetManager.SendHttpMessage(url, this.AckBind,
            new string[] { "username", RoleManager.Udid, "channel", "longyuan", "user_id", this.UserId }));
            Destroy(btnYes);
        }

        private void OnClickNo() {
            uiFBChoosenlBoard.GetComponent<CanvasGroup>().alpha = 0;
            uiFBChoosenlBoard.GetComponent<CanvasGroup>().blocksRaycasts = false;
            GameHelper.ClearChildren(pnlNewList);
            GameHelper.ClearChildren(pnlOldList);
        }
    }
}