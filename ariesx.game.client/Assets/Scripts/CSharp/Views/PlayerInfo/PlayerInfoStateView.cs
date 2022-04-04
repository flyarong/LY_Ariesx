using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

namespace Poukoute {
    public class PlayerInfoStateView : BaseView {
        private PlayerInfoStateViewPreference viewPref;
        private PlayerInfoStateViewModel viewModel;
        private const int PRODUCTION_TIMES = 10;
        private float detailOffset = 0;
        private Transform tempTransform = null;


        private void Awake() {
            viewModel = this.GetComponent<PlayerInfoStateViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIPlayerInfo.PnlPlayerInfo.PnlState");
            this.viewPref = this.ui.transform.GetComponent<PlayerInfoStateViewPreference>();

            this.viewPref.btnResInfo.onClick.AddListener(this.OnBtnResInfoClick);
            this.viewPref.btnRoleAvatar.onClick.AddListener(this.OnBtnRoleAvatarClick);
            this.viewPref.pnlList.onClick.AddListener(this.onListClick);
            this.viewPref.btnBasicInfoDetail.onClick.AddListener(this.OnBasicInfoDetailClick);
            this.viewPref.btnResourcesDetail.onClick.AddListener(this.OnResourcesDetailClick);
            this.viewPref.btnTributeDetail.onClick.AddListener(this.OnTributeDetailClick);

            this.viewPref.btnEditName.onClick.AddListener(this.OnEditNameClick);
            this.viewPref.btnChangeName.onClick.AddListener(this.OnChangeNameClick);
            this.viewPref.ifName.characterLimit = LocalConst.GetLimit(LocalConst.PLAYER_NAME);
            this.viewPref.ifName.onEndEdit.AddListener(this.OnEditNameEnd);
            this.viewPref.ifName.onValueChanged.AddListener(this.OnIfNameValueChanged);
            this.viewPref.ifDescription.characterLimit =
               LocalConst.GetLimit(LocalConst.PLAYER_DESCRIPE);
            //this.viewPref.ifDescription.onEndEdit.AddListener(this.OnDescriptionEndEdit);
            this.viewPref.btnEditDesc.onClick.AddListener(this.OnBtnEditClick);
            this.viewPref.btnCancelChangeDesc.onClick.AddListener(this.OnBtnCancelEditClick);

            this.viewPref.pnlResourcesDetail.gameObject.SetActiveSafe(false);
            this.viewPref.pnlTributeDetail.gameObject.SetActiveSafe(false);
            this.viewPref.pnlBasicInfoDetail.gameObject.SetActiveSafe(false);

        }

        public void SetPlayerInfo() {
            this.SetHead();
            this.SetPowerInfo();
            this.SetResourcesInfo();
            this.SetTribute();
            this.BasicInfo();
            this.SetDesc();
        }

        private void ShowDetail(Transform witch) {
            witch.gameObject.SetActiveSafe(true);
            Vector2 position = witch.
                GetComponent<RectTransform>().anchoredPosition;
            AnimationManager.Animate(witch.gameObject, "Show",
                position + Vector2.up * this.detailOffset, position, null);
            tempTransform = witch;
        }

        public void ShowDetailBtn(Transform whitch) {
            if (tempTransform != null) {
                Vector2 position = this.tempTransform.GetComponent<RectTransform>()
                    .anchoredPosition;
                AnimationManager.Animate(this.tempTransform.gameObject,
                    "Hide", position,
                    position + Vector2.up * this.detailOffset, () => {
                        this.tempTransform.gameObject.SetActiveSafe(false);
                        tempTransform = null;
                        this.ShowDetail(whitch);
                    });
            } else {
                this.ShowDetail(whitch);
            }
        }

        public void HideDetail() {
            if (tempTransform != null) {
                Vector2 position = this.tempTransform.GetComponent<RectTransform>()
                    .anchoredPosition;
                AnimationManager.Animate(this.tempTransform.gameObject,
                    "Hide", position,
                    position + Vector2.up * this.detailOffset, () => {
                        this.tempTransform.gameObject.SetActiveSafe(false);
                        tempTransform = null;

                    });
            }
        }

        void SetHead() {
            this.viewPref.imgAvatar.sprite = RoleManager.GetRoleAvatar();
            this.viewPref.txtName.text = RoleManager.GetRoleName();
            this.SetPlayerAllianceInfo();//player alliance
            this.SetFallenInfo(RoleManager.GetMasterAllianceName());//player is fallan
            this.viewPref.btnChangeName.gameObject.SetActiveSafe(false);
            this.viewPref.btnEditName.gameObject.SetActive(RoleManager.CanEditUserName());
            this.SetShortId();
            this.viewPref.txtWorldName.text = 
                LocalManager.GetValue(LocalHashConst.house_keeper_state_server) + 
                this.viewModel.GetWorldInfo();
        }

