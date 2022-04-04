using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class DemonShadowDetailsViewModel: BaseViewModel {
        public DemonShadowViewModel parent;
        private DemonShadowDetailsView view;
        private CampaignModel model;
        public MapResouceInfo resourceInfo = new MapResouceInfo();
        /* Model data get set */
        public Activity ChosenActivity {
            get {
                return this.model.chosenActivity;
            }
        }
        /*****************************************************/


        void Awake() {
            this.model = ModelManager.GetModelData<CampaignModel>();
            this.parent = this.transform.parent.GetComponent<DemonShadowViewModel>();
            this.view = this.gameObject.AddComponent<DemonShadowDetailsView>();
        }

        public void MoveWithClick(Vector2 coordinate) {
            this.parent.MoveWithClick(coordinate);
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.Show();
            }
            this.GetDemonShadowDetailsReq();
            this.GetCampaignRewardDamination();
        }

        private void GetDemonShadowDetailsReq() {
            DominationInfoReq dominationInfoReq = new DominationInfoReq();
            NetManager.SendMessage(dominationInfoReq,
                typeof(DominationInfoAck).Name, this.GetDemonShadowDetailsAck);
        }

        private void GetDemonShadowDetailsAck(IExtensible message) {
            DominationInfoAck dominationInfoAck = message as DominationInfoAck;
            BossTroop bossTroop = dominationInfoAck.Info;
            bool hasDominaInfo = (bossTroop != null);
            this.view.SetPnlAgainstNotOpen(hasDominaInfo);
            if (hasDominaInfo) {
                this.view.SetPnlAgainstOpen(bossTroop);
            }
        }



        private void GetCampaignRewardDamination() {
            //设置魔影入侵奖励详情
            model.dominationRewardList = model.chosenActivity.Domination;
            this.view.SetCampaigRewards(model.dominationRewardList[0].LastBloodReward.Reward);
        }

        public void OnClickShowBossInfo(BossTroop dominaInfo) {
            this.parent.ShowCampaignBossInfo(dominaInfo);
        }

        public void ShowPnlAgainstOpen(BossTroop info, NPCCityConf city) {
            this.view.SetPnlAgainstOpen(info);
        }

        public void OnClickShowRule() {
            this.parent.ShowCampaignRules(
               this.ChosenActivity.GetActivityBody());
        }

        public void OnClickShowRewardDetails() {
            //显示奖励详情，弹出新弹框
            parent.ShowCampaignRewardDomination();
        }

        public void GetAllianceFallenTargets() {
            GetAllianceFallenTargetsReq fallenTargetsReq = new GetAllianceFallenTargetsReq();
            NetManager.SendMessage(fallenTargetsReq,
                                    typeof(GetAllianceFallenTargetsAck).Name,
                                    this.GetAllianceFallenTargetsAck);
        }

        int citiesCount;
        public List<NPCCityConf> allianceCities = new List<NPCCityConf>();

        private void GetAllianceFallenTargetsAck(IExtensible message) {
            GetAllianceFallenTargetsAck fallenTargetsAck =
                            message as GetAllianceFallenTargetsAck;
            citiesCount = fallenTargetsAck.Cities.Count;
            int index;
            this.allianceCities.Clear();
            for (index = 0; index < citiesCount; index++) {
                FallenTarget fallen = fallenTargetsAck.Cities[index];
                if (!this.allianceCities.Contains(this.GetCityConf(fallen)))
                    this.allianceCities.Add(this.GetCityConf(fallen));
            }
            SelectCreateDomination();
        }

        private NPCCityConf GetCityConf(FallenTarget fallenTarget) {
            string cityKey = NPCCityConf.GetCityKey(fallenTarget);
            NPCCityConf cityConf = NPCCityConf.GetConf(cityKey);
            return cityConf;
        }

        private void SelectCreateDomination() {
            if (citiesCount <= 0) {
                //没有城市 --> 弹框提示
                UIManager.ShowAlert(
                    LocalManager.GetValue(LocalHashConst.domination_nocity),
                   btnInfoLabel: LocalManager.GetValue(LocalHashConst.button_confirm));
            } else if (RoleManager.GetAllianceRole() == AllianceRole.Elder ||
                         RoleManager.GetAllianceRole() == AllianceRole.Member ||
                         RoleManager.GetAllianceRole() == AllianceRole.None) {
                //官员以下 --> 弹框提示
                UIManager.ShowAlert(
                    LocalManager.GetValue(LocalHashConst.domination_notleader),
                   btnInfoLabel: LocalManager.GetValue(LocalHashConst.button_confirm));
            } else {
                SelectCreatDominationByCanCallCity();
            }
        }

        public void OnClickAgainst() {
            GetAllianceFallenTargets();
        }

        private void SelectCreatDominationByCanCallCity() {
            GetDominationCanCallCityReq();
        }

        private void GetDominationCanCallCityReq() {
            DominationNotCallCityReq dominationCanCallCityReq = new DominationNotCallCityReq();
            dominationCanCallCityReq.AllianceId = RoleManager.GetAllianceId();
            NetManager.SendMessage(dominationCanCallCityReq,
                typeof(DominationNotCallCityAck).Name, this.GetDominationCanCallCityAck);
        }
       
        private void GetDominationCanCallCityAck(IExtensible message) {
            DominationNotCallCityAck dominationNotCallCityAck = message as DominationNotCallCityAck;
            var coords = dominationNotCallCityAck.Coords;
            //先去掉不能召唤的城市
            for (int i = this.allianceCities.Count - 1; i > 0; i--) {
                for (int k = 0; k < coords.Count; k++) {
                    Vector2 coordVec = MiniMapCityConf.GetConf(allianceCities[i].id).coordinate;
                    Coord coord = coords[k];
                    if (coordVec.x == coord.X && coordVec.y == coord.Y) {
                        allianceCities.RemoveAt(i);
                    }
                }
            }
            model.NPCCityConfList = allianceCities;
            if (allianceCities.Count == 0 || allianceCities == null) {
                //没有未开启的城市 --> 弹框提示
                UIManager.ShowAlert(
                     LocalManager.GetValue(LocalHashConst.domination_allsummoned)
                  , btnInfoLabel: LocalManager.GetValue(LocalHashConst.button_confirm));
            } else {
                //有未开启的城市
                UIManager.ShowConfirm(
                    LocalManager.GetValue(LocalHashConst.notice_title_warning),
                    LocalManager.GetValue(LocalHashConst.domination_summon_confirm),
                    () => {
                        //确定进入下一步选择城市
                        parent.ShowCampaignCitySelect();
                    },
                    () => { });
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
            }
        }

        /* Add 'NetMessageAck' function here*/

        /***********************************/
    }
}
