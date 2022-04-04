using UnityEngine;
using TMPro;

namespace Poukoute {
    public class CountDownDayView : MonoBehaviour {
        public TextMeshProUGUI txtDay;

        public void SetContent(int days) {
            this.txtDay.text = days.ToString();
        }
    }
}