        public void SetShortId() {
            if (!RoleManager.ShortId.CustomIsEmpty()) {
                this.viewPref.txtPlayerId.gameObject.SetActiveSafe(true);
                this.viewPref.txtPlayerId.text =
                    LocalManager.GetValue(
                        LocalHashConst.house_keeper_state_playerid) 
                        + " " + RoleManager.ShortId;
            }
        }

        public void ShowHint(string hintDes) {
            this.viewPref.txtHint.text = hintDes;
            AnimationManager.Animate(this.viewPref.pnlHint.gameObject, "Show");
        }

        public void ChangeAvatar(string hintDes) {
            this.viewPref.imgAvatar.sprite = RoleManager.GetRoleAvatar();
            ShowHint(hintDes);
        }

        public void SetPlayerAllianceInfo() {
            Alliance alliance = RoleManager.GetAlliance();
            bool inAlliance = (alliance != null) && !alliance.Id.CustomIsEmpty();
            if (inAlliance) {
                this.viewModel.GetMyAlliance();
            } else {
                this.viewPref.txtAlliance.text = LocalManager.GetValue(LocalHashConst.not_in_alliance);
                //string role = RoleManager.GetAllianceRole().ToString().ToLower();
                this.viewPref.txtRole.text = LocalManager.GetValue(LocalHashConst.not_in_alliance);
                this.viewPref.imgAllianceIcon.gameObject.SetActiveSafe(false);
            }
        }

        public void SetPlayerAllianceDetail() {
            Alliance alliance = RoleManager.GetAlliance();
            this.viewPref.txtAlliance.text = alliance.Name;
            string role = RoleManager.GetAllianceRole().ToString().ToLower();
            this.viewPref.txtRole.text = LocalManager.GetValue("alliance_position_", role);
            this.viewPref.imgAllianceIcon.gameObject.SetActiveSafe(true);
            int logoId = alliance.Emblem;
            this.viewPref.imgAllianceIcon.sprite = ArtPrefabConf.GetAliEmblemSprite(logoId);
        }

        private void SetFallenInfo(string befallingAllianeName) {
            bool isFalling = !string.IsNullOrEmpty(befallingAllianeName);
            this.viewPref.pnlFallen.gameObject.SetActiveSafe(isFalling);
            this.viewPref.txtFallenAlliance.gameObject.SetActiveSafe(isFalling);

            if (isFalling) {
                this.viewPref.txtFallenAlliance.text =
                    string.Format(LocalManager.GetValue(LocalHashConst.fallen_title), befallingAllianeName);
            }
        }

        private void SetPowerInfo() {
            int force = RoleManager.GetForce();
            int forceBackground = ForceRewardConf.GetCurrentForceLevelForce();
            //int forceLevel =
            //    ForceRewardConf.GetForceLevel(force);
            this.viewPref.txtPowerValueValue.text =
                string.Concat(GameHelper.GetFormatNum(force, maxLength: 3, decLength: 2), "/",
                    GameHelper.GetFormatNum(forceBackground, maxLength: 3, decLength: 2));
            Dictionary<Vector2, Point> pointDict = RoleManager.GetPointDict();
            this.viewPref.txtTileCountValue.text =
                string.Concat(pointDict.Count, "/", RoleManager.GetPointsLimit());

            int maxLevel = 0;
            foreach (var item in pointDict) {
                int level = this.viewModel.GetTileLevel(item.Value.Coord);
                if (level > maxLevel) {
                    maxLevel = level;
                }
            }
            this.viewPref.txtTileLimitValue.text = GameHelper.GetLevelLocal(maxLevel);
        }

        private readonly string productionKey = "{0}/{1}";
        private readonly string tileHourLocal = "--";
        private void SetResourcesInfo() {
            if (this.viewModel.AllResourcesProduction == null) {
                this.ResetProductionInfo();
                return;
            }
            string tileHourLocal = LocalManager.GetValue(LocalHashConst.time_hour);
            this.viewPref.txtLumberValue.text = string.Format(this.productionKey,
                GameHelper.GetFormatNum(this.viewModel.AllResourcesProduction.Lumber
                * PRODUCTION_TIMES), tileHourLocal);
            this.viewPref.txtSteelValue.text = string.Format(this.productionKey,
                GameHelper.GetFormatNum(this.viewModel.AllResourcesProduction.Steel
                * PRODUCTION_TIMES), tileHourLocal);
            this.viewPref.txtMarbleValue.text = string.Format(this.productionKey,
                GameHelper.GetFormatNum(this.viewModel.AllResourcesProduction.Marble
                * PRODUCTION_TIMES), tileHourLocal);
            this.viewPref.txtFoodValue.text = string.Format(this.productionKey,
                GameHelper.GetFormatNum(this.viewModel.AllResourcesProduction.Food
                * PRODUCTION_TIMES), tileHourLocal);
        }

