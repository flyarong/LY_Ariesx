using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public enum TipType {
        Info,
        Notice,
        Warn,
        Error
    }

    public class Tip {
        public string text;
        public TipType type;
        public float angle;
        public Vector3 position;
        public bool clickable;

        public Tip(string text, TipType type, float angle, Vector2 position) {
            this.text = text;
            this.type = type;
            this.angle = angle;
            this.position = position;
            this.clickable = true;
        }

        public Tip(string text, TipType type) {
            this.text = text;
            this.type = type;
            this.clickable = false;
        }
    }

    public enum GlocalTipType {
        Notice,
        Warning,
        None
    }

    public class GameGlobalTips: MonoBehaviour {
        #region ui elements
        [SerializeField]
        private Button btnBackground;
        [SerializeField]
        private Transform pnlTip;
        [SerializeField]
        private CanvasGroup warningTipCG;
        [SerializeField]
        private TextMeshProUGUI txtWarningTip;
        [SerializeField]
        private CanvasGroup noticeTipCG;
        [SerializeField]
        private TextMeshProUGUI txtNoticeTip;
        [SerializeField]
        private Transform pnlWifi;
        [SerializeField]
        private Transform pnlCircle;
        [SerializeField]
        private Image imgCircle;
        #endregion

        public Coroutine tipCoroutine;

        public void ShowTip(string text, TipType type, float waitTime) {
            if (this.tipCoroutine != null) {
                StopCoroutine(this.tipCoroutine);
            }
            this.tipCoroutine =
                StartCoroutine(this.ShowTip(new Tip(text, type), waitTime));
        }

        public IEnumerator ShowTip(Tip tip, float waitTime) {
            this.SetTip(tip);
            yield return YieldManager.GetWaitForSeconds(waitTime);
            AnimationManager.Animate(this.pnlTip.gameObject, "Hide",
                () => UIManager.HideUI(this.pnlTip.gameObject));
            this.tipCoroutine = null;
        }

        public void ShowWifiAlert() {
            AnimationManager.Animate(this.pnlWifi.gameObject, "Twink", loop: true, needRestart: false);
            UIManager.ShowUI(this.pnlWifi.gameObject);
        }

        public void HideWifiAlert() {
            AnimationManager.Stop(this.pnlWifi.gameObject);
            UIManager.HideUI(this.pnlWifi.gameObject);
        }

        private IEnumerator ShowWifiAlertDelay() {
            yield return YieldManager.GetWaitForSeconds(1);
        }

        public void ShowNetCircle() {
            this.btnBackground.onClick.RemoveAllListeners();
            this.btnBackground.gameObject.SetActive(true);
            UIManager.ShowUI(this.pnlCircle.gameObject);
            AnimationManager.Animate(this.imgCircle.gameObject, "Rotate", isOffset: true);
        }

        public void HideNetCircle() {
            this.btnBackground.gameObject.SetActive(false);
            UIManager.HideUI(this.pnlCircle.gameObject);
        }

        private void SetTip(Tip tip) {
            switch (tip.type) {
                case TipType.Error:
                case TipType.Warn:
                    this.SetWarningTip(tip);
                    break;
                case TipType.Info:
                default:
                    this.SetNoticeTip(tip);
                    break;
            }

            UIManager.ShowUI(this.pnlTip.gameObject);
            AnimationManager.Animate(this.pnlTip.gameObject, "Show");
        }
        private void SetTipSubViewVisible(GlocalTipType tipType) {
            // UIManager.SetUICanvasGroupEnable(this.normalTipCG, tipType == GlocalTipType.Normal);
            UIManager.SetUICanvasGroupEnable(this.warningTipCG, tipType == GlocalTipType.Warning);
            UIManager.SetUICanvasGroupEnable(this.noticeTipCG, tipType == GlocalTipType.Notice);
        }

        private void SetNoticeTip(Tip tip) {
            this.SetTipSubViewVisible(GlocalTipType.Notice);
            this.txtNoticeTip.text = tip.text;
        }

        private void SetWarningTip(Tip tip) {
            this.SetTipSubViewVisible(GlocalTipType.Warning);
            this.txtWarningTip.text = tip.text;
        }

        //private void SetNormalTip(Tip tip) {
        //    this.SetTipSubViewVisible(GlocalTipType.Normal);
        //    if (tip.clickable) {
        //        Vector2 origin = MapUtils.UIToWorldPoint(
        //            this.pnlTip.GetComponent<RectTransform>().anchoredPosition
        //        );
        //        Vector2 direction = (Vector2)tip.position - origin;
        //        float angle = Vector2.Angle(Vector2.up, direction);
        //        if (direction.x > 0) {
        //            angle = -angle;
        //        }
        //        Transform imgArrow = null;
        //        if (angle > 0) {
        //            imgArrow = this.imgArrowLeft;
        //            this.imgArrowLeft.gameObject.SetActiveSafe(false);
        //            this.imgArrowRight.gameObject.SetActiveSafe(false);
        //        } else {
        //            imgArrow = this.imgArrowRight;
        //            this.imgArrowLeft.gameObject.SetActiveSafe(false);
        //            this.imgArrowRight.gameObject.SetActiveSafe(false);
        //        }
        //        imgArrow.transform.eulerAngles = Vector3.zero;
        //        origin = MapUtils.UIToWorldPoint(
        //             imgArrow.GetComponent<RectTransform>().anchoredPosition
        //         );
        //        direction = (Vector2)tip.position - origin;
        //        angle = Vector2.Angle(Vector2.up, direction);
        //        if (direction.x > 0) {
        //            angle = -angle;
        //        }
        //        imgArrow.eulerAngles = Vector3.forward * (angle);
        //        UIManager.ShowUI(this.pnlTip.gameObject);
        //        AnimationManager.Animate(this.pnlTip.gameObject, "Show");
        //        this.btnBubble.onClick.RemoveAllListeners();
        //        this.btnBubble.onClick.AddListener(() => {
        //            TriggerManager.Invoke(Trigger.CameraMove, MapUtils.PositionToCoordinate(tip.position));
        //        });
        //    } else {
        //        this.imgArrowLeft.gameObject.SetActiveSafe(false);
        //        this.imgArrowRight.gameObject.SetActiveSafe(false);
        //    }
        //    this.txtNormalTip.text = tip.text;
        //}


    }
}
