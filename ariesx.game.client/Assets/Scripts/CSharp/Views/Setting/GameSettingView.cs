using Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {

    public class GameSettingView : BaseView {
        private GameSettingViewModel viewModel;
        private GameSettingViewPreference viewPref;

        private static string StatusOn = "setting_btn_status_on";
        private static string StatusOff = "setting_btn_status_off";

        //private void Awake() {
        //    this.viewModel = this.transform.GetComponent<GameSettingViewModel>();
        //}

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIGameSetting");
            this.viewModel = this.transform.GetComponent<GameSettingViewModel>();
            this.viewPref = this.ui.transform.GetComponent<GameSettingViewPreference>();

            this.InitButtonsClickCallback();
        }

        private void InitButtonsClickCallback() {
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClicked);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClicked);

            this.viewPref.btnMusic.onClick.AddListener(this.OnBtnMusicCliked);
            this.viewPref.btnSounds.onClick.AddListener(this.OnBtnSoundClicked);
            this.viewPref.btnLanguage.onClick.AddListener(this.OnBtnLanguageClicked);
            this.viewPref.btnPush.onClick.AddListener(this.OnBtnPushClicked);
            this.viewPref.btnFitterTribeChat.onClick.AddListener(this.OnBtnFitterTribeChatClicked);
            this.viewPref.btnGooglePlay.onClick.AddListener(this.OnBtnGooglePlayClicked);
            this.viewPref.btnFacebook.onClick.AddListener(this.OnBtnFacebookClicked);
            this.viewPref.btnDragonnest.onClick.AddListener(this.OnBtnDragonnestClicked);
            this.viewPref.btnGameMaker.onClick.AddListener(this.OnBtnGameMakerCliked);
            this.viewPref.btnHelpSurpport.onClick.AddListener(this.OnBtnHelpSurpportClicked);
            this.viewPref.btnPrivacy.onClick.AddListener(this.OnBtnPrivacyCliked);
            this.viewPref.btnTermsService.onClick.AddListener(this.OnBtnTermServiceClicked);
            this.viewPref.btnParentGuid.onClick.AddListener(this.OnBtnParentGuidClicked);
        }


        #region Public interface
        public void UpdateMusicStatus(bool isOn) {
            this.UpdateBtnSwitchStatus(
                isOn, this.viewPref.txtMusicStatus, 
                this.viewPref.imgMusicFill);
        }

        public void UpdateSoundsStatus(bool isOn) {
            this.UpdateBtnSwitchStatus(
                isOn, this.viewPref.txtSoundsStatus, 
                this.viewPref.imgSoundsFill);
        }

        public void UpdatePushStatus(bool isOn) {
            this.UpdateBtnSwitchStatus(
                isOn, this.viewPref.txtPushStatus, 
                this.viewPref.imgPushFill);
        }

        public void UpdateFitterStatus(bool isOn) {
            this.UpdateBtnSwitchStatus(
                isOn, this.viewPref.txtFitterStatus, 
                this.viewPref.imgFitterFill);
        }

        public void UpdateGooglePlayStatus(bool isConnect) {
            this.UpdateBtnConnectedStatus(
                isConnect, this.viewPref.txtGooglePlayStatus, 
                this.viewPref.imgGoogleplayFill);
        }

        public void UpdateFacebookStatus(bool isConnect) {
            this.UpdateBtnConnectedStatus(
                isConnect, this.viewPref.txtFacebookStatus, 
                this.viewPref.imgFaceboolFill);
        }

        public void UpdateDragonnestStatus(bool isConnect) {
            this.UpdateBtnConnectedStatus(
                isConnect, this.viewPref.txtDragonnestStatus, 
                this.viewPref.imgDragonnestFill);
        }

        public void UpdateLanguageStatus() {

        }
        #endregion


        private void UpdateBtnSwitchStatus(bool isOn, TextMeshProUGUI text, Image image) {
            text.text = isOn ?
                LocalManager.GetValue(LocalHashConst.off) :
                LocalManager.GetValue(LocalHashConst.on);
            image.sprite = ArtPrefabConf.GetSprite(isOn ? StatusOff : StatusOn);
        }

        private void UpdateBtnConnectedStatus(bool isConnect, TextMeshProUGUI text, Image image) {
            text.text = isConnect ?
                LocalManager.GetValue(LocalHashConst.connect) :
                LocalManager.GetValue(LocalHashConst.disconnect);
            image.material = isConnect ? null : PoolManager.GetMaterial(MaterialPath.matGray);
        }


        #region Buttons call back
        private void OnBtnCloseClicked() {

        }

        private void OnBtnMusicCliked() {
            this.viewModel.SetGameMusicMute();
        }

        private void OnBtnSoundClicked() {

        }

        private void OnBtnLanguageClicked() {

        }

        private void OnBtnPushClicked() {

        }

        private void OnBtnFitterTribeChatClicked() {

        }

        private void OnBtnGooglePlayClicked() {

        }

        private void OnBtnFacebookClicked() {

        }

        private void OnBtnDragonnestClicked() {

        }

        private void OnBtnGameMakerCliked() {

        }

        private void OnBtnHelpSurpportClicked() {

        }

        private void OnBtnPrivacyCliked() {

        }

        private void OnBtnTermServiceClicked() {

        }

        private void OnBtnParentGuidClicked() {

        }
        #endregion
    }
}
