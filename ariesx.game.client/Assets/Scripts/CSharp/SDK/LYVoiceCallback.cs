using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Text;
using LitJson;
using Protocol;

namespace Poukoute {
    public class LYVoiceCallback : MonoBehaviour {
        private const int TYPE_GETANCHORAVATAR = 1000;//获取头像
        private const int TYPE_DESTORY = 1001;//
        private const int TYPE_LOGOUT = 1002;//登出
        private const int TYPE_INIT = 1003;//初始化
        private const int TYPE_BUYGIFT = 1004;//购买礼物
        private const int TYPE_LIVESTATUS = 1006;//直播状态

        //1. 初始化
        public static System.Action initSuccess;
        public static System.Action initFailed;

        //2.支付订单
        public static System.Action buyGiftSuccess;
        public static System.Action buyGiftFailed;

        //3.登出
        public static System.Action logoutSuccess;
        public static System.Action logoutFailed;

        //4.获取头像
        public static System.Action avatarSuccess;
        public static System.Action avatarFailed;

        void Awake() {
            gameObject.name = "LyVoice";
        }

        public void onMessageReceived(string msg) {
            JsonData data = JsonMapper.ToObject(msg);
            int type = data["type"].ValueAsInt();
            //Debug.LogError("onMessageReceived: " + type + "  " + data["data"].ValueAsString());
            bool success = (data["code"].ValueAsString()).CustomEquals("1") ? true : false;
            switch (type) {
                case TYPE_INIT://初始化
                    if (success) {
                        initSuccess();
                    } else {
                        initFailed();
                    }
                    break;
                case TYPE_BUYGIFT://购买礼物
                    if (success) {
                        buyGiftSuccess();
                    } else {
                        buyGiftFailed();
                    }
                    break;
                case TYPE_LOGOUT://登出
                    if (success) {
                        logoutSuccess();
                    } else {
                        logoutFailed();
                    }
                    break;
                case TYPE_GETANCHORAVATAR://获取头像
                    if (success) {
                        //Debug.Log("TYPE_GETANCHORAVATAR: " + data["data"]);
                        avatarSuccess();

                    } else {
                        avatarFailed();
                    }
                    break;
                case TYPE_LIVESTATUS://直播状态回调
                    TriggerManager.Invoke(Trigger.VoiceLiveStatusChange, data["data"].ValueAsString());
                    break;
                default:
                    break;
            }
        }


    }
}
