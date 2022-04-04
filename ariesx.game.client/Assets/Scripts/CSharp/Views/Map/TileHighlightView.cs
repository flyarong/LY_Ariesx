using UnityEngine;
using System.Collections;

namespace Poukoute {
    public class TileHighlightView : MonoBehaviour {
        public Shader CurShader;
        private Material CurMaterial;
        private SpriteRenderer spriteRenderer;
        private Material material {
            get {
                if (CurMaterial == null) {
                    CurMaterial = this.spriteRenderer.material;
                    CurMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return CurMaterial;
            }
        }

        private void Start() {
            UpdateManager.Regist(UpdateInfo.TileHighlightView, this.UpdateAction);
            CurShader = Shader.Find("Sprites/Default");
            this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        private void UpdateAction() {
            if (CurShader != null) {
                Texture sourceTexture = material.GetTexture("_MainTex");
                int renderWidth = sourceTexture.width;
                int renderHeight = sourceTexture.height;

                RenderTexture renderBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight);
                for (int i = 0; i < 2; i++) {
                    //RenderTexture tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0);
                    Graphics.Blit(sourceTexture, renderBuffer, material, 1);
                    RenderTexture tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0);
                    Graphics.Blit(renderBuffer, tempBuffer, CurMaterial, 2);
                    RenderTexture.ReleaseTemporary(renderBuffer);
                    renderBuffer = tempBuffer;
                }
                // Sprite.Create(, )
                Graphics.Blit(renderBuffer, this.CurMaterial);
                RenderTexture.ReleaseTemporary(renderBuffer);
            }
        }
    }
}