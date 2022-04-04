using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {

    public delegate void ReqAction();
    public delegate void AckHandler(IExtensible message);
    public delegate void NtfHandler(IExtensible message);
    public delegate void HttpHandler(WWW result);

    public delegate void ErrorHandler();

    public class NetEvent : UnityEvent<IExtensible> { }

    public class NetHandler {

        private static NetHandler self;
        private NetHandler() { }
        public static NetHandler Instance {
            get {
                if (self == null) {
                    self = new NetHandler();
                    self.InitModelHandler();
                }
                return self;
            }
        }

        public Dictionary<string, NetEvent> dataHandleDict = new Dictionary<string, NetEvent>();
        public Dictionary<string, NetEvent> ntfHandlerDict = new Dictionary<string, NetEvent>();
        public Dictionary<string, Dictionary<int, UnityAction<IExtensible>>> ackHandlerDict =
            new Dictionary<string, Dictionary<int, UnityAction<IExtensible>>>();
        public Dictionary<string, Dictionary<int, UnityAction<IExtensible>>> errorHandlerDict =
            new Dictionary<string, Dictionary<int, UnityAction<IExtensible>>>();
        public Dictionary<string, Dictionary<int, Coroutine>> timeOutHandlerDict =
            new Dictionary<string, Dictionary<int, Coroutine>>();

        private void InitModelHandler() {
        }

        public static void AddDataHandler(string packageName, UnityAction<IExtensible> handler) {
            if (handler != null) {
                NetEvent netEvent;
                if (!Instance.dataHandleDict.TryGetValue(packageName, out netEvent)) {
                    netEvent = new NetEvent();

                    Instance.dataHandleDict.Add(packageName, netEvent);
                }
                netEvent.AddListener(handler);
            }
        }

        public static void RemoveDataHandler(string packageName, UnityAction<IExtensible> handler) {
            if (handler != null) {
                NetEvent netEvent;
                if (Instance.dataHandleDict.TryGetValue(packageName, out netEvent)) {
                    netEvent.RemoveListener(handler);
                }
            }
        }

        // To do: need to know which package can duplicate send.
        public static void AddAckHandler(string packageName, int tag, UnityAction<IExtensible> handler) {
            if (handler != null) {
                Dictionary<int, UnityAction<IExtensible>> netActionDict;
                if (!Instance.ackHandlerDict.TryGetValue(packageName, out netActionDict)) {
                    netActionDict = new Dictionary<int, UnityAction<IExtensible>>();
                    Instance.ackHandlerDict.Add(packageName, netActionDict);
                }
                netActionDict[tag] = handler;
            }
        }

        private static void RemoveAckHandler(string packageName, int tag) {
            Dictionary<int, UnityAction<IExtensible>> netActionDict;
            if (Instance.ackHandlerDict.TryGetValue(packageName, out netActionDict)) {
                if (netActionDict.ContainsKey(tag)) {
                    netActionDict.Remove(tag);
                }
                if (netActionDict.Count == 0) {
                    Instance.ackHandlerDict.Remove(packageName);
                }
            }
        }

        public static void AddErrorHandler(string packageName, int tag,
            UnityAction<IExtensible> errorHandler) {
            if (errorHandler != null) {
                Dictionary<int, UnityAction<IExtensible>> netActionDict;
                if (!Instance.errorHandlerDict.TryGetValue(packageName, out netActionDict)) {
                    netActionDict = new Dictionary<int, UnityAction<IExtensible>>();
                    Instance.errorHandlerDict.Add(packageName, netActionDict);
                }
                netActionDict[tag] = errorHandler;
            }
        }

        public static void AddTimeOutHandler(string packageName, int tag, Coroutine timeOutHandler) {
            Dictionary<int, Coroutine> coroutineDict;
            if (!Instance.timeOutHandlerDict.TryGetValue(packageName, out coroutineDict)) {
                coroutineDict = new Dictionary<int, Coroutine>();
                Instance.timeOutHandlerDict.Add(packageName, coroutineDict);
            }
            coroutineDict[tag] = timeOutHandler;
        }

        public static void RemoveTimeOutHandler(string packageName, int tag) {
            Dictionary<int, Coroutine> coroutineDict;
            if (Instance.timeOutHandlerDict.TryGetValue(packageName, out coroutineDict)) {
                Coroutine coroutine;
                if (coroutineDict.TryGetValue(tag, out coroutine)) {
                    //Debug.LogError(packageName + " " + tag);
                    NetManager.StopTimeOutCheck(coroutine);
                    coroutineDict.Remove(tag);
                    if (coroutineDict.Count == 0) {
                        Instance.timeOutHandlerDict.Remove(packageName);
                    }
                }
            }
        }

        public static void RemoveTimeOutHandler(string packageName) {
            Dictionary<int, Coroutine> coroutineDict;
            if (Instance.timeOutHandlerDict.TryGetValue(packageName, out coroutineDict)) {
                foreach (var pair in coroutineDict) {
                    NetManager.StopTimeOutCheck(pair.Value);
                }
                coroutineDict.Clear();
                if (coroutineDict.Count == 0) {
                    Instance.timeOutHandlerDict.Remove(packageName);
                }
            }
        }

        public static void AddNtfHandler(string packageName, UnityAction<IExtensible> handler) {
            if (handler != null) {
                NetEvent netEvent;
                if (!Instance.ntfHandlerDict.TryGetValue(packageName, out netEvent)) {
                    netEvent = new NetEvent();
                    Instance.ntfHandlerDict.Add(packageName, netEvent);
                }
                netEvent.AddListener(handler);
            }
        }

        public static void RemoveNtfHandler(string packageName, UnityAction<IExtensible> handler) {
            if (handler != null) {
                NetEvent netEvent;
                if (Instance.ntfHandlerDict.TryGetValue(packageName, out netEvent)) {
                    netEvent.RemoveListener(handler);
                }
            }
        }

        /* for protobuf-net in c# */
        public static void ExecHandler(string packageName, int tag, IExtensible message) {
            UnityAction<IExtensible> action;
            Dictionary<int, UnityAction<IExtensible>> actionDict;
            if (packageName.CustomEquals("ErrorAck")) {
                ErrorAck errorAck = message as ErrorAck;
                string referAck = errorAck.ReferAck;
                RemoveAckHandler(referAck, tag);
                RemoveTimeOutHandler(referAck, tag);
                Debug.LogError(errorAck.Error);
                if (Instance.errorHandlerDict.TryGetValue(referAck, out actionDict)) {
                    if (actionDict.TryGetValue(tag, out action)) {
                        actionDict.Remove(tag);
                        action.Invoke(message);
                    }
                    if (actionDict.Count == 0) {
                        Instance.errorHandlerDict.Remove(referAck);
                    }
                }
            }
            NetEvent handler;
            if (Instance.dataHandleDict.TryGetValue(packageName, out handler)) {
                handler.Invoke(message);
            }

            if (Instance.ackHandlerDict.TryGetValue(packageName, out actionDict)) {
                if (actionDict.TryGetValue(tag, out action)) {
                    action.Invoke(message);
                    RemoveTimeOutHandler(packageName, tag);
                }
                return;
            }

            if (Instance.ntfHandlerDict.TryGetValue(packageName, out handler)) {
                handler.Invoke(message);
            }
        }

        public static bool IsPackageHasExecHandler(string packageName) {
            return (Instance.dataHandleDict.ContainsKey(packageName) ||
                Instance.ackHandlerDict.ContainsKey(packageName) ||
                Instance.ntfHandlerDict.ContainsKey(packageName));
        }

        public static IEnumerator ExecHandlerDelay(string packageName, int tag, IExtensible message) {
#if UNITY_EDITOR || DEVELOPER
            yield return YieldManager.GetWaitForSeconds(0f);
#else
            yield return YieldManager.GetWaitForSeconds(0f);
#endif
            ExecHandler(packageName, tag, message);
        }
    }
}