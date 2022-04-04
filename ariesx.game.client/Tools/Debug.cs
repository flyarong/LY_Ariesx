using UnityEngine.Events;
using System;
using System.Collections;
using System.IO;
using System.Diagnostics;

public class Debug {
    public static bool isDebug = true;
    public class DebugEvent : UnityEvent<string> { }
    public static DebugEvent debugEvent = new DebugEvent();

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPER")]
    public static void Log(object message, UnityEngine.Object context) {
        if (isDebug) {
            UnityEngine.Debug.Log(message, context);
        }
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPER")]
    public static void LogWarning(object message, UnityEngine.Object context) {
        if (isDebug) {
            UnityEngine.Debug.LogWarning(message, context);
        }
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPER")]
    public static void LogError(object message, UnityEngine.Object context) {
        if (isDebug) {
            UnityEngine.Debug.LogError(message, context);
        }
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPER")]
    public static void Log(object message) {
        if (isDebug) {
            UnityEngine.Debug.Log(message);
        }
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPER")]
    public static void LogWarning(object message) {
        if (isDebug) {
            UnityEngine.Debug.LogWarning(message);
        }
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPER")]
    public static void LogError(object message) {
        if (isDebug) {
            UnityEngine.Debug.LogError(message);
        }
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPER")]
    public static void Logf(string message, params object[] args) {
        if (isDebug) {
            UnityEngine.Debug.LogFormat(message, args);
        }
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPER")]
    public static void LogWarningf(string message, params object[] args) {
        if (isDebug) {
            UnityEngine.Debug.LogWarningFormat(message, args);
        }
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPER")]
    public static void LogErrorf(string message, params object[] args) {
        if (isDebug) {
            UnityEngine.Debug.LogErrorFormat(message, args);
        }
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPER")]
    public static void LogUpload(string message) {
        debugEvent.Invoke(message);
    }
}