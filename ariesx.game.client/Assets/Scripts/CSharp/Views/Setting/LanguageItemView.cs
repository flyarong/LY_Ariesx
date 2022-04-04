using Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class LanguageItemView : MonoBehaviour {
        #region UI elements
        [SerializeField]
        private Button btnItem;
        [SerializeField]
        private TextMeshProUGUI txtLanguage;
        [SerializeField]
        private GameObject chosenMark;
        #endregion

        public void SetContent(string language, UnityAction OnItemClick, bool isCurrentAllianceLanguage = false) {
            this.txtLanguage.text = language;
            this.chosenMark.SetActiveSafe(language.CustomEquals(LocalManager.Language));

            this.btnItem.onClick.RemoveAllListeners();
            this.btnItem.onClick.AddListener(OnItemClick);
            this.btnItem.onClick.AddListener(() => {
                this.chosenMark.SetActiveSafe(true);
            });

            if (isCurrentAllianceLanguage)
                this.chosenMark.SetActiveSafe(isCurrentAllianceLanguage);
        }

        public void SetChosenStatus(string chosenLanguage) {
            this.chosenMark.SetActiveSafe(
                chosenLanguage.CustomEquals(this.txtLanguage.text));
        }
    }
}
