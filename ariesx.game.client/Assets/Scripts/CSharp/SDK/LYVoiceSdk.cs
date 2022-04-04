using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using Protocol;

namespace Poukoute {
    public class LYVoiceSdk : MonoBehaviour {
        private static LYVoiceSdk Instance;
#if !UNITY_EDITOR && UNITY_ANDROID
        private AndroidJavaClass jc;
        private AndroidJavaObject jo;
        private AndroidJavaClass jMain;
#endif        

        private int registerDays = 0;
        private int limitLevel = 1;
        private int limitRegisterDays = 1;
        private string headAvatar = string.Empty;
        private static bool isInit = false;

        private void Awake() {
            Instance = this;
#if !UNITY_EDITOR && UNITY_ANDROID
            TriggerManager.Regist(Trigger.VoiceLiveUserDataChange, this.ChangeAccount);
            TriggerManager.Regist(Trigger.Login, this.InitLyVoiceSdk);
            TriggerManager.Regist(Trigger.Logout, this.Logout);
            this.jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            this.jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            this.jMain = new AndroidJavaClass("com.UnityGCSDK");
#endif
        }

        public void InitLyVoiceSdk() {
#if !UNITY_EDITOR && UNITY_ANDROID
            if (!isInit) {
                bool isAboard = false;
                if (VersionConst.version.CustomEquals("googleplay")) {
                    isAboard = true;
                }
                string anchorId = RoleManager.Udid;
                string gameToken = RoleManager.LoginToken;
                string channel = VersionConst.channel;
                string gameKey = "bc4455c1f7a9c0f7";
                BuildModel buildModel = ModelManager.GetModelData<BuildModel>();
                int anchorLevel = buildModel.GetBuildLevelByName(ElementName.townhall);
                this.registerDays = anchorLevel;
                string userName = RoleManager.GetRoleName();
                userName = userName.CustomIsEmpty() ? "Undefined" : userName;
                Debug.LogError("InitLyVoiceSdk callded");
                this.DoInit(isAboard, gameKey, gameToken, channel, anchorId,
                            this.headAvatar, userName, anchorLevel,
                            this.registerDays, this.limitLevel, this.limitRegisterDays,
                            this.OnInitSuccess, this.OninitFailed, this.OnBuyGiftSuccess, this.OnBuyGiftFailed);

            }
#endif
        }

        /**
         * 初始化语音直播
         * @param mContext
         * @param gameKey
         * @param anchorId
         * @param avatar 
         * @param anchorName 主播名字
         * @param anchorLevel 主播等级
         * @param registerDays 注册天数
         * @param limitLevel 最小直播等级
         * @param limitRegisterDays 最少天数
         * @param callback 初始化结果回调
         * @param giftCallBackListener 购买礼物回调
         */
        private void DoInit(bool isAboard, string gameKey, string gameToken, string channel, string anchorId, string avatar,
                    string anchorName, int anchorLevel, int registerDays, int limitLevel, int limitRegisterDays,
                    System.Action initSuccess, System.Action initFailed,
                    System.Action buyGiftSuccess, System.Action buyGiftFailed) {
#if !UNITY_EDITOR && UNITY_ANDROID
            LYVoiceCallback.initSuccess = initSuccess;
            LYVoiceCallback.initFailed = initFailed;
            LYVoiceCallback.buyGiftSuccess = buyGiftSuccess;
            LYVoiceCallback.buyGiftFailed = buyGiftFailed;
            jMain.CallStatic("initUnity", jo, isAboard, gameKey, gameToken, channel, anchorId, avatar, anchorName, anchorLevel,
                        registerDays, limitLevel, limitRegisterDays);
#endif
        }
        //显示主界面
        public static void ShowLyvoiceLiveView() {
#if !UNITY_EDITOR && UNITY_ANDROID
            Instance.jMain.CallStatic("showLiveVoiceMainActivity", Instance.jo, LocalManager.SdkLanguage);
#endif
        }

        private void OnApplicationQuit() {
#if !UNITY_EDITOR && UNITY_ANDROID
            Instance.Logout();
#endif
        }

        //切换账号
        void ChangeAccount() {
#if !UNITY_EDITOR && UNITY_ANDROID
            try {
                BuildModel buildModel = ModelManager.GetModelData<BuildModel>();
                int anchorLevel = buildModel.GetBuildLevelByName(ElementName.townhall) + 1;
                Debug.LogError("ChangeAccount " + anchorLevel);
                if (anchorLevel > 0) {
                    string userName = RoleManager.GetRoleName();
                    userName = userName.CustomIsEmpty() ? "Undefined" : userName;
                    this.registerDays = anchorLevel;
                    jMain.CallStatic("updateUserInfoUnity", this.headAvatar, userName, anchorLevel, this.registerDays);
                }
            } catch {
                ;
            }
#endif
        }

        //登出
        private void Logout() {
#if !UNITY_EDITOR && UNITY_ANDROID
            LYVoiceCallback.logoutSuccess = () => {
            };
            LYVoiceCallback.logoutFailed = () => {
            };

            try {
                Instance.jMain.CallStatic("logout", Instance.jo);
            } catch {
                ;
            }
#endif
        }
        //获取主播头像
        public static void GetAvatar(System.Action initSuccess, System.Action initFailed) {
#if !UNITY_EDITOR && UNITY_ANDROID
            LYVoiceCallback.avatarSuccess = initSuccess;
            LYVoiceCallback.avatarFailed = initFailed;
            Instance.jMain.CallStatic("getLiveAnchorAvatarUnity");
#endif
        }

        //刷新房间数据
        public static void UpdataRoomData() {
#if !UNITY_EDITOR && UNITY_ANDROID
            Instance.jMain.CallStatic("refreshWatchingRoomData");
#endif
        }



#region Do init Callback
        private void OnInitSuccess() {
            isInit = true;
        }
        private void OninitFailed() {
            isInit = false;
        }
        private void OnBuyGiftSuccess() {
        }
        private void OnBuyGiftFailed() {
        }
        private void OnLogoutSuccess() {
            isInit = false;
        }
        private void OnLogoutFailed() {
            isInit = false;
        }
#endregion

    }

}