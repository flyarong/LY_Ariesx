using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class LocalizedText : MonoBehaviour {

        public string key;
        [SerializeField]
        List<string> paramList;

        // Use this for initialization
        void Start() {
            for(int i = 0; i < paramList.Count; i++) {
                paramList[i] = LocalManager.GetValue(paramList[i]); 
            }

            Text text = GetComponent<Text>();
            text.text = string.Format(
                LocalManager.GetValue(key),
                paramList
            );
        }

    }
}
