using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public class ChestView : MonoBehaviour {
        public UnityEvent onStartEnd = new UnityEvent();
        public UnityEvent onOpenEnd = new UnityEvent();
        public UnityEvent onOpenPlay = new UnityEvent();
        
        public void OnStartEnd() {
            this.onStartEnd.InvokeSafe();
        }

        public void OnOpenEnd() {
            this.onOpenEnd.InvokeSafe();
        }

        public void OnOpenPlay() {
            this.onOpenPlay.InvokeSafe();
        }
    }
}
