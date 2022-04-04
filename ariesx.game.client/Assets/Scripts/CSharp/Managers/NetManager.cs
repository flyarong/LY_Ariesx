using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class NetManager : MonoBehaviour {
        private NetProtocol netProtocol;
        private static NetManager self;
        //public string playerID;
        private UnityAction connectedAction;
        private UnityAction disconnectedAction;
        private bool isConnected;
        private string serverUrl;
        private const float TimeOut = 3f;
        private byte[] reply;

        //private int lastServerTag = 0;
        private int lastClientTag = 0;

        private Coroutine heartbeatCoroutine = null;
        private int heartbeatReference = 0;
        private const int heartbeatThreshold = 3;
        private float heartbeatInterval = 3f;

        private int reconnectReference = 0;
        private readonly int reconnectTotalReference = 15;
        private readonly float reconnectInterval = 0.8f;

        public static NetManager Instance {
            get {
                if (self == null) {
                    Debug.LogError("Netmanager is not initialized.");
                }
                return self;
            }
        }

        void Awake() {
            self = this;
            UpdateManager.Regist(UpdateInfo.NetManager, this.ReceiveMessage);
        }

        void Start() {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.CustomEquals("SceneMain")) {
                ModelManager.LoadScene("SceneLogin");
            }
        }

        // Use enumerator move next to simulater coroutine.
        IEnumerator Connect() {
            IEnumerator enumerator = netProtocol.ConnectAsync();
            bool isSucceed = true;
            object ret = null;
            while (true) {
                try {
                    if (!enumerator.MoveNext()) {
                        break;
                    }
                    ret = enumerator.Current;
                } catch (Exception e) {
                    Debug.LogError(e.Message);
                    isSucceed = false;
                    reconnectReference++;
                    break;
                }
                yield return ret;
            }
            if (isSucceed) {
                this.reconnectReference = 0;
                this.isConnected = true;
                this.connectedAction.InvokeSafe();

                NetHandler.RemoveTimeOutHandler(typeof(HeartbeatAck).Name);
                this.heartbeatReference = 0;
                this.heartbeatCoroutine = StartCoroutine(this.HeartbeatReq());
            } else {
                if (reconnectReference > reconnectTotalReference) {
                    Debug.LogError("reconnectReference " + reconnectReference);
                    UIManager.HideNetCircle();
                    //Debug.LogError("RestartGame");
                    GameManager.RestartGame();
                } else {
                    yield return YieldManager.GetWaitForSeconds(this.reconnectInterval);
                    InnerConnectToServer();
                }
                this.isConnected = false;
            }
        }

        public static void Disconnect() {
            if (Instance.netProtocol != null) {
                Instance.netProtocol.Close();
            }
        }

        //void UpdateAction() {
        //    this.ReceiveMessage();
        //}

        private IEnumerator HeartbeatReq() {
            HeartbeatReq req = new HeartbeatReq();
            while (true) {
                SendMessage(req, typeof(HeartbeatAck).Name, HeartbeatAck,
                    (message) => HeartbeatError(), HeartbeatError);
                yield return YieldManager.GetWaitForSeconds(this.heartbeatInterval);
            }
        }

        private void HeartbeatAck(IExtensible message) {
            this.heartbeatReference = 0;
            this.heartbeatInterval = 3f;
            UIManager.HideWifiAlert();
        }

        private void HeartbeatError() {
            this.heartbeatReference++;
            this.heartbeatInterval = 2f;
            if (this.heartbeatReference > heartbeatThreshold) {
                if (this.isConnected) {
                    this.Reconnect();
                }
            } else {
                if (this.isConnected) {
                    UIManager.ShowWifiAlert();
                }
            }

        }

        public static bool GetIsConnected() {
            return Instance.isConnected;
        }

        public static void ConnectToServer(string url) {
            Instance.serverUrl = string.Concat("ws://", url, ":12340/websocket");
            Instance.InnerConnectToServer();
            GameManager.InitStepC();
        }

        /// <summary>
        /// Send a http request, and handle the response.
        /// </summary>
        /// <param name="url">The request url</param>
        /// <param name="handler">The response handler</param>
        public static IEnumerator SendHttpMessage(string url, HttpHandler handler) {
            yield return YieldManager.GetWaitForSeconds(1f);
            WWW result = new WWW(url);
            yield return result;
            handler(result);
        }

        /// <summary>
        /// Send a http request, and handle the response.
        /// </summary>
        /// <param name="url">The request url</param>
        /// <param name="handler">The response handler</param>
        /// <param name="values"> post values </param>
        public static IEnumerator SendHttpMessage(string url, HttpHandler handler, params string[] values) {
            yield return YieldManager.GetWaitForSeconds(1f);
            WWWForm form = new WWWForm();
            int length = values.Length / 2;
            for (int i = 0; i < length; i++) {
                form.AddField(values[i * 2], values[i * 2 + 1]);
            }
            WWW result = new WWW(url, form);
            yield return result;
            handler(result);
        }
        /// <summary>
        /// Send package to server.
        /// </summary>
        /// <param name="package">Package name.</param>
        /// <param name="message">The package body.</param>
        /// <param name="callBack">The ack handler</param>
        public static void SendMessage(IExtensible message, string ackPackage,
            UnityAction<IExtensible> callBack, UnityAction<IExtensible> errorCallback = null,
            UnityAction timeoutCallback = null) {
            int tag = Instance.lastClientTag++;
            if (!Instance.isConnected) {
                Instance.StartCoroutine(TimeOutInvoke(timeoutCallback));
                return;
            }
            if (!ackPackage.CustomIsEmpty()) {
                NetHandler.AddAckHandler(ackPackage, tag, callBack);
                NetHandler.AddErrorHandler(ackPackage, tag, errorCallback);
                NetHandler.AddTimeOutHandler(ackPackage, tag, Instance.StartCoroutine(
                    TimeOutCheck(ackPackage, tag, timeoutCallback)));
            }
            byte[] serializeMessage = SerializePackage(message.GetType().Name, tag, message);
            self.netProtocol.SendMessage(serializeMessage);
        }

        //public static void AddMessageTimeOut(string ackPackge, string key, UnityAction action) {
        //    NetHandler.AddTimeOutHandler(ackPackge, key, Instance.StartCoroutine(
        //        Instance.TimeOutCheck(ackPackge, key)));
        //}

        /// <summary>
        /// Receive package from server, it will handle package automatic by using
        /// the handle presaved.
        /// </summary>
        private void ReceiveMessage() {
            if (!this.isConnected) {
                return;
            }
            try {
                reply = netProtocol.ReceiveMessage();
                if (reply != null) {
                    string packageName;
                    int tag;
                    IExtensible message = self.DeserializePackage(out packageName, out tag, reply);
                    if (message == null) {
                        return;
                    }
#if UNITY_EDITOR || DEVELOPER
                    StartCoroutine(NetHandler.ExecHandlerDelay(packageName, tag, message));
#else
                    NetHandler.ExecHandler(packageName, tag, message);
#endif
                    //if (VersionConst.IsDeveloper()) {
                    //    StartCoroutine(NetHandler.ExecHandlerDelay(packageName, tag, message));
                    //} else {
                    //    NetHandler.ExecHandler(packageName, tag, message);
                    //}
                }
            } catch (Exception e) {
                if (e is PONetException) {
                    this.Reconnect();
                    // When connection disconnect, just stop fte
                    //FteManager.StopFte();
                } else {
                    throw e;
                }
            }
        }

        private void Reconnect() {
            UIManager.ShowNetCircle();
            this.reconnectReference = 0;
            if (this.heartbeatCoroutine != null) {
                StopCoroutine(this.heartbeatCoroutine);
            }
            this.isConnected = false;
            this.disconnectedAction.InvokeSafe();
            this.InnerConnectToServer();
            //  StartCoroutine(this.Connect());
        }

        //public static int GetLastServerTag() {
        //    return Instance.lastServerTag;
        //}

        private static byte[] SerializePackage(string packageName, int tag, object message) {
#if UNITY_EDITOR || DEVELOPER
            if (!packageName.CustomEquals("HeartbeatReq")) {
                Debug.Log("Send: " + packageName);
            }
#endif
            MemoryStream m = new MemoryStream();
            Serializer.Serialize(m, message);
            m.Position = 0;
            int length = (int)m.Length;
            byte[] buffer = new byte[length];
            m.Read(buffer, 0, length);

            ApiReq req = new ApiReq() {
                Flag = tag,
                ReqName = packageName,
                Data = buffer
            };
            m.Close();

            m = new MemoryStream();
            Serializer.Serialize(m, req);
            m.Position = 0;
            length = (int)m.Length;
            buffer = new byte[length];
            m.Read(buffer, 0, length);

            return buffer;
        }

        // To do : change to private function.
        public IExtensible DeserializePackage(out string packageName, out int tag, byte[] message) {
            MemoryStream m = new MemoryStream(message);
            ApiAck ack = Serializer.Deserialize<ApiAck>(m);
            packageName = string.Concat(char.ToUpper(ack.AckName[0]), ack.AckName.Substring(1));
#if UNITY_EDITOR || DEVELOPER
            if (!packageName.CustomEquals("HeartbeatAck")) {
                Debug.Log("Receive: " + packageName);
            }
#endif
            packageName = packageName.Replace("_", "+");
            tag = ack.Flag;
            m.Close();

            return NetHandler.IsPackageHasExecHandler(packageName) ?
                this.InnerDeserializePackage(ack, packageName) : null;

            //byte[] buffer = ack.Data;

            //m = new MemoryStream(buffer);
            //try {
            //    IExtensible data = Serializer.Deserialize(
            //        System.Type.GetType(string.Concat("Protocol.", packageName)), m
            //    ) as IExtensible;
            //    return data;
            //} catch (Exception e) {
            //    //Debug.LogError(e.Message);
            //    Debug.LogUpload(string.Format("Package: {0} deserialize error: {1}",
            //        packageName, e.ToString()));
            //    return null;
            //}
        }

        private IExtensible InnerDeserializePackage(ApiAck ack, string packageName) {
            byte[] buffer = ack.Data;
            MemoryStream m = new MemoryStream(buffer);
            packageName = packageName.Replace("_", "+");
            try {
                IExtensible data = Serializer.Deserialize(
                    System.Type.GetType(string.Concat("Protocol.", packageName)), m
                ) as IExtensible;
                return data;
            } catch (Exception e) {
                Debug.LogError(e.Message);
                Debug.LogUpload(string.Format("Package: {0} deserialize error: {1}",
                    packageName, e.ToString()));
                return null;
            }
        }

        private static IEnumerator TimeOutInvoke(UnityAction action) {
            yield return YieldManager.EndOfFrame;
            action.InvokeSafe();
            UIManager.ShowTip(LocalManager.GetValue(LocalHashConst.net_timeout), TipType.Warn);
        }

        private static IEnumerator TimeOutCheck(string packageName, int tag, UnityAction timeoutCallback) {
            yield return YieldManager.GetWaitForSeconds(TimeOut);
            timeoutCallback.InvokeSafe();
            NetHandler.RemoveTimeOutHandler(packageName, tag);
            // To do: need remove？
            //NetHandler.RemoveAckHandler(packageName, tag);
            //if (this.isConnected) {
            //    UIManager.ShowTip(LocalManager.GetValue("net_timeout"), TipType.Warn);
            //}
        }

        public static void StopTimeOutCheck(Coroutine coroutine) {
            Instance.StopCoroutine(coroutine);
        }

        public void InnerConnectToServer() {
            this.netProtocol = gameObject.GetComponent<WebSocketProtocol>() ??
                gameObject.AddComponent<WebSocketProtocol>();
            this.netProtocol.Init(this.serverUrl);
            StartCoroutine(this.Connect());
        }

        public static void AddConnectedAction(UnityAction action) {
            Instance.connectedAction += action;
        }

        public static void RemoveConnectedEvent(UnityAction action) {
            Instance.connectedAction -= action;
        }

        public static void RemoveAllConnectedEvent() {
            Instance.connectedAction = null;
        }

        public static void AddDisConnectedAction(UnityAction action) {
            Instance.disconnectedAction += action;
        }

        public static void RemoveDisConnectedEvent(UnityAction action) {
            Instance.disconnectedAction -= action;
        }

        void OnDestroy() {
            if (netProtocol != null) {
                this.netProtocol.Close();
            }
        }

        //void OnApplicationPause(bool pauseStatus) {
        //Debug.LogError("Pause");
        // check net status.
        // reconnect to server.
        // set time out mech.
        //}

        void OnApplicationFocus(bool hasFocus) {
            if (this.isConnected) {
                this.heartbeatReference = 3;
                if (this.heartbeatCoroutine != null) {
                    StopCoroutine(this.heartbeatCoroutine);
                }
                this.heartbeatCoroutine = StartCoroutine(this.HeartbeatReq());
            }

            //#if UNITY_EDITOR
            //            //Test
            //            if (!hasFocus) {
            //                NetManager.Disconnect();
            //            }
            //#endif
        }
    }
}
