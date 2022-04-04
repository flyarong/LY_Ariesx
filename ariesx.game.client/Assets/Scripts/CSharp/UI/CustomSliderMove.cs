using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Poukoute {
    public class CustomSliderMove : MonoBehaviour {
        private SpriteRenderer customRender;

        // To do: need call once the image sprite is change.
        void Awake() {
            this.customRender = this.GetComponent<SpriteRenderer>();
            this.SetSprite();
        }

        public void SetSprite() {
            Sprite sprite = this.customRender.sprite;
            float minX = sprite.textureRect.min.x / sprite.texture.width;
            float maxX = sprite.textureRect.max.x / sprite.texture.width;
            float width = maxX - minX;
            float minY = sprite.textureRect.min.y / sprite.texture.height;
            float maxY = sprite.textureRect.max.y / sprite.texture.height;
            float height = maxY - minY;
            this.customRender.material.SetFloat("_MinX", minX);
            this.customRender.material.SetFloat("_Width", width);
            this.customRender.material.SetFloat("_MinY", minY);
            this.customRender.material.SetFloat("_Height", height);
        }
    }
}
