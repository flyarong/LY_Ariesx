using UnityEngine;
using System.Collections;

namespace Poukoute {
    abstract public class NetProtocol : MonoBehaviour {
        abstract public void SendMessage(byte[] message);
        abstract public byte[] ReceiveMessage();
        abstract public void Init(string url);
        abstract public void Close();
        abstract public IEnumerator ConnectAsync();
        abstract public IEnumerator Connect();
    }
}
