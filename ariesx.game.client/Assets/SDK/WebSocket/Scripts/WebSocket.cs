using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Threading;
using Poukoute;

public class WebSocket : Stream {
    //private bool initialized = false;
    public delegate void WebSocketEvent(ref byte[] buffer);
    public WebSocketEvent OnRecv {
        get; set;
    }

    public override bool CanWrite {
        get {
            throw new NotImplementedException();
        }
    }
    public override bool CanSeek {
        get {
            throw new NotImplementedException();
        }
    }
    public override bool CanTimeout {
        get {
            return base.CanTimeout;
        }
    }

    public override bool CanRead {
        get {
            throw new NotImplementedException();
        }
    }

    public override long Length {
        get {
            throw new NotImplementedException();
        }
    }

    public override long Position {
        get {
            throw new NotImplementedException();
        }

        set {
            throw new NotImplementedException();
        }
    }

    public byte[] readBuffer = null;

    public override void Write(byte[] buffer, int offset, int count) {
        // Debug.LogError("Write: " + Encoding.UTF8.GetString(buffer));
        byte[] writeBuffer = new byte[count];
        Array.ConstrainedCopy(buffer, offset, writeBuffer, 0, count);
        this.Send(writeBuffer);
    }

    public override int Read(byte[] buffer, int offset, int count) {
        Array.Clear(buffer, offset, count);

        while (this.readBuffer == null && m_Error == null) {
            readBuffer = this.Recv();
            if (!m_IsConnected) {
                Debug.LogError("Connect Error");
                throw new IOException();
            }
            Thread.Sleep(8);
        }

        if (this.m_Error != null) {
            throw new IOException();
        }

        this.OnRecv.Invoke(ref readBuffer);
        count = Mathf.Min(count, this.readBuffer.Length - offset);

        Array.ConstrainedCopy(this.readBuffer, offset, buffer, offset, count);

        if (count == this.readBuffer.Length - offset) {
            this.readBuffer = null;
        }
        // Debug.LogError("Receive: " + Encoding.UTF8.GetString(buffer));
        return count;

    }

    public override long Seek(long offset, SeekOrigin origin) {
        throw new NotImplementedException();
    }

    public override void SetLength(long value) {
        throw new NotImplementedException();
    }

    public override void Flush() {
        throw new NotImplementedException();
    }

    private Uri mUrl;

    public WebSocket(Uri url) {
        mUrl = url;

        string protocol = mUrl.Scheme;
        if (!protocol.Equals("ws", StringComparison.Ordinal) && 
            !protocol.Equals("wss", StringComparison.Ordinal))
            throw new ArgumentException("Unsupported protocol: " + protocol);
    }

    public void SendString(string str) {
        Send(Encoding.UTF8.GetBytes(str));
    }

    public string RecvString() {
        byte[] retval = Recv();
        if (retval == null)
            return null;
        return Encoding.UTF8.GetString(retval);
    }

#if UNITY_WEBGL && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern int SocketCreate (string url);

	[DllImport("__Internal")]
	private static extern int SocketState (int socketInstance);

	[DllImport("__Internal")]
	private static extern void SocketSend (int socketInstance, byte[] ptr, int length);

	[DllImport("__Internal")]
	private static extern void SocketRecv (int socketInstance, byte[] ptr, int length);

	[DllImport("__Internal")]
	private static extern int SocketRecvLength (int socketInstance);

	[DllImport("__Internal")]
	private static extern void SocketClose (int socketInstance);

	[DllImport("__Internal")]
	private static extern int SocketError (int socketInstance, byte[] ptr, int length);

	int m_NativeRef = 0;

	public void Send(byte[] buffer)
	{
		SocketSend (m_NativeRef, buffer, buffer.Length);
	}

	public byte[] Recv()
	{
		int length = SocketRecvLength (m_NativeRef);
		if (length == 0)
			return null;
		byte[] buffer = new byte[length];
		SocketRecv (m_NativeRef, buffer, length);
		return buffer;
	}

	public IEnumerator Connect()
	{
		m_NativeRef = SocketCreate (mUrl.ToString());

		while (SocketState(m_NativeRef) == 0)
			yield return null;
	}

    public bool Start() {
        m_NativeRef = SocketCreate (mUrl.ToString());
        
        Int32 startTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        Int32 endTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        while(SocketState(m_NativeRef) == 0 && endTime - startTime < 3) { 
            Thread.Sleep(6);
            endTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        if (endTime - startTime >= 3) {
            throw new IOException();
        }
        return true;
    }

 
	public void Close()
	{
		SocketClose(m_NativeRef);
	}

	public string error
	{
		get {
			const int bufsize = 1024;
			byte[] buffer = new byte[bufsize];
			int result = SocketError (m_NativeRef, buffer, bufsize);

			if (result == 0)
				return null;

			return Encoding.UTF8.GetString (buffer);				
		}
	}
#else
    WebSocketSharp.WebSocket m_Socket;

    Queue<byte[]> m_Messages = new Queue<byte[]>();
    bool m_IsConnected = false;
    string m_Error = null;

    public IEnumerator Connect() {
        m_Socket = new WebSocketSharp.WebSocket(mUrl.ToString());

        m_Socket.OnMessage += (sender, e) => m_Messages.Enqueue(e.RawData);
        m_Socket.OnOpen += (sender, e) => m_IsConnected = true;
        m_Socket.OnError += (sender, e) => m_Error = e.Message;
        m_Socket.ConnectAsync();
        while (!m_IsConnected && m_Error == null) {
            yield return null;
        }
        if (m_Error != null) {
            throw new PONetException();
        }

    }

    public bool Start() {
        m_Socket = new WebSocketSharp.WebSocket(mUrl.ToString(), new string[] { "xmpp" });

        m_Socket.OnMessage += (sender, e) => m_Messages.Enqueue(e.RawData);
        m_Socket.OnOpen += (sender, e) => m_IsConnected = true;
        m_Socket.OnError += (sender, e) => m_Error = e.Message;
        m_Socket.ConnectAsync();
        Int32 startTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        Int32 endTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        while (!m_IsConnected && m_Error == null && endTime - startTime < 3) {
            endTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Thread.Sleep(16);
        }
        if (endTime - startTime >= 3) {
            throw new IOException();
        }
        return true;
    }

    public IEnumerator Test() {
        //yield return new WaitForSeconds(2f);
        yield return YieldManager.GetWaitForSeconds(2f);
        Debug.LogError("After Waiting for 2 seconds");
    }

    public void Send(byte[] buffer) {
        m_Socket.Send(buffer);
    }

    public byte[] Recv() {
        if (m_Messages.Count == 0)
            return null;
        return m_Messages.Dequeue();
    }

    public override void Close() {
        m_Socket.Close();
        m_IsConnected = false;
    }

    public string error {
        get {
            return m_Error;
        }
    }


#endif
}