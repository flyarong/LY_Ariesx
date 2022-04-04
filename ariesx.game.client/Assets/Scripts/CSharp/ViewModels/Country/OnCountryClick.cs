using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Poukoute {
    public class OnCountryClick : MonoBehaviour, IBeginDragHandler,
        IDragHandler, IEndDragHandler, IPointerClickHandler {
        #region ui element
        public int countryIndex;
        [SerializeField]
        private SpriteRenderer imgStatus;
        [SerializeField]
        private TextMeshPro txtStatus;
        [SerializeField]
        private GameObject pnlCountry;
        [SerializeField]
        private SpriteRenderer imgCountry;
        [SerializeField]
        private SpriteOutline spriteOutline;
        #endregion

        private bool isDragging = false;

        public class CountryChosen : UnityEvent<int> { }
        public CountryChosen OnCountryChosen = new CountryChosen();
        public class CountryDrag : UnityEvent<PointerEventData> { }
        public CountryDrag OnCountryDrag = new CountryDrag();
        public CountryDrag OnCountryBeginDrag = new CountryDrag();

        private void Start() {
            this.txtStatus.text = NPCCityConf.GetMapSNLocalName(countryIndex);
        }

        public void SetContryStatus(CountryStatus status) {
            this.imgStatus.enabled = true;
            this.imgStatus.sprite =
                ArtPrefabConf.GetSprite(
                    string.Concat("country_", status.ToString().ToLower()));
        }

        #region interface action
        public void OnPointerClick(PointerEventData eventData) {
            if (!this.isDragging) {
                this.OnCountryChosen.Invoke(this.countryIndex);
            }
        }

        public void OnDrag(PointerEventData eventData) {
            this.OnCountryDrag.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            this.isDragging = false;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            this.isDragging = true;
            this.OnCountryBeginDrag.Invoke(eventData);
        }
        #endregion

        public void SetCountryIsChosen(bool isChosen) {
            if (this.spriteOutline.enabled != isChosen) {
                this.spriteOutline.enabled = isChosen;
                if (isChosen) {
                    Vector3 tmpPos = this.transform.position;
                    if (this.countryIndex > 3) {
                        tmpPos.y = CountryViewModel.countryCamera.transform.position.y;
                    }
                    CountryViewModel.ResetCountryCameraPos(tmpPos);
                } else {
                    this.imgCountry.color = Color.white;
                }
                GameHelper.SetSortingLayerID(this.gameObject,
                    isChosen ? "Country" : "Default");
            }
        }
    }
}