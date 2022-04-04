using UnityEngine;
using UnityEngine.UI;
using Protocol;
using TMPro;

namespace Poukoute {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTextMeshPro : MonoBehaviour {
        public string key;
        
        void Start() {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
#if UNITY_EDITOR
            string local = LocalManager.GetValue(key);
            if (local.CustomIsEmpty()) {
                Debug.LogWarningf(
                    "{0} {1} not valid!  {2}",
                    this.transform.name,
                    key,
                    key.CustomGetHashCode()
                );
            }
#endif
            text.text = LocalManager.GetValue(key);
        }
    }
}
