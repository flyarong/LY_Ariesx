using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class TileOutline : MonoBehaviour {
        public Texture texture;
        public RenderTexture renderTexture;
        public Material material;

        //private MeshRenderer meshRenderer;

        void Awake() {
            //this.meshRenderer = this.GetComponent<MeshRenderer>();
            UpdateManager.Regist(UpdateInfo.TileOutline, this.UpdateAction);
        }
        
        private void UpdateAction() {
            Graphics.Blit(this.texture, this.renderTexture, this.material);
        }
    }
}
