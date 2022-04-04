using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Poukoute {
    public class DebugView : MonoBehaviour {
        private GameObject ui;
        private Button btnNewPlayer;
        
        void Awake() {
            this.ui = this.gameObject;
            this.btnNewPlayer = this.ui.transform.Find("BtnNewPlayer").GetComponent<Button>();
            this.btnNewPlayer.onClick.RemoveAllListeners();
            this.btnNewPlayer.onClick.AddListener(this.OnBtnNewPlayer);
        }

        private void OnBtnNewPlayer() {
            PlayerPrefs.SetString("udid", System.Guid.NewGuid().ToString());
            GameManager.RestartGame();
        }
    }
}