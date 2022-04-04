using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Poukoute {
    public class PaySuccessArgument : EventArgs {
        private string token;
        private string orderId;
        public PaySuccessArgument(string token, string orderId) {
            this.token = token;
            this.orderId = orderId;
        }
        public string Token {
            get { return this.token; }
        }
        public string OrderId {
            get { return this.orderId; }
        }
    }
    public class ConsumeSuccessArgument : EventArgs {
        private string token;
        private string orderId;
        public ConsumeSuccessArgument(string token, string orderId) {
            this.token = token;
            this.orderId = orderId;
        }
        public string Token {
            get { return this.token; }
        }
        public string OrderId {
            get { return this.orderId; }
        }
    }
    public class GooglePlay : MonoBehaviour {
        private static AndroidJavaObject JO;
        public static GooglePlay self;
        public delegate void PaySuccessHandler(object sender, PaySuccessArgument e);
        public  event PaySuccessHandler PaySuccessEvent;
        public delegate void ConsumeSuccessHandler(object sender, ConsumeSuccessArgument e);
        public event ConsumeSuccessHandler ConsumeSuccessEvent;
        private void Awake() {
            self = this.gameObject.GetComponent<GooglePlay>();
            #if !UNITY_EDITOR && UNITY_ANDROID
            AndroidJavaClass JC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            JO = JC.GetStatic<AndroidJavaObject>("currentActivity");
            //JO.Call("init");
#endif
        }

        public void onPaySuccess(string PayJson) {
            Dictionary<string, object> dict =
                        (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(PayJson);
            Debug.Log("#**************************************************************************************************************************************");
            Debug.Log((string)(System.Object)dict["productId"]);
            Debug.Log((string)(System.Object)dict["developerPayload"]);
            string developerPayload = (string)(System.Object)dict["developerPayload"];
            Debug.Log((string)(System.Object)dict["purchaseToken"]);
            string purchaseToken = (string)(System.Object)dict["purchaseToken"];
            PaySuccessArgument paySuccessArgument = new PaySuccessArgument(purchaseToken, developerPayload);
            this.PaySuccessEvent(this, paySuccessArgument);
        }

        public void onConsumeSuccess(string jo) {
            Dictionary<string, object> dict =
                         (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(jo);
            string token = (string)(System.Object)dict["token"];
            string orderId = (string)(System.Object)dict["orderId"];
            ConsumeSuccessArgument ConsumeSuccessArgument = new ConsumeSuccessArgument(token, orderId);
            this.ConsumeSuccessEvent(this, ConsumeSuccessArgument);

        }

        public static void PAY(string amount, string user_id, string product_id, string channel_uid,
                    string app_order_id, string product_name) {
            JO.Call("pay", amount, user_id, product_id, channel_uid, app_order_id, product_name);
        }

        public static void GetPurchases() {
            JO.Call("getPurchases");
        }

        public static void ConsumePurchase(string token,string orderId) {
            JO.Call("consumePurchase", token,orderId);
        }
    }
}
