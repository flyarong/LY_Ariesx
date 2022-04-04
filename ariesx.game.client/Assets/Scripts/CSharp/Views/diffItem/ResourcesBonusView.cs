using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class ResourcesBonusView : MonoBehaviour {
        public TextMeshProUGUI txtResouceAmount;
        public TextMeshProUGUI txtResouceBonus;
        public TextMeshProUGUI txtResourceName;
        public Image imgResource;

        public void SetResouceInfo(string spritePath, string nameText, string currentValue, string bonus) {
            this.imgResource.sprite = ArtPrefabConf.GetSprite(spritePath);
            this.txtResourceName.text = nameText;

            this.txtResouceAmount.text = currentValue;
            this.txtResouceBonus.text = bonus;
        }
    }
}
