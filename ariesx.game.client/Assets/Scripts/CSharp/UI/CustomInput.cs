using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomInput : BaseInput {
    public override int touchCount {
        get {
            return Mathf.Min(1, Input.touchCount);
        }
    }

    public int pinchCount {
        get {
            return Input.touchCount;
        }
    }
}
