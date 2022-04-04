using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashView : MonoBehaviour {

    void Awake() {
        this.GetComponent<CanvasScaler>().matchWidthOrHeight =
            Camera.main.aspect < (9 / 16f) ? 0 : 1;
    }
}
