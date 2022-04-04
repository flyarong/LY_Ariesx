using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {

    public class WebSocketProtocol : NetProtocol {
        private WebSocket webConn;
        
        

        // Use this for initialization

        override public void Init(string url) {
            webConn = new WebSocket(new Uri(url));
        }

        override public void SendMessage(byte[] message) {
            webConn.Send(message);
        }

        override public byte[] ReceiveMessage() {
            if (webConn == null) {
                return null;
            }
            byte[] reply = webConn.Recv();
            if (webConn.error != null) {
                Debug.LogError(webConn.error);
                throw new PONetException();
            } else if (reply != null) {
                return reply;
            }
            return null;
        }

        override public  void Close() {
            this.webConn.Close();
        }

        override public IEnumerator ConnectAsync() {
            IEnumerator enumerator = webConn.Connect();
            object ret = null;
            while (true) {
                try {
                    if (!enumerator.MoveNext()) {
                        break;
                    }
                    ret = enumerator.Current;
                } catch(Exception e) {
                    throw e;
                } 
                yield return ret;
            }
        }

        override public IEnumerator Connect() {
            yield return null;
        }

    }
}

