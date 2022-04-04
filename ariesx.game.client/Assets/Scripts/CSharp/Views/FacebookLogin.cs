using ProtoBuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using TMPro;
namespace Poukoute {
    public class FacebookLogin : MonoBehaviour {
        public Button btnFB;
        public TextMeshProUGUI txtFB;
        public Button btnYes;
        public Button btnNo;
        public Transform uiFBChoosenlBoard;
        public Transform pnlOldList;
        public Transform pnlNewList;
        private string facebookId = null;
        private string UserId;


        void Awake() {
            btnFB.onClick.AddListener(this.OnBtnWorldClick);
            btnYes.onClick.AddListener(this.OnClickYes);
            btnNo.onClick.AddListener(this.OnClickNo);
            facebookId = PlayerPrefs.GetString("facebookId");
            if (!VersionConst.CanLinkFacebook()) {
                btnFB.gameObject.SetActiveSafe(false);
            }
            if (!string.IsNullOrEmpty(this.facebookId)) {
                Destroy(txtFB.GetComponent<LocalizedTextMeshPro>());
                txtFB.text = LocalManager.GetValue("facebook_alreadybound");
                Destroy(btnFB);
            }
        }

        public void OnBtnWorldClick() {
            FB.LogInWithReadPermissions(
                 new List<string>() { "public_profile", "email", "user_friends" },
                AuthCallBack);
        }

        private void AuthCallBack(ILoginResult result) {
            if (FB.IsLoggedIn) {
                // AccessToken class will have session details
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                this.UserId = aToken.UserId;
                string FBurl = string.Concat(VersionConst.url, "api/client/accounts/login_with_facebook");
                StartCoroutine(NetManager.SendHttpMessage(FBurl, this.FBLoginAck,
                   new string[] { "input_token", aToken.TokenString, "user_id", aToken.UserId }));
                Debug.Log("ATOKEN:" + aToken);
                Debug.Log(aToken.TokenString);
                Debug.Log(aToken.Permissions);
                Debug.Log(aToken.LastRefresh);
                Debug.Log(aToken.ExpirationTime);
                // Print current access token's User ID
                Debug.Log("The aToKen UserId :" + aToken.UserId);
                // Print current access token's granted permissions
                foreach (string perm in aToken.Permissions) {
                    Debug.Log(perm);
                }
            } else {
                UIManager.ShowTip(LocalManager.GetValue("facebook_error_loginfaile"), TipType.Error);
                Debug.Log("User cancelled login");
            }
        }

        private void FBLoginAck(WWW www) {
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
                    UIManager.ShowTip(LocalManager.GetValue("facebook_error_seververifyfaile"), TipType.Error);
                } else {
                    Debug.Log("FBACKOK*********************************************************************************************************************************");
                    RoleManager.LoginToken = (string)(System.Object)dict["login_token"];
                    string FBurl = string.Concat(VersionConst.url, "api/client/accounts/bind_prepare");
                    StartCoroutine(NetManager.SendHttpMessage(FBurl, this. BindReq,
                       new string[] { "user_id", this.UserId, "udid", RoleManager.Udid }));
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
                if (dict.ContainsKey("messages")) {
                    Debug.Log("NEW***********************************************************************************************************************************************");
                    PlayerPrefs.SetString("loginToken", RoleManager.LoginToken);
                    PlayerPrefs.SetString("facebookId", UserId);
                    string url = string.Concat(VersionConst.url, "api/client/accounts/bind");
                    StartCoroutine(NetManager.SendHttpMessage(url, this.AckBind,
                    new string[] { "udid", RoleManager.Udid, "user_id", UserId }));
                   
                } else {
                    Debug.Log("OLD*********************************************************************************************************************************************");
                    ShowChoose((List<System.Object>)dict["udid_role_list"], (List<System.Object>)dict["facebook_role_list"]);
                }
            }
        }

        private void ShowChoose(List<object> newList, List<object> oldList) {
            foreach (Dictionary<string, object> world in oldList) {
                GameObject Item = PoolManager.GetObject(PrefabPath.fBRoleItem, pnlOldList);
                FBItemPreference ItemView = Item.GetComponent<FBItemPreference>();
                ItemView.txtName.text = (string)world["role_name"];
                ItemView.txtSever.text = (string)world["world_name"];
                ItemView.txtLv.text = (string)world["level"];
            }
            foreach (Dictionary<string, object> world in newList) {
                GameObject Item = PoolManager.GetObject(PrefabPath.fBRoleItem, pnlNewList);
                FBItemPreference ItemView = Item.GetComponent<FBItemPreference>();
                ItemView.txtName.text = (string)world["role_name"];
                ItemView.txtSever.text = (string)world["world_name"];
                ItemView.txtLv.text = (string)world["level"];
            }
            uiFBChoosenlBoard.GetComponent<CanvasGroup>().alpha = 1;
            uiFBChoosenlBoard.GetComponent<CanvasGroup>().blocksRaycasts = true;
            btnYes.onClick.AddListener(OnClickYes);
            btnNo.onClick.AddListener(OnClickNo);
        }

        private void AckBind(WWW www) {
            //Dictionary<string, object> dict =
            //       (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(www.text);
            Debug.Log("bind***************************************************************************");
            Debug.Log(www);
           // Debug.Log((string)dict["messeges"]);
            Destroy(txtFB.GetComponent<LocalizedTextMeshPro>());
            txtFB.text = LocalManager.GetValue("facebook_alreadybound");
            Destroy(btnFB);
        }

        private void OnClickYes() {
            Debug.Log("YES**********************************************************************************************");
            PlayerPrefs.SetString("loginToken", RoleManager.LoginToken);
            PlayerPrefs.SetString("facebookId", UserId);
            GameManager.RestartGame();
        }

        private void OnClickNo() {
            uiFBChoosenlBoard.GetComponent<CanvasGroup>().alpha = 0;
            uiFBChoosenlBoard.GetComponent<CanvasGroup>().blocksRaycasts = false;
            GameHelper.ClearChildren(pnlNewList);
            GameHelper.ClearChildren(pnlOldList);
        }
    }
}
