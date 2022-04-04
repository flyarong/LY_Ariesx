using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Protocol;

namespace Poukoute {
    public class FteTrigger : MonoBehaviour {
        public UnityAction onTriggerInvoke = null;
        
        public void TiggerInvoke() {
            this.onTriggerInvoke.InvokeSafe();
        }
    }
}