        private void ResetProductionInfo() {
            this.viewPref.txtLumberValue.text = tileHourLocal;
            this.viewPref.txtSteelValue.text = tileHourLocal;
            this.viewPref.txtMarbleValue.text = tileHourLocal;
            this.viewPref.txtFoodValue.text = tileHourLocal;
        }

        private void SetTribute() {
            this.viewPref.txtTribute.text = string.Concat(string.Format("+{0}/",
                GameHelper.GetFormatNum(this.viewModel.tributeGole
                )), LocalManager.GetValue("tribute"));
        }

        private void BasicInfo() {
            this.viewPref.txtHeroCountValue.text
                = String.Concat(RoleManager.GetOwnedHeroAmount(), "/",
                                         GameConst.HERO_COUNT);
            this.viewPref.txtArmyCountValue.text
                = String.Concat(RoleManager.GetTroopNum(), "/",
                                         RoleManager.GetAllTroopNum());
            this.viewPref.txtStrongholdValue.text
                = String.Concat(RoleManager.GetBuildedStrongholdNumber(), "/",
                                         RoleManager.GetAllBuildableStrongholdNum() +
                                         RoleManager.GetBuildedStrongholdNumber());
        }

        public void SetDesc() {
            if (string.IsNullOrEmpty(RoleManager.GetRoleDesc())) {
                this.viewPref.txtDescription.text = LocalManager.GetValue(LocalHashConst.player_desc);
            } else {
                this.viewPref.txtDescription.text = RoleManager.GetRoleDesc();
            }
        }
        public void SetRoleDesc() {
            this.viewPref.txtDescription.text = this.viewModel.Description;
        }

        private void OnBtnEditClick() {
            this.viewPref.ifDescription.gameObject.SetActiveSafe(
                !this.viewPref.ifDescription.gameObject.activeSelf);
            this.viewPref.txtDescription.gameObject.SetActiveSafe(
                !this.viewPref.txtDescription.gameObject.activeSelf);
            this.viewPref.btnCancelChangeDesc.gameObject.SetActiveSafe(
                !this.viewPref.btnCancelChangeDesc.gameObject.activeSelf);
            if (this.viewPref.ifDescription.gameObject.activeSelf) {
                this.viewPref.ifDescription.text = string.Empty;
                this.viewPref.ifDescription.ActivateInputField();
                this.viewPref.imgEditDesc.sprite = ArtPrefabConf.GetSprite("edit_ok");

            } else {
                this.viewPref.ifDescription.DeactivateInputField();
                this.viewPref.imgEditDesc.sprite = ArtPrefabConf.GetSprite("edit_desc");
                OnDescriptionEndEdit(this.viewPref.ifDescription.text);
            }
        }

        //bool isCancelEdit = false;
        private void OnBtnCancelEditClick() {
            //isCancelEdit = true;
            this.viewPref.ifDescription.gameObject.SetActiveSafe(
                !this.viewPref.ifDescription.gameObject.activeSelf);
            this.viewPref.txtDescription.gameObject.SetActiveSafe(
                !this.viewPref.txtDescription.gameObject.activeSelf);
            this.viewPref.btnCancelChangeDesc.gameObject.SetActiveSafe(
                !this.viewPref.btnCancelChangeDesc.gameObject.activeSelf);
            this.viewPref.imgEditDesc.sprite = ArtPrefabConf.GetSprite("edit_desc");
        }

        private void OnDescriptionEndEdit(string description) {
            this.HideDetail();
            if (!string.IsNullOrEmpty(description) &&
                !description.CustomEquals(this.viewPref.txtDescription.text)) {
                this.viewModel.Description = description;
            }
        }

        public void OnEditNameDone(string newName) {
            RoleManager.SetRoleName(newName);
            this.viewPref.txtName.gameObject.SetActive(true);
            this.viewPref.txtName.text = newName;
            this.viewPref.btnEditName.gameObject.SetActive(false);
            this.viewPref.btnChangeName.gameObject.SetActiveSafe(false);
            this.viewPref.ifName.gameObject.SetActive(false);
        }

