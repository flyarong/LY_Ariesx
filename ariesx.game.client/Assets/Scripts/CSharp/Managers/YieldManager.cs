using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions.Comparers;
using Protocol;

// Usage:
//    yield return new WaitForEndOfFrame();     =>      yield return YieldManager.EndOfFrame;
//    yield return new WaitForFixedUpdate();    =>      yield return YieldManager.FixedUpdate;
//    yield return new WaitForSeconds(1.0f);    =>      yield return YieldManager.GetWaitForSeconds(1.0f);

// http://forum.unity3d.com/threads/c-coroutine-waitforseconds-garbage-collection-tip.224878/

namespace Poukoute {
    public static class YieldManager {
        public static bool Enabled = true;

        public static int _internalCounter = 0; // counts how many times the app yields

        // WARNING: 
        //      (Gu Lu) The comments below are incorrect in Unity 5.5.0
        //          - float DOES NOT needs customized IEqualityComparer (but enums and structs does)
        //      however all these lines are kept to help later reader to share this 
        //         knowledge (for education purpose only). 
        //------------------------------------------------------------------
        ///////////////////// obsoleted code begins \\\\\\\\\\\\\\\\\\\\\\\\
        //
        //// dictionary with a key of ValueType will box the value 
        //// to perform comparison / hash code calculation while scanning the hashtable.
        //// here we implement IEqualityComparer<float> and pass it to your dictionary to avoid that GC
        //class FloatComparer : IEqualityComparer<float>
        //{
        //    bool IEqualityComparer<float>.Equals(float x, float y)
        //    {
        //        return x == y;
        //    }
        //    int IEqualityComparer<float>.GetHashCode(float obj)
        //    {
        //        return obj.GetHashCode();
        //    }
        //}
        //\\\\\\\\\\\\\\\\\\\\\\\\ obsoleted code ends /////////////////////
        //------------------------------------------------------------------

        static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
        public static WaitForEndOfFrame EndOfFrame {
            get { _internalCounter++; return Enabled ? _endOfFrame : new WaitForEndOfFrame(); }
        }

        static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
        public static WaitForFixedUpdate FixedUpdate {
            get { _internalCounter++; return Enabled ? _fixedUpdate : new WaitForFixedUpdate(); }
        }

        public static WaitForSeconds GetWaitForSeconds(float seconds) {
            _internalCounter++;

            if (!Enabled)
                return new WaitForSeconds(seconds);

            WaitForSeconds wfs;
            if (!_waitForSecondsYielders.TryGetValue(seconds, out wfs))
                _waitForSecondsYielders.Add(seconds, wfs = new WaitForSeconds(seconds));
            return wfs;
        }

        //public static WaitUntil GetWaitUtil(System.Func<bool> func) {
        //    _internalCounter++;

        //    if (!Enabled)
        //        return new WaitUntil(func);

        //    WaitUntil wu;
        //    if (!_waitUntilYielders.TryGetValue(fun)
        //}

        public static void ClearWaitForSeconds() {
            _waitForSecondsYielders.Clear();
        }

        // To do: when call twice.
        public static IEnumerator DelayCallAction(UnityAction action, float delay) {
            yield return YieldManager.GetWaitForSeconds(delay);
            action.InvokeSafe();
        }

        public static IEnumerator EndOfFrameCallAction(UnityAction action) {
            yield return YieldManager.EndOfFrame;
            action.InvokeSafe();
        }

        static Dictionary<float, WaitForSeconds> _waitForSecondsYielders =
                new Dictionary<float, WaitForSeconds>(100, new FloatComparer());

        
    }
}