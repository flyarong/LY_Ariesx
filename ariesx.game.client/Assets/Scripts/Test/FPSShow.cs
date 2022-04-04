using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Poukoute {
    public class FPSShow : MonoBehaviour {
        private Text fpsText;
        //private string formatStr;

        void Awake() {
            this.fpsText = this.GetComponent<Text>();
            //this.formatStr = this.fpsText.text;
            StartCoroutine(this.ShowFps());
        }

        IEnumerator ShowFps() {
            while (true) {
                //yield return new WaitForSeconds(0.2f);
                yield return YieldManager.GetWaitForSeconds(0.2f);
                this.fpsText.text = string.Format("FPS: {0}", (1.0f / Time.unscaledDeltaTime).ToString("F2"));
            }
        }
    }
}
