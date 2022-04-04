using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleCamera : MonoBehaviour {

    public static UnityEvent onRenderEvent = new UnityEvent();

    public void OnPostRender() {
        UnityEngine.Debug.Break();
        //onRenderEvent.Invoke();
    }
}