        private void OnEditNameClick() {
            this.viewPref.ifName.gameObject.SetActive(
                !this.viewPref.ifName.gameObject.activeSelf);

            this.viewPref.txtName.gameObject.SetActive(
                !this.viewPref.txtName.gameObject.activeSelf);

            if (this.viewPref.ifName.gameObject.activeSelf) {
                this.viewPref.ifName.text = this.viewPref.txtName.text;
                this.viewPref.ifName.ActivateInputField();
            } else {
                this.viewPref.ifName.DeactivateInputField();
            }
        }
        private void OnIfNameValueChanged(string name) {
            if (name.CustomStartsWith(" ")) {
                name = name.TrimStart(' ');
                this.viewPref.ifName.text = name;
                this.viewPref.ifName.MoveTextStart(false);
                return;
            }
            if (name.Contains("  ")) {
                name = name.Replace("  ", " ");
                this.viewPref.ifName.caretPosition--;
            }
            int nameLimit = LocalConst.GetLimit(LocalConst.PLAYER_NAME);
            if (name.Length > nameLimit) {
                this.viewPref.txtTips.text = string.Format(
                    LocalManager.GetValue(LocalHashConst.content_less_than), nameLimit);
            } else if (string.IsNullOrEmpty(name)) {
                this.viewPref.txtTips.text = LocalManager.GetValue(LocalHashConst.content_could_not_empty);
            } else if (!this.IsContentAvaliable(name)) {
                this.viewPref.txtTips.text = LocalManager.GetValue(LocalHashConst.content_only_num_char);
            } else {
                this.viewPref.txtTips.text = string.Empty;
                this.viewPref.ifName.text = name;
            }
        }

        private void OnEditNameEnd(string name) {
            this.HideDetail();
            if (string.IsNullOrEmpty(this.viewPref.txtTips.text) &&
                !name.CustomEquals(this.viewPref.txtName.text) && !name.CustomIsEmpty()) {
                this.viewPref.btnEditName.gameObject.SetActiveSafe(false);
                this.viewPref.btnChangeName.gameObject.SetActiveSafe(true);
                this.viewPref.ifName.text = this.viewPref.ifName.text.TrimEnd(' ');
            } else {
                this.ResetNameEdit();
            }
            this.viewPref.txtTips.text = string.Empty;
        }

        private void OnChangeNameClick() {
            UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.warning_confirm),
                string.Concat(LocalManager.GetValue(LocalHashConst.warning_change_name), " <color=#f6e46c>", this.viewPref.ifName.text, "</color> ?"),
                () => {
                    this.viewModel.EditNameReq(this.viewPref.ifName.text);
                }, this.ResetNameEdit);
        }

        private void ResetNameEdit() {
            this.viewPref.ifName.gameObject.SetActiveSafe(false);
            this.viewPref.txtName.gameObject.SetActiveSafe(true);
            this.viewPref.btnEditName.gameObject.SetActiveSafe(true);
            this.viewPref.btnChangeName.gameObject.SetActiveSafe(false);
        }

        private void OnResourcesDetailClick() {
            if (!(this.viewPref.pnlResourcesDetail.gameObject.activeSelf)) {
                this.ShowDetailBtn(viewPref.pnlResourcesDetail);
            } else {
                this.HideDetail();
            }
        }

        private void OnTributeDetailClick() {
            this.HideDetail();
            if (!(this.viewPref.pnlTributeDetail.gameObject.activeSelf)) {
                this.ShowDetailBtn(viewPref.pnlTributeDetail);
            } else {
                this.HideDetail();
            }
        }

        private void OnBasicInfoDetailClick() {
            this.HideDetail();
            if (!(this.viewPref.pnlBasicInfoDetail.gameObject.activeSelf)) {
                this.ShowDetailBtn(viewPref.pnlBasicInfoDetail);
            } else {
                this.HideDetail();
            }
        }

        private void OnBtnResInfoClick() {
            this.HideDetail();
            this.viewModel.ResourcesShow();
        }

        private void OnBtnRoleAvatarClick() {
            this.viewModel.RoleAvatarShow();
        }

        private void onListClick() {
            this.HideDetail();
        }
        private bool IsContentAvaliable(string s) {
            foreach (char c in s) {
                if (!(Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c)))
                    return false;
            }
            return true;
        }

        protected override void OnVisible() {
            this.viewPref.txtTips.text = string.Empty;
            this.viewPref.ifName.gameObject.SetActiveSafe(false);
            this.viewPref.ifDescription.gameObject.SetActiveSafe(false);
            this.viewPref.txtName.gameObject.SetActiveSafe(true);
        }

        protected override void OnInvisible() {

            base.OnInvisible();
            this.HideDetail();
        }
    }
}
