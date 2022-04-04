using System;
using Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace Poukoute {
    public class CountryView : BaseView, IBeginDragHandler,
        IDragHandler, IEndDragHandler, IPinchHandler, IBeginPinchHandler, IEndPinchHandler {
        private CountryViewModel viewModel;
        private CountryViewPreference viewPref;


        private int countryIndex = -1;
        private bool isShowCountryInfo = false;
        private Vector2 oldDistance = Vector2.zero;
        private Vector2 originDistance = Vector2.zero;

        private float lerp = 1.5f;
        private bool isUpdateCameraFieldOfView = false;
        private float tmpFieldOfView;
        /*********************************************************/
        private void Awake() {
            this.viewModel = this.transform.GetComponent<CountryViewModel>();
            this.viewPref = this.transform.GetComponent<CountryViewPreference>();

            this.viewPref.btnChoose.onClick.AddListener(this.OnBtnChooseClick);
        }

        private void Start() {
            int countriesCount = this.viewPref.countryClick.Length;
            for (int i = 0; i < countriesCount; i++) {
                this.viewPref.countryClick[i].OnCountryChosen.AddListener(this.ShowCountryInfo);
                this.viewPref.countryClick[i].OnCountryDrag.AddListener(this.OnDrag);
                this.viewPref.countryClick[i].OnCountryBeginDrag.AddListener(this.OnBeginDrag);
            }

            int tmpLeftCount = -1;
            foreach (GetBornPointsAck.Status bornStatus in this.viewModel.BornPoints) {
                this.viewPref.countryClick[bornStatus.MapSN - 1].SetContryStatus(
                    this.GetBorunPointStatus(bornStatus.LeftCount));
                if (tmpLeftCount < bornStatus.LeftCount) {
                    tmpLeftCount = bornStatus.LeftCount;
                    this.countryIndex = bornStatus.MapSN;
                }
            }
            this.ShowComic();
            AudioManager.Stop(AudioType.Show);
            AudioManager.Play("bg_chose_comic", AudioType.Background, AudioVolumn.High, loop: false);
        }

        public void HideCountryInfo() {
            if (this.isShowCountryInfo) {
                AnimationManager.Animate(this.viewPref.pnlCountryInfo, "Hide");
                this.isShowCountryInfo = false;
                this.ResetAllCountryChosenStatus();
            }
        }

        private void ShowComic() {
            this.viewPref.pnlComic.SetActive(true);
            int count = 0;
            List<float> delayList = new List<float> { 0, 1f, 2.5f };
            foreach (Transform child in this.viewPref.pnlComicText.transform) {
                int index = ++count;
                child.gameObject.SetActive(true);
                AnimationManager.Animate(child.gameObject, "Show", () => {
                    if (index == 3) {
                        AnimationManager.Animate(this.viewPref.pnlComicText, "Hide", () => {
                            this.viewPref.pnlComic.SetActive(false);
                            AudioManager.Play("bg_chose_state", AudioType.Background, AudioVolumn.High, loop: true);
                            UpdateManager.Regist(UpdateInfo.CountryChoose, this.UpdateCameraFieldOfView);
                        });
                    }
                }, delay: delayList[index - 1], isOffset: false);
            }
        }

        private float cameraFieldOfView = 0.0f;
        private void UpdateCameraFieldOfView() {
            this.cameraFieldOfView = this.viewPref.countryCamera.fieldOfView;
            if (GameHelper.NearlyEqual(
                cameraFieldOfView, 55f, 0.01f)) {
                this.isUpdateCameraFieldOfView = false;
                UpdateManager.Unregist(UpdateInfo.CountryChoose, this.UpdateCameraFieldOfView);
                this.OnCountrySelected(this.countryIndex);
            } else {
                this.isUpdateCameraFieldOfView = true;
                this.viewPref.countryCamera.fieldOfView =
                    Mathf.Lerp(cameraFieldOfView, 55f, this.lerp * Time.unscaledDeltaTime);
            }
        }

        private CountryStatus GetBorunPointStatus(int leftCount) {
            if (leftCount > 600) {
                return CountryStatus.Smooth;
            } else if (leftCount < 601 && leftCount > 198) {
                return CountryStatus.Average;
            } else if (leftCount < 199 && leftCount > 0) {
                return CountryStatus.Crowded;
            } else {
                return CountryStatus.Full;
            }
        }

        private void ShowCountryInfo(int countryIndex) {
            if (!this.viewModel.isLoginWorld &&
                countryIndex != this.countryIndex &&
                !this.isUpdateCameraFieldOfView) {
                this.HideCountryInfo();
                this.countryIndex = countryIndex;
                this.OnCountrySelected(countryIndex);
            }
        }

        private void OnCountrySelected(int countryIndex) {
            this.viewPref.txtCountryDesc.text =
                NPCCityConf.GetMapSNLocalDesc(countryIndex);
            this.isShowCountryInfo = true;
            AnimationManager.Animate(this.viewPref.pnlCountryInfo, "Show");
            this.viewPref.countryClick[countryIndex - 1].SetCountryIsChosen(true);
            this.viewPref.btnChoose.gameObject.SetActiveSafe(countryIndex < 9);
        }

        private void ResetAllCountryChosenStatus() {
            int countriesCount = this.viewPref.countryClick.Length;
            for (int i = 0; i < countriesCount; i++) {
                this.viewPref.countryClick[this.countryIndex - 1].SetCountryIsChosen(false);
            }
            this.countryIndex = -1;
        }

        public Camera GetCountryCamera() {
            return this.viewPref.countryCamera;
        }

        #region interface 
        public void OnBeginDrag(PointerEventData eventData) {
            if (this.isUpdateCameraFieldOfView) {
                return;
            }
            this.HideCountryInfo();
        }

        public void OnDrag(PointerEventData eventData) {
            if (this.isUpdateCameraFieldOfView) {
                return;
            }
            Vector3 countriesPos = this.viewPref.countryCamera.transform.position;
            countriesPos -= (Vector3)(eventData.delta / 150);
            this.viewPref.countryCamera.transform.position =
                this.viewModel.GetContryCameraFinalPos(countriesPos);
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (this.isUpdateCameraFieldOfView) {
                return;
            }
        }

        public void OnPinch(PinchEventData eventData) {
            if (this.isUpdateCameraFieldOfView) {
                return;
            }
#if UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
            float filedPercent = 1 - eventData.scrollAxis * 0.1f;
#else
            Vector2 pointA = eventData.data[0].position;
            Vector2 pointB = eventData.data[1].position;
            Vector2 currentDistance = pointA - pointB;
            if (Vector2.Angle(currentDistance, originDistance) > 100) {
                return;
            }
            float filedPercent = 
                    (1 - Mathf.Sqrt(this.oldDistance.sqrMagnitude / currentDistance.sqrMagnitude));
            this.oldDistance = currentDistance;
#endif
            tmpFieldOfView = this.viewPref.countryCamera.fieldOfView * filedPercent;
            this.viewPref.countryCamera.fieldOfView =
                Mathf.Clamp(tmpFieldOfView, 40f, 60f);

        }

        public void OnBeginPinch(PinchEventData eventData) {
            if (this.isUpdateCameraFieldOfView) {
                return;
            }
#if UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
            //Debug.LogError(this.viewPref.countryCamera.fieldOfView + " " + data.scrollAxis);
            Vector2 pointA = eventData.data[0].position;
            Vector2 pointB = eventData.data[1].position;
            this.oldDistance = pointA - pointB;
            this.originDistance = this.oldDistance;
#endif
        }

        public void OnEndPinch(PinchEventData eventData) {
            //Debug.LogError(this.viewPref.countryCamera.fieldOfView + " " + data.scrollAxis);
        }
        #endregion


        private void OnBtnChooseClick() {
            this.viewModel.OnCountryChosen(this.countryIndex);
        }
    }
}
