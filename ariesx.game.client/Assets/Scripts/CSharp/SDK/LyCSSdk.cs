using ProtoBuf;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    [Serializable]
    public class CSResult {
        public int code;
        public string msg;
        public CSResultdata data;
    }

    [Serializable]
    public class CSResultdata {
        public string token;
        public int expired;
        public string api_url;
        public string im_url;
    }


    public class LyCSSdk : MonoBehaviour {
        private static LyCSSdk self;

        private Button btnCustom;
        private Transform pnlSetting;
        private GameObject ui;
        private string token;
        private string apiUrl;
        private string imUrl;

        // Android instance.
#if UNITY_ANDROID
        AndroidJavaClass jc;
        AndroidJavaObject jo;
        AndroidJavaClass jMain;
#endif
        private void Awake() {
            self = this;
            this.ui = GameObject.FindGameObjectWithTag("Debug");
            this.pnlSetting = this.ui.transform.Find("PnlDebug");
            Transform pnlDebug = this.pnlSetting.Find("PnlDebug");
            this.btnCustom = pnlDebug.Find("BtnCustom").GetComponent<Button>();
            this.btnCustom.onClick.RemoveAllListeners();
            this.btnCustom.onClick.AddListener(this.OnBtnCustomerClick);

            Debug.Log("lyCSSdk Init");
#if !UNITY_EDITOR && UNITY_ANDROID
            this.jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            this.jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            this.jMain = new AndroidJavaClass("com.lyservice.CSSDK");
#endif

        }

        private void OnBtnCustomerClick() {
            Debug.Log("start calling...");
            if (!PlayerPrefs.HasKey("csExpired") ||
                !PlayerPrefs.HasKey("csToken") ||
                !PlayerPrefs.HasKey("csApiUrl") ||
                !PlayerPrefs.HasKey("csImUrl")) {
                this.CustomerServiceTokenReq();
                return;
            }
            int timeStamp = PlayerPrefs.GetInt("csExpired");
            string token = PlayerPrefs.GetString("csToken");
            string apiUrl = PlayerPrefs.GetString("csApiUrl");
            string imUrl = PlayerPrefs.GetString("csImUrl");

            Debug.LogError(timeStamp + " : " + RoleManager.GetCurrentUtcTime() / 1000);
            if (timeStamp < RoleManager.GetCurrentUtcTime() / 1000) {
                this.CustomerServiceTokenReq();
            } else {
                this.token = token;
                this.apiUrl = apiUrl;
                this.imUrl = imUrl;
                this.ShowCSFrame();
            }
        }

        private void CustomerServiceTokenReq() {
            this.btnCustom.interactable = false;
            GetCustomerServiceTokenReq req = new GetCustomerServiceTokenReq();
            NetManager.SendMessage(req, typeof(GetCustomerServiceTokenAck).Name, 
                CustomerServiceTokenAck, (message) => { this.btnCustom.interactable = true; },
                () => { this.btnCustom.interactable = true; });
        }

        private void CustomerServiceTokenAck(IExtensible message) {
            this.btnCustom.interactable = true;
            GetCustomerServiceTokenAck ack = message as GetCustomerServiceTokenAck;
            PlayerPrefs.SetString("csToken", ack.Token);
            PlayerPrefs.SetString("csApiUrl", ack.ApiUrl);
            PlayerPrefs.SetString("csImUrl", ack.IMUrl);
            PlayerPrefs.SetInt("csExpired", (int)ack.Expired +
                (int)(RoleManager.GetCurrentUtcTime() / 1000));
            this.token = ack.Token;
            this.apiUrl = ack.ApiUrl;
            this.imUrl = ack.IMUrl;
            this.ShowCSFrame();
        }

        private void ShowCSFrame() {
            Debug.Log(this.token);
            Debug.Log(this.apiUrl);
            Debug.Log(this.imUrl);
#if UNITY_EDITOR
#elif UNITY_ANDROID
            this.jMain.CallStatic("showCustomer", this.jo, 
                this.token, LocalManager.SdkLanguage, this.apiUrl, this.imUrl);
#endif
        }

        public static string GetDeviceId() {
            Debug.Log("getDeviceId calling");
            if (self == null) {
                return string.Empty;
            }
#if UNITY_ANDROID
            string deviceId = self.jMain.CallStatic<string>("getDevice", self.jo);
            return deviceId;
#else
            return string.Empty;
#endif
        }
        //  public static
    }
}
