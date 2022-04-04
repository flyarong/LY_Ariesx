using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class UpgradeResourcesItemView : MonoBehaviour {
        public TextMeshProUGUI txtResouceAmount;
        public Image imgResource;

        public bool SetResouceInfo(Resource resource, int resourceAmount, int resouceMax) {
            this.imgResource.sprite = ArtPrefabConf.GetSprite(
                SpritePath.resourceIconPrefix, resource.ToString().ToLower());
            this.txtResouceAmount.text = GameHelper.GetFormatNum(resourceAmount); ;

            if (resourceAmount > resouceMax) {
                this.txtResouceAmount.color = Color.red;
                return  false;
            } else {
                this.txtResouceAmount.color = Color.white;
            }
            return true;
        }
    }
}
