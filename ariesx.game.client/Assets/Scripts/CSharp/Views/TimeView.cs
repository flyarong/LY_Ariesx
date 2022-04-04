using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Poukoute {
    public class TimeView : MonoBehaviour {
        #region UI
        public Transform hour;
        public Transform min;
        #endregion

        // Update is called once per frame
        public void SetTime() {
            long time = RoleManager.GetCurrentUtcTime();
            System.TimeSpan timeSpan = System.TimeSpan.FromMilliseconds(time);
            int min = timeSpan.Minutes;
            int hour = timeSpan.Hours;
            this.hour.rotation = Quaternion.Euler(0, 0, hour / 12f * -360);
            this.min.rotation = Quaternion.Euler(0, 0, min / 60f * -360);
        }
    }
}
