using UnityEngine;
using UnityEngine.Events;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class PlayerAllianceInfoView : BaseView {
        private PlayerAllianceInfoViewModel viewModel;
        private PlayerAllianceInfoViewPreference viewPref;
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<PlayerAllianceInfoViewModel>();
            //this.InitUi();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIPlayerAllianceInfo");
            this.viewPref = this.ui.transform.GetComponent<PlayerAllianceInfoViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnMessage.onClick.AddListener(this.OnBtnMessageSend);
        }

        public override void PlayShow() {
            base.PlayShow();
        }

        public override void PlayHide(UnityAction callback) {
            base.PlayHide(callback);
        }

        public void SetPlayerPublickInfo() {
            PlayerPublicInfo playerInfo = this.viewModel.currentPlayerInfo;
            this.viewPref.imgAvatar.sprite = RoleManager.GetRoleAvatarByKey(playerInfo.Avatar);
            this.viewPref.txtName.text = playerInfo.Name;
            bool canSendMessage = playerInfo.Name.CustomEquals(RoleManager.GetRoleName());
            this.viewPref.btnMessage.transform.gameObject.SetActiveSafe(!canSendMessage);
            string role = Enum.GetName(typeof(AllianceRole), playerInfo.AllianceRole).ToLower();

            //this.viewPref.txtInflunce.text = playerInfo.Force.ToString();
            int currentLevel = ForceRewardConf.GetForceLevel(playerInfo.Force);
            currentLevel = currentLevel == 0 ? 1 : currentLevel;
            ForceRewardConf forceRewardConf = ForceRewardConf.GetConf(currentLevel.ToString());
            this.viewPref.txtInflunce.text = forceRewardConf.forceLocal;
            //this.viewPref.imgFlunce.sprite = PoolManager.GetSprite(forceRewardConf.iconPath);

            this.SetFallenInfo(playerInfo.FallenBy);
            bool isInAlliance = !playerInfo.AllianceName.CustomIsEmpty();
            this.viewPref.imgAlliance.gameObject.SetActiveSafe(isInAlliance);
            this.viewPref.txtOfficial.text = isInAlliance ?
                LocalManager.GetValue("alliance_position_", role) : string.Empty;
            this.viewPref.txtAllianceName.StripLengthWithSuffix(isInAlliance ? playerInfo.AllianceName :
                LocalManager.GetValue(LocalHashConst.mail_battle_report_no_alliance));
            if (isInAlliance) {
                this.viewPref.imgAlliance.sprite =
                ArtPrefabConf.GetAliEmblemSprite(playerInfo.AllianceEmblem);
            }
            this.viewPref.txtDesc.text = playerInfo.Desc;
        }

        public void ShowOnlyGreenBtn(ButtonClickWithLabel greenBtnInfo) {
            this.SetBtnGreenInfo(greenBtnInfo);
            this.viewPref.btnRed.gameObject.SetActiveSafe(false);
        }

        public void ShowOnlyRedBtn(ButtonClickWithLabel redBtnInfo) {
            this.SetBtnRedInfo(redBtnInfo);
            this.viewPref.btnGreen.gameObject.SetActiveSafe(false);
        }

        public void ShowBothButton(ButtonClickWithLabel greenBtnInfo, ButtonClickWithLabel redBtnInfo) {
            this.SetBtnGreenInfo(greenBtnInfo);
            this.SetBtnRedInfo(redBtnInfo);
        }

        public void HideAllButton() {
            this.viewPref.btnRed.gameObject.SetActiveSafe(false);
            this.viewPref.btnGreen.gameObject.SetActiveSafe(false);
        }

        private void SetFallenInfo(string befallingAllianeName) {
            bool isFalling = !string.IsNullOrEmpty(befallingAllianeName);
            this.viewPref.txtFallenAlliance.gameObject.SetActiveSafe(isFalling);
            this.viewPref.pnlFallen.gameObject.SetActiveSafe(isFalling);

            if (isFalling) {
                this.viewPref.txtFallenAlliance.text =
                    string.Format(LocalManager.GetValue(LocalHashConst.fallen_title), befallingAllianeName);
            }
        }

        private void SetBtnRedInfo(ButtonClickWithLabel redBtnInfo) {
            this.viewPref.btnRed.gameObject.SetActiveSafe(true);
            this.viewPref.txtRedLabel.text = redBtnInfo.txtLabel;
            this.viewPref.btnRed.onClick.RemoveAllListeners();
            this.viewPref.btnRed.onClick.AddListener(
                () => {
                    redBtnInfo.btnClick.InvokeSafe();
                    this.viewModel.Hide();
                });
            this.viewPref.btnRed.interactable = redBtnInfo.enable;

        }
        private void SetBtnGreenInfo(ButtonClickWithLabel greenBtnInfo) {
            this.viewPref.btnGreen.gameObject.SetActiveSafe(true);
            this.viewPref.txtGreenLabel.text = greenBtnInfo.txtLabel;
            this.viewPref.btnGreen.onClick.RemoveAllListeners();
            this.viewPref.btnGreen.onClick.AddListener(
                () => {
                    greenBtnInfo.btnClick.InvokeSafe();
                    this.viewModel.Hide();
                });
            this.viewPref.btnGreen.interactable = greenBtnInfo.enable;
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        private void OnBtnMessageSend() {
            //Debug.LogError("OnBtnMessageSend callded " + this.txtName.text);
            this.viewModel.SendMessageTo(
                this.viewPref.txtName.text, this.viewModel.currentPlayerInfo.Id);
        }
    }
}
