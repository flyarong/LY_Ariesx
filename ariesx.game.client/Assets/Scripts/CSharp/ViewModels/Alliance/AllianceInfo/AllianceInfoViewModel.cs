using System;
using System.Collections.Generic;
using Protocol;
using ProtoBuf;

namespace Poukoute {
    public class AllianceInfoViewModel : BaseViewModel, IViewModel {
        private AllianceViewModel parent;
        private AllianceInfoModel model;
        private BuildModel buildModel;
        private AllianceCreateOrJoinModel createOrJoinModel;
        private AllianceMembersViewModel allianceMembersViewModel;
        private AllianceInfoView view;

        public Alliance ViewAlliance {
            get {
                return this.model.viewAlliance;
            }
            set {
                this.model.viewAlliance = value;
                this.OnViewAllianceChange();
            }
        }

        private EventAbdication AllianceAbdication {
            get {
                return this.ViewAlliance.EventAbdication;
            }
        }

        public long RejoinAllianceFinishAt {
            get {
                return this.createOrJoinModel.rejoinAllianceFinishAt;
            }
        }

        private int TownhallRequireLevel {
            get {
                return AllianceModel.townhallRequireLevel;
            }
        }

        /* Other members */
        public bool NeedFresh {
            get; set;
        }


        /**********************************************/

        private void Awake() {
            this.parent = this.transform.parent.GetComponent<AllianceViewModel>();
            this.view = this.gameObject.AddComponent<AllianceInfoView>();
            this.model = ModelManager.GetModelData<AllianceInfoModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.createOrJoinModel = ModelManager.GetModelData<AllianceCreateOrJoinModel>();
        }

        public void ShowAllianceInfo(string allianceId) {
            if (!allianceId.CustomIsEmpty()) {
                this.GetAllianceInfo(allianceId);
            }
        }

        public void Hide() {
            this.view.PlayHide();
        }

        public void ShowAllianceMemOperate(PlayerPublicInfo playerInfo,
                ButtonClickWithLabel greenBtnInfo, ButtonClickWithLabel redBtnInfo) {
            this.parent.ShowAllianceMemOperate(playerInfo, greenBtnInfo, redBtnInfo);
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(null);
        }


        public bool ReachTownhallRequireLevel() {
            return this.buildModel.GetBuildLevelByName(ElementName.townhall)
                                >= this.TownhallRequireLevel;
        }

        public string GetRejoinCooldownInfo() {
            string time = GameHelper.TimeFormat(
                             this.RejoinAllianceFinishAt - RoleManager.GetCurrentUtcTime());
            return string.Format(LocalManager.GetValue(LocalHashConst.create_alliance_cooldown),
                string.Concat("<size=24><color=#FF6769FF>", time, "</color></size>"));
        }

        public void ApplyToJoinAlliance(bool allianceMemberFull = false,
                                         bool townhallLevelNotValide = false,
                                         bool isInAlliance = false,
                                         bool allianceJoinClose = false,
                                         bool rejoinCoolingDone = true,
                                         bool canJoinDirectly = false,
                                         bool reachAllianceForceLimit = false) {
            if (townhallLevelNotValide) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.no_alliance_unlock_condition),
                    TipType.Info);
                return;
            }

            if (isInAlliance) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.server_player_already_in_alliance),
                    TipType.Info);
                return;
            }

            if (canJoinDirectly) {
                this.ResetStatus();
                this.parent.ApplyJoinAlliance(string.Empty);
                this.Hide();
                return;
            }

            if (allianceJoinClose) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.alliance_refrused_join),
                    TipType.Info);
                return;
            }

            if (!rejoinCoolingDone) {
                UIManager.ShowTip(
                    this.GetRejoinCooldownInfo(),
                    TipType.Info);
                return;
            }

            if (allianceMemberFull) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.server_alliance_member_maximum),
                    TipType.Info);
                return;
            }


            bool forceLimit =
                this.ViewAlliance.JoinCondition.Type == (int)JoinConditionType.Limit;
            if (forceLimit && !reachAllianceForceLimit) {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.alliance_force_not_reach),
                    TipType.Info);
                return;
            }

            this.ResetStatus();
            this.parent.ShowSubWindowByType(AllianceSubWindowType.Apply);
        }

        public void ShowAllianceDisplayBoard(DisplayType allianceDisplayBoard) {
            this.parent.ShowAllianceDisplayBoard(allianceDisplayBoard);
        }

        #region Private callbacks
        private void ResetStatus() {
            this.NeedFresh = true;
        }

        private void OnViewAllianceChange() {
            this.view.SetAllianceInfo();
        }

        private void GetAllianceInfo(string allianceId) {
            GetAllianceReq getAllianceReq = new GetAllianceReq() {
                Id = allianceId
            };
            NetManager.SendMessage(getAllianceReq,
                                    typeof(GetAllianceAck).Name,
                                    this.GetAllianceInfoAck);
        }

        #endregion

        /* Add 'NetMessageAck' function here*/

        private void GetAllianceInfoAck(IExtensible message) {
            GetAllianceAck getAllianceAck = message as GetAllianceAck;
            this.ViewAlliance = getAllianceAck.Alliance;
            this.view.PlayShow();
        }

        public void ShowMembersList() {
            this.parent.ShowAllianceMembersList(this.ViewAlliance.Id);
        }
    }
}
