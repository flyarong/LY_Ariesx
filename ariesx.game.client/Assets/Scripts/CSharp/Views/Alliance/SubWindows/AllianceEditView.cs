using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class AllianceEditView : BaseView {
        private AllianceEditViewModel viewModel;
        private AllianceEditViewPreference viewPref;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<AllianceEditViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIAllianceWindows.PnlWindows.PnlEdit");
            this.viewPref = this.ui.transform.GetComponent<AllianceEditViewPreference>();
            this.viewPref.btnImageLogo.onClick.AddListener(this.OnBtnEditLogoClick);
            this.viewPref.btnEditLogo.onClick.AddListener(this.OnBtnEditLogoClick);
            this.viewPref.ifDescription.characterLimit =
                LocalConst.GetLimit(LocalConst.ALLIANCE_DESCRIPE);
            this.viewPref.ifDescription.onValueChanged.AddListener(this.OnIfDescriptionValueChange);
            this.viewPref.txtPlaceholder.text =
                string.Concat("0/", LocalConst.GetLimit(LocalConst.ALLIANCE_DESCRIPE));
            this.viewPref.btnOk.onClick.AddListener(this.OnBtnOk);
            this.viewPref.btnCancel.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnChangeLanguage.onClick.AddListener(this.OnBtnChangeLanguageClick);
        }

        private void OnBtnChangeLanguageClick() {
            this.viewModel.AllianceLangChooseViewModel.Show();
        }

        public void SetInfo() {
            this.viewPref.influnceChoose.TypeInfo = new TypeChooseContent(
                this.viewModel.InflunceList, null
            );
            int influnceListCount = this.viewModel.InflunceList.Count;
            for (int index = 0; index < influnceListCount; index++) {
                if (int.Parse(this.viewModel.InflunceList[index]) == this.viewModel.InflunceCondition) {
                    this.viewPref.influnceChoose.TypeIndex = index;
                    break;
                }
            }

            this.viewPref.typeChoose.TypeValueChangeCallback = this.OnConditionTypeChange;
            this.viewPref.typeChoose.TypeInfo = new TypeChooseContent(
                                    this.viewModel.ConditionList, this.viewModel.ConditionLocalList);
            this.viewPref.typeChoose.TypeIndex = (int)this.viewModel.JoinCondition;

            this.viewPref.ifDescription.text = this.viewModel.Alliance.Description;
            this.viewPref.imgLogo.sprite =
                ArtPrefabConf.GetAliEmblemSprite(this.viewModel.Alliance.Emblem);
            this.viewPref.txtAllianceLanguage.text =
               LocalManager.GetValue(GameConst.ALLIANCE_LANGUAGE, AllianceLanguageConf.GetAllianceLanguage(this.viewModel.Alliance.Language.ToString()));
            this.viewModel.Language = this.viewModel.Alliance.Language;
            //this.OnConditionTypeChange();
        }

        public void OnLanguageChange() {
            if (this.IsVisible) {
                this.viewPref.txtAllianceLanguage.text = "";
            }
        }

        /* Propert change function */
        public void OnLogoChange() {
            if (this.IsVisible) {
                this.viewPref.imgLogo.sprite =
                ArtPrefabConf.GetAliEmblemSprite(this.viewModel.AllianceEmblem);
            }
        }

        private void OnConditionTypeChange() {
            JoinConditionType type = (JoinConditionType)Enum.Parse(
                            typeof(JoinConditionType), this.viewPref.typeChoose.TypeIndex.ToString());
            bool isTypeLimit = (type == JoinConditionType.Limit);
            this.viewPref.influnceChoose.imgGrayMask.SetActiveSafe(!isTypeLimit);
            if (isTypeLimit) {
                if (this.viewPref.influnceChoose.TypeIndex == 0) {
                    this.viewPref.btnInflunceLeftBtn.image.material = PoolManager.GetMaterial(MaterialPath.matGray);
                    this.viewPref.imgInflunceLeftArrow.material = PoolManager.GetMaterial(MaterialPath.matGray);
                    this.viewPref.btnInflunceLeftBtn.interactable = false;
                    this.viewPref.btnInflunceRightBtn.interactable = true;
                } else if (this.viewPref.influnceChoose.TypeIndex == this.viewModel.InflunceList.Count - 1) {
                    this.viewPref.btnInflunceLeftBtn.interactable = true;
                    this.viewPref.btnInflunceRightBtn.image.material = PoolManager.GetMaterial(MaterialPath.matGray);
                    this.viewPref.imgInfluncelRightArrow.material = PoolManager.GetMaterial(MaterialPath.matGray);
                    this.viewPref.btnInflunceRightBtn.interactable = false;
                } else {
                    this.viewPref.btnInflunceLeftBtn.image.material = null;
                    this.viewPref.btnInflunceLeftBtn.interactable = true;
                    this.viewPref.btnInflunceRightBtn.image.material = null;
                    this.viewPref.btnInflunceRightBtn.interactable = true;
                }
                if (this.viewPref.influnceChoose.TypeInfo.typeLocalList != null) {
                    this.viewPref.txtInflunce.text = this.viewPref.influnceChoose.TypeInfo.typeLocalList[this.viewPref.influnceChoose.TypeIndex];
                } else {
                    this.viewPref.txtInflunce.text = this.viewPref.influnceChoose.TypeInfo.typeList[this.viewPref.influnceChoose.TypeIndex];
                }
            } else {
                this.viewPref.influnceChoose.txtType.text = "--";
                this.viewPref.btnInflunceLeftBtn.interactable = false;
                this.viewPref.btnInflunceRightBtn.interactable = false;
            }
        }
        /***************************/

        protected void OnBtnCloseClick() {
            this.viewModel.HideWindow();
        }

        private void OnBtnEditLogoClick() {
            this.viewModel.ShowLogoView();
        }

        private void OnBtnOk() {
            this.viewModel.JoinCondition =
                (JoinConditionType)Enum.Parse(typeof(JoinConditionType),
                                            this.viewPref.typeChoose.TypeIndex.ToString());
            this.viewModel.InflunceCondition = int.Parse(
                this.viewModel.InflunceList[this.viewPref.influnceChoose.TypeIndex]);
            this.viewModel.SetAllianceReq();
        }

        private void OnIfDescriptionValueChange(string value) {
            this.viewModel.Description = value;
            this.viewPref.txtPlaceholder.text =
                string.Concat(value.Length, "/", LocalConst.GetLimit(LocalConst.ALLIANCE_DESCRIPE));
        }
        private void OnIfForceChange(string value) {
            this.viewModel.InflunceCondition = int.Parse(value);
        }

        public void SetAllianceLanguage(AllianceLanguageConf language) {
            this.viewPref.txtAllianceLanguage.text = LocalManager.GetValue(
                GameConst.ALLIANCE_LANGUAGE, language.language);
            this.viewModel.Language = int.Parse(language.id);
        }
    }
}
