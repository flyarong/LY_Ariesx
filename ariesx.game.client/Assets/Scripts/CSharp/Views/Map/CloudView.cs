using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Protocol;

namespace Poukoute {
    public class CloudView : BaseView {
        private SpriteRenderer spriteRenderer;
        private Vector3 offset;
        private float speed;
        public int areaIndex;
        public int areaRange = 9;
        public int horinzonOffset = 15;
        void Awake() {
            this.spriteRenderer = this.GetComponent<SpriteRenderer>();
            UpdateManager.Regist(UpdateInfo.CloudView, this.UpdateAction);
        }

        private void UpdateAction() {
            this.transform.position += Vector3.left * speed * Time.unscaledDeltaTime;
        }

        private IEnumerator Check() {
            //yield return new WaitForSeconds(1f);
            yield return YieldManager.GetWaitForSeconds(1f);
            while ((this.transform.position.x - GameManager.MainCamera.transform.position.x).Abs() < this.horinzonOffset * 1.1f) {
                //yield return new WaitForSeconds(1f);
                yield return YieldManager.GetWaitForSeconds(1f);
            }
            MapElementManager.Instance.RemoveElement(this.areaIndex);
        }

        void OnEnable() {
            int randomRange = this.areaIndex * this.areaRange;
            this.offset = new Vector3(
                horinzonOffset,
                Random.Range(-7 + randomRange, -4 + randomRange),
                1 + 0.1f * this.areaIndex
            );
            this.speed = Random.Range(0.2f, 0.7f);
            int index = Random.Range(1, 4);
            this.spriteRenderer.sprite =
                ArtPrefabConf.GetSprite(
                    string.Concat(SpritePath.tileLayerAbovePrefix, "cloud", index));
            this.transform.position = GameManager.MainCamera.transform.position + offset;

            StartCoroutine(this.Check());
        }
    }
}
