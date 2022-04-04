
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class ResourceItemView : MonoBehaviour {
        public Image imgIcon;
        public TextMeshProUGUI txtNumber;

        public void SetResource(Resource type, int value) {
            this.SetResource(GameHelper.LowerFirstCase(type.ToString()), value);
        }

        public void SetResource(Resource type, string value) {
            this.SetResource(GameHelper.LowerFirstCase(type.ToString()), value);
        }

        public void SetResource(string type, int value) {
            this.SetResource(type, GameHelper.GetFormatNum(value));
        }

        public void SetResource(string type, string value) {
            this.txtNumber.text = value;
           // this.txtNumber.color = Color.white;
            this.imgIcon.sprite = ArtPrefabConf.GetSprite(
                SpritePath.resourceIconPrefix, type);
        }

        public void SetItemDetail(string spritePath, int value) {
            this.txtNumber.text = GameHelper.GetFormatNum(value);
           // this.txtNumber.color = Color.white;
            this.imgIcon.sprite = ArtPrefabConf.GetSprite(spritePath);
        }

        public void SetItemDetail(string spritePath, string amount) {
            this.txtNumber.text = amount;
           // this.txtNumber.color = Color.white;
            this.imgIcon.sprite = ArtPrefabConf.GetSprite(spritePath);
        }
        
    }
}
