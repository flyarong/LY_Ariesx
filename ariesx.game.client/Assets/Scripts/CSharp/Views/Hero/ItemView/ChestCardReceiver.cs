using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChestCardReceiver : MonoBehaviour {
    public UnityAction onPlayStart = null;
    public UnityAction onPlayEnd = null;
    public UnityAction onShowDetail = null;
    public UnityAction onShowName = null;
    public UnityAction onShowSlider = null;
    public UnityAction onShowStar = null;
    public UnityAction onHaloFade = null;
    public UnityAction onPlayMiddle = null;

    public void OnPlayEnd() {
        this.onPlayEnd.InvokeSafe();
    }

    public void OnPlayMiddle() {
        this.onPlayMiddle.InvokeSafe();
    }

    public void OnShowDetail() {
        this.onShowDetail.InvokeSafe();
    }

    public void OnShowName() {
        this.onShowName.InvokeSafe();
    }

    public void OnShowSlider() {
        this.onShowSlider.InvokeSafe();
    }

    public void OnShowStar() {
        this.onShowStar.InvokeSafe();
    }

    public void OnHaloFade() {
        this.onHaloFade.InvokeSafe();
    }

    public void OnPlayStart() {
        this.onPlayStart.InvokeSafe();
    }
}
