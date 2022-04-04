using System.Collections.Generic;
using Protocol;
using System;
using UnityEngine.Events;

namespace Poukoute {
    public delegate void OnButtonClick();
    public class ButtonClickWithLabel {
        public string txtLabel;
        public OnButtonClick btnClick;
        // To do : Tell why disable
        public bool enable;

        public ButtonClickWithLabel(string label, OnButtonClick click, bool isEnable = true) {
            this.txtLabel = label;
            this.btnClick = click;
            this.enable = isEnable;
        }
    }

    public class PlayerAllianceInfoViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private PlayerAllianceInfoView view;
        //private AllianceCreateOrJoinModel createOrJoinModel;

        public PlayerPublicInfo currentPlayerInfo;

        void Awake() {
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.view = this.gameObject.AddComponent<PlayerAllianceInfoView>();
            //this.createOrJoinModel = ModelManager.GetModelData<AllianceCreateOrJoinModel>();
        }

        public void SendMessageToPlayer(PlayerPublicInfo playerInfo, ButtonClickWithLabel greenBtnInfo) {
            this.currentPlayerInfo = playerInfo;
            this.ShowPlayerAllianceInfo();
            this.view.ShowOnlyGreenBtn(greenBtnInfo);
        }

        public void ShowAllianceMemOperate(PlayerPublicInfo playerInfo,
                                            ButtonClickWithLabel greenBtnInfo,
                                            ButtonClickWithLabel redBtnInfo,
                                            bool isForMemberInfo) {
            this.currentPlayerInfo = playerInfo;
            this.ShowPlayerAllianceInfo();
            string allianceId = RoleManager.GetAllianceName();
            bool isAlly = allianceId.CustomEquals(playerInfo.AllianceName);
            bool isOwn = playerInfo.Id.CustomEquals(RoleManager.GetRoleId());
            bool hasRightOperate = RoleManager.GetAllianceRole() != AllianceRole.Member;
            bool notLeader = (AllianceRole)playerInfo.AllianceRole != AllianceRole.Owner;
            bool canOperateRole = isAlly && !isOwn && hasRightOperate && notLeader;
            if (canOperateRole && !isForMemberInfo) {
                this.view.ShowBothButton(greenBtnInfo, redBtnInfo);
            } else {
                this.view.HideAllButton();
            }
        }

        public void Hide(UnityAction callback = null) {
            this.view.PlayHide(callback);
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }

        protected override void OnReLogin() {
            this.Hide();
        }

        public void SendMessageTo(string userName, string userId) {
            this.view.afterHideCallback = () => this.parent.SendMessageTo(userName, userId, true);
            this.Hide();
        }


        private void ShowPlayerAllianceInfo() {
            this.view.PlayShow(() => {
                this.view.SetPlayerPublickInfo();
            });
        }
    }
}
