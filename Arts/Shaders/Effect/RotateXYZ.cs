
using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// \class RotateXYZ
// 
// \brief 
// 
///////////////////////////////////////////////////////////////////////////////

public class RotateXYZ : MonoBehaviour {

    public float XSpeed = 0.0f;
    public float YSpeed = 0.0f;
    public float ZSpeed = 0.0f;

    void Update () {
        transform.Rotate ( new Vector3 ( XSpeed, YSpeed, ZSpeed ) * Time.deltaTime );
    }

    // DISABLE { 
    // void OnBecameInvisible() {
    //     enabled = false;
    // }

    // void OnBecameVisible() {
    //     enabled = true;
    // }
    // } DISABLE end 
}
