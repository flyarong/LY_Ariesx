using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class CampainGuideItemView : MonoBehaviour {
        public TextMeshProUGUI txtDesc;
        public TextMeshProUGUI txtPoint;
        public Button btnItem;

        public void SetContent(string content, string point, UnityAction onItemClick) {
            this.txtDesc.text = content;
            this.txtPoint.text = point.ToString();
            this.btnItem.onClick.RemoveAllListeners();
            this.btnItem.onClick.AddListener(onItemClick);
        }
    }
}
