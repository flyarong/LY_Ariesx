using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class AllianceCreateView : BaseView {
        private AllianceCreateViewModel viewModel;
        private AllianceCreateViewPreference viewPref;

        private bool isAbleJoinAlliance = false;
        private string time = string.Empty;
        private string strCoolDownInfo = string.Empty;
        /*************/

        //void Awake() {
        //    this.viewModel = this.gameObject.GetComponent<AllianceCreateViewModel>();
        //    //this.InitUi();
        //}

        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<AllianceCreateViewModel>();
            this.ui = UIManager.GetUI("UIAllianceCreateOrJoin.PnlCreateOrJoin.PnlContent.PnlCreate");
            this.viewPref = this.ui.transform.GetComponent<AllianceCreateViewPreference>();
            /* Cache the ui components here */
            this.viewPref.ifName.characterLimit = LocalConst.GetLimit(LocalConst.ALLIANCE_NAME);
            this.viewPref.ifName.onValueChanged.AddListener(this.OnIfNameValueChange);
            this.viewPref.txtNamePlaceholder.text =
                string.Concat("0/", LocalConst.GetLimit(LocalConst.ALLIANCE_NAME));

            this.viewPref.ifDescription.characterLimit =
                LocalConst.GetLimit(LocalConst.ALLIANCE_DESCRIPE);
            this.viewPref.ifDescription.onValueChanged.AddListener(this.OnIfDescriptionValueChange);
            this.viewPref.txtDescPlaceholder.text =
                string.Concat("0/", LocalConst.GetLimit(LocalConst.ALLIANCE_DESCRIPE));
            this.viewPref.txtLanguage.text = LocalManager.GetValue(GameConst.ALLIANCE_LANGUAGE, "cn");
            this.viewPref.btnEditLogo.onClick.AddListener(this.OnBtnEditLogoClick);
            this.viewPref.btnCreate.onClick.AddListener(this.OnBtnCreateClick);
            this.viewPref.btnChangeLanguage.onClick.AddListener(this.OnBtnChangeLanguageClick);
            strCoolDownInfo = String.Format(LocalManager.GetValue(LocalHashConst.create_alliance_cooldown),
                "<size=24><color=#FF6769FF>{0}</color></size>");
        }

        private void OnBtnChangeLanguageClick() {
            this.viewModel.AllianceLangChooseViewModel.Show();
        }

        /* Propert change function */
        public void OnLogoChange() {
            this.viewPref.imgLogo.sprite =
                ArtPrefabConf.GetAliEmblemSprite(this.viewModel.AllianceEmblem);
        }

        /***************************/

        public void SetInfo() {
            this.viewPref.influnceChoose.TypeInfo = new TypeChooseContent(
                            this.viewModel.InflunceList, null);
            this.viewPref.typeChoose.TypeValueChangeCallback = this.OnConditionTypeChange;
            this.viewPref.typeChoose.TypeInfo = new TypeChooseContent(
                            this.viewModel.ConditionList, this.viewModel.ConditionLocalList);

            this.isAbleJoinAlliance = (this.viewModel.RejoinAllianceFinishAt
                                        < RoleManager.GetCurrentUtcTime());
            this.viewPref.pnlCooldown.gameObject.SetActiveSafe(!this.isAbleJoinAlliance);

            bool isGoldEnough =
                RoleManager.GetResource(Resource.Gold) >= GameConst.CREATE_ALLIANCE_GOLD_COST;
            this.viewPref.btnCreate.interactable = (isAbleJoinAlliance && isGoldEnough);
            this.viewPref.txtCost.color = isGoldEnough ? new Color(0x80, 0x80, 0x80, 0xFF) : Color.red;
            this.viewPref.ifName.text = string.Empty;
            this.viewPref.ifDescription.text = string.Empty;
        }

        private void UpdateAction() {
            this.isAbleJoinAlliance = (this.viewModel.RejoinAllianceFinishAt <
                        RoleManager.GetCurrentUtcTime());
            if (!this.isAbleJoinAlliance) {
                time = GameHelper.TimeFormat(
                        this.viewModel.RejoinAllianceFinishAt - RoleManager.GetCurrentUtcTime());
                this.viewPref.txtCooldownTime.text = string.Format(strCoolDownInfo, time);
            }
        }

        private void OnConditionTypeChange() {
            JoinConditionType type = (JoinConditionType)Enum.Parse(
                            typeof(JoinConditionType), this.viewPref.typeChoose.TypeIndex.ToString());
            bool isTypeLimit = (type == JoinConditionType.Limit);
            this.viewPref.influnceChoose.imgGrayMask.SetActiveSafe(!isTypeLimit);
            if (type == JoinConditionType.Limit) {
                if (this.viewPref.influnceChoose.TypeIndex == 0) {
                    this.SetBtnDisable(this.viewPref.btnInflunceLeftBtn);
                    this.SetBtnEnable(this.viewPref.btnInflunceRightBtn);
                } else if (this.viewPref.influnceChoose.TypeIndex == this.viewModel.InflunceList.Count - 1) {
                    this.SetBtnDisable(this.viewPref.btnInflunceRightBtn);
                    this.SetBtnEnable(this.viewPref.btnInflunceLeftBtn);
                } else {
                    this.SetBtnEnable(this.viewPref.btnInflunceRightBtn);
                    this.SetBtnEnable(this.viewPref.btnInflunceLeftBtn);
                }
                if (this.viewPref.influnceChoose.TypeInfo.typeLocalList != null) {
                    this.viewPref.txtInflunce.text =
                        this.viewPref.influnceChoose.TypeInfo.typeLocalList[this.viewPref.influnceChoose.TypeIndex];
                } else {
                    this.viewPref.txtInflunce.text =
                        this.viewPref.influnceChoose.TypeInfo.typeList[this.viewPref.influnceChoose.TypeIndex];
                }
            } else {
                this.SetBtnDisable(this.viewPref.btnInflunceLeftBtn);
                this.SetBtnDisable(this.viewPref.btnInflunceRightBtn);
                this.viewPref.txtInflunce.text = "--";
            }
        }

        private void SetBtnDisable(Button button) {
            if (button.interactable) {
                button.interactable = false;
                button.image.material = PoolManager.GetMaterial(MaterialPath.matGray);
            }
        }

        private void SetBtnEnable(Button button) {
            if (!button.interactable) {
                button.interactable = true;
                button.image.material = null;
            }
        }

        private void OnIfNameValueChange(string value) {
            this.viewPref.txtNamePlaceholder.text =
                string.Concat(value.Length, "/", LocalConst.GetLimit(LocalConst.ALLIANCE_NAME));
        }

        private void OnIfDescriptionValueChange(string value) {
            this.viewPref.txtDescPlaceholder.text =
                string.Concat(value.Length, "/", LocalConst.GetLimit(LocalConst.ALLIANCE_DESCRIPE));
        }

        public void SetGameLanguage(AllianceLanguageConf language) {
            this.viewPref.txtLanguage.text = LocalManager.GetValue(GameConst.ALLIANCE_LANGUAGE, language.language);
        }

        private void OnBtnEditLogoClick() {
            this.viewModel.ShowLogoView();
        }

        private void OnBtnCreateClick() {
            this.viewModel.InputAllianceName = this.viewPref.ifName.text;
            this.viewModel.Description = this.viewPref.ifDescription.text;
            this.viewModel.JoinCondition = (JoinConditionType)Enum.Parse(
                    typeof(JoinConditionType), this.viewPref.typeChoose.TypeIndex.ToString());
            this.viewModel.InflunceCondition = int.Parse(
                    this.viewModel.InflunceList[this.viewPref.influnceChoose.TypeIndex]);
            this.viewModel.CreateAlliance();
        }

        protected override void OnVisible() {
            UpdateManager.Regist(UpdateInfo.AllianceCreateView, this.UpdateAction);
        }

        protected override void OnInvisible() {
            UpdateManager.Unregist(UpdateInfo.AllianceCreateView);
        }
    }
}
