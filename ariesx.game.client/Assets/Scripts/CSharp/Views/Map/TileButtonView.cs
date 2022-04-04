using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class TileButtonView : MonoBehaviour {
        [SerializeField]
        private Image imgIcon;
        [SerializeField]
        private TextMeshProUGUI txtBtn;
        [SerializeField]
        public CustomButton button;

        private TileButtonType buttonType = TileButtonType.None;
        private TileButtonType ButtonType {
            set {
                if (this.buttonType != value) {
                    this.buttonType = value;
                    this.OnButtonTypeChange();
                }
            }
        }

        public void SetBtnType(TileButtonType btnType, UnityAction callback, bool gray, string label) {
            this.ButtonType = btnType;
            if (!string.IsNullOrEmpty(label)) {
                this.txtBtn.text = label;
            }
            this.button.Grayable = gray;
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(callback);
        }

        private void OnButtonTypeChange() {
            string buttonTypeStr = buttonType.ToString().ToLower();
            this.imgIcon.sprite = ArtPrefabConf.GetSprite(
                SpritePath.tileButtonPrefix,buttonTypeStr);
            this.txtBtn.text = LocalManager.GetValue("button_tile_", buttonTypeStr);
        }
    }
}
