using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceLogoItemView : MonoBehaviour {
        public Button btnInfo;
        public Image imgLogo;

        public void SetLogoInfo(int emblem, UnityAction callback) {
            this.btnInfo.onClick.RemoveAllListeners();
            this.btnInfo.onClick.AddListener(callback);
            this.imgLogo.sprite = ArtPrefabConf.GetAliEmblemSprite(emblem);
        }
    }
}