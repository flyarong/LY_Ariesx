using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Protocol;
using UnityEngine.Events;

namespace Poukoute {
    public class StoreBtnView : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI txtRemainTime;
        [SerializeField]
        private Image imgTextBG;
        [SerializeField]
        private Image imgStore;

        private string txtTime = string.Empty;
        private System.TimeSpan timeSpan;
        private long remainTime = 0;
        //private UnityAction OnRemainTimeCountDown = null;

        public void SetContent(long remainSeconds) {
            bool stillCountDown = (remainSeconds >= 0);
            this.SetStoreInfo(stillCountDown);
            if (remainSeconds > 0) {
                timeSpan = System.TimeSpan.FromSeconds(remainSeconds);
                if (timeSpan.Days > 0) {
                    txtTime = string.Concat(timeSpan.Days, "d", timeSpan.Hours, "h");
                }
                else if (timeSpan.Hours > 0) {
                    txtTime = string.Concat(timeSpan.Hours, "h", timeSpan.Minutes, "m");
                }
                else {
                    txtTime = string.Concat(timeSpan.Minutes, "m", timeSpan.Seconds, "s");
                    this.remainTime = remainSeconds;
                    StartCoroutine(this.UpdateRemainTimeInfo());
                }
                this.txtRemainTime.text = txtTime;
                this.SetStoreInfo(true);
            }
        }

        private IEnumerator UpdateRemainTimeInfo() {
            yield return YieldManager.GetWaitForSeconds(1);
            timeSpan = System.TimeSpan.FromSeconds(this.remainTime);
            txtTime = string.Concat(timeSpan.Minutes, "m", timeSpan.Seconds, "s");
            this.txtRemainTime.text = txtTime;
            --this.remainTime;
            bool stillCountDown = (this.remainTime >= 0);
            if (stillCountDown) {
                StartCoroutine(this.UpdateRemainTimeInfo());
            }
            else {
                this.SetStoreInfo(false);
            }
        }

        private void SetStoreInfo(bool stillCountDown) {
            this.txtRemainTime.gameObject.SetActiveSafe(stillCountDown);
            this.imgTextBG.gameObject.SetActiveSafe(stillCountDown);
            imgStore.material = stillCountDown ? PoolManager.GetMaterial(MaterialPath.matScan) : null;
            imgTextBG.material = stillCountDown ? PoolManager.GetMaterial(MaterialPath.matScan) : null;
        }

        public void SetStoreBtn(bool isFree) {
            imgStore.material = isFree ? PoolManager.GetMaterial(MaterialPath.matScan) : null;
        }
    }
}