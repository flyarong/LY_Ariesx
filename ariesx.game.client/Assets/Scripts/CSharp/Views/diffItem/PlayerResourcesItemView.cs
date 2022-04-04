using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class PlayerResourcesItemView : MonoBehaviour {
        public TextMeshProUGUI txtAmount;
        public Image imgResource;

        public void SetContent(Resource resource, int amount) {
            this.imgResource.sprite = 
                ArtPrefabConf.GetSprite(SpritePath.resourceIconPrefix, resource.ToString().ToLower());
            this.txtAmount.text = GameHelper.GetFormatNum(amount);
        }
    }
}
