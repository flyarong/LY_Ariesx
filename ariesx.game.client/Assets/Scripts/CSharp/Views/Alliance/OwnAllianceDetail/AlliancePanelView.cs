using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class AlliancePanelView : BaseView {
        private AlliancePanelViewModel viewModel;
        private AlliancePanelViewPreference viewPref;
        private bool isAllianceOwner = false;

        /***********************************/

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIAlliance.PnlAlliance.PnlAllianceDetail");
            this.viewModel = this.gameObject.GetComponent<AlliancePanelViewModel>();
            this.viewPref = this.ui.transform.GetComponent<AlliancePanelViewPreference>();
            this.SetAllianceUI();
        }

        private void SetAllianceUI() {
            this.viewPref.btnMembersList.onClick.AddListener(this.OnBtnMembersListClick);
            this.viewPref.alyExperienceSli.onValueChanged.AddListener(this.OnSliderValueChange);
            this.viewPref.btnSetting.onClick.AddListener(this.OnBtnSettingClick);
            this.viewPref.btnMessage.onClick.AddListener(this.OnBtnMessageClick);
            this.viewPref.btnInfo.onClick.AddListener(this.OnBtnInfoClick);
            this.viewPref.btnLeaveOrDissolve.onClick.AddListener(this.OnBtnDissolveClick);
        }

        public void SetAllianceHeadContent(Alliance alliance, AllianceRole role) {
            this.SetButtonsInfos(role);
            this.SetAllianceTipInfo(alliance);
        }

        private void SetAllianceTipInfo(Alliance alliance) {
            int allianceLevel = AllianceLevelConf.GetAllianceLevelByExp(alliance.Exp);
            AllianceLevelConf allianceConf = AllianceLevelConf.GetConf(allianceLevel.ToString());
            this.viewPref.txtName.text = alliance.Name;
            this.viewPref.txtLevel.text = GameHelper.GetLevelLocal(allianceLevel);

            bool isReachMaxAlyLevel = allianceLevel >= AllianceLevelConf.GetAllianceMaxLevel();
            this.viewPref.pnlAlyExperience.gameObject.SetActiveSafe(!isReachMaxAlyLevel);
            if (!isReachMaxAlyLevel) {
                int alyUpgradeExp = AllianceLevelConf.GetAllianceUpgradeExpByLevel(allianceLevel);
                this.viewPref.alyExperienceSli.maxValue = alyUpgradeExp;
                this.viewPref.alyExperienceSli.value = alliance.Exp + 1;
            } else {
                this.viewPref.alyExperienceSli.maxValue = 1;
                this.viewPref.alyExperienceSli.value = 1;
            }
            this.viewPref.txtDesc.text = alliance.Description.CustomIsEmpty() ?
                LocalManager.GetValue(LocalHashConst.alliance_default_desc) : alliance.Description;
            this.viewPref.txtInflunce.text = alliance.Exp.ToString();
            this.viewPref.txtMembers.text = alliance.MemberCount + "/" + allianceConf.maxMember;
            this.viewPref.txtLanguage.text = LocalManager.GetValue(
                GameConst.ALLIANCE_LANGUAGE, AllianceLanguageConf.GetAllianceLanguage(alliance.Language.ToString()));
            this.viewPref.imgLogo.sprite =
                ArtPrefabConf.GetAliEmblemSprite(alliance.Emblem);

            this.SetAllianceResourBonus(allianceConf);
        }

        private void SetAllianceResourBonus(AllianceLevelConf allianceConf) {
            this.viewPref.txtLumber.text = string.Concat(
                "+", (allianceConf.lumberbuff * 100), "%");
            this.viewPref.txtSteel.text = string.Concat(
                "+", (allianceConf.steelbuff * 100), "%");
            this.viewPref.txtFood.text = string.Concat(
                "+", (allianceConf.foodbuff * 100), "%");
            this.viewPref.txtMarble.text = string.Concat(
                "+", (allianceConf.marblebuff * 100), "%");
        }

        private void SetButtonsInfos(AllianceRole role) {
            this.isAllianceOwner = (role == AllianceRole.Owner);
            this.viewPref.txtLeaveOrDissolve.text =
                (this.isAllianceOwner) ?
                LocalManager.GetValue(LocalHashConst.button_alliance_dissolve) :
                LocalManager.GetValue(LocalHashConst.button_alliance_leave);

            bool canSendAllyMsg = this.isAllianceOwner || (role == AllianceRole.Leader);
            this.viewPref.btnMessage.gameObject.SetActiveSafe(canSendAllyMsg);
            this.viewPref.btnSetting.gameObject.SetActiveSafe(canSendAllyMsg);
        }

        private void SetBtnDissolveVisible() {
            string ownAllianceId = RoleManager.GetAllianceId();
            bool isOwnAlliance = ownAllianceId.CustomEquals(this.viewModel.AllianceId);
            bool canDissolveAlliance = isOwnAlliance && (this.viewModel.SelfPlayerPublickInfo != null) &&
                (this.viewModel.SelfPlayerPublickInfo.AllianceRole == (int)AllianceRole.Owner) /*&&
                (this.viewModel.ItemViewCount < 2)*/; // To do: Add members count adjust.
            this.SetBtnDissolveVisible(canDissolveAlliance);
        }

        private void SetBtnDissolveVisible(bool visible) {
            this.viewPref.btnLeaveOrDissolve.gameObject.SetActiveSafe(visible);
        }

        /*************** btn callback ***************************/
        private void OnBtnDissolveClick() {
            string content = RoleManager.IsUnderProtection() ?
                        string.Format(LocalManager.GetValue(LocalHashConst.alliance_quit_warning, 
                                        string.Concat(30, LocalManager.GetValue(LocalHashConst.time_minute)))) :
                        string.Format(LocalManager.GetValue(LocalHashConst.alliance_quit_warning, 
                                         string.Concat(24, LocalManager.GetValue(LocalHashConst.time_hour))));
            if (this.isAllianceOwner) {
                UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.button_alliance_dissolve),
                     LocalManager.GetValue(LocalHashConst.alliance_dissolve_tip),
                   this.viewModel.DissolveAllianceReq, () => { },
                 tips: content, txtTipsAlignment: TextAlignmentOptions.Left
             );
            } else {
                UIManager.ShowConfirm(
                    LocalManager.GetValue(LocalHashConst.button_alliance_leave),
                    LocalManager.GetValue(LocalHashConst.alliance_leave_tip),
                    this.viewModel.QuitAllianceReq, () => { },
                    tips: content, txtTipsAlignment: TextAlignmentOptions.Left
                );
            }
        }

        private void OnBtnSettingClick() {
            this.viewModel.ShowEditAllianceInfoPanel();
        }

        private void OnBtnMessageClick() {
            this.viewModel.ShowAllianceChatroom();
        }

        private void OnBtnMembersListClick() {
            this.viewModel.ShowMembersList();
        }

        private void OnBtnInfoClick() {
            this.viewModel.ShowAllianceDisplayBoard(DisplayType.AllianceDisplayBoard);
        }

        private void OnSliderValueChange(float value) {
            this.viewPref.txtAlyExperience.text =
                string.Concat(value - 1, "/", this.viewPref.alyExperienceSli.maxValue);
        }
    }
}
