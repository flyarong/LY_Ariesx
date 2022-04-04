using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;

namespace Poukoute {
    public class DevilFightingDetailView: BaseView {
        private DevilFightingDetailViewModel viewModel;
        private DevilFightingDetailViewPreference viewPref;


        /*************/
        void Awake() {
            this.viewModel = this.gameObject.GetComponent<DevilFightingDetailViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UICampaign.PnlCampaign.PnlDevilFightingHoldler.PnlDevilFightingDetails");
            this.viewPref = this.ui.transform.GetComponent<DevilFightingDetailViewPreference>();
        }

        public void SetContent() {
            //Debug.LogError("SetContent " + this.viewModel.AllDemonTroopConfs.Count);
            this.viewPref.txtCampaignTitle.text = string.Format(
                LocalManager.GetValue(LocalHashConst.melee_detail_rule),
                CampaignModel.MonsterLocalName
            );
            GameHelper.ResizeChildreCount(this.viewPref.pnlList,
                this.viewModel.AllDemonTroopConfs.Count, PrefabPath.pnlCampainGuideItem);
            CampainGuideItemView guideItemView = null;
            int childIndex = 0;
            string content = string.Empty;
            DemonTroopConf demonTroopConf = null;
            foreach (var demon in this.viewModel.AllDemonTroopConfs) {
                demonTroopConf = demon.Value as DemonTroopConf;
                guideItemView =
                    this.viewPref.pnlList.GetChild(childIndex++).GetComponent<CampainGuideItemView>();
                content = string.Format(
                    LocalManager.GetValue(LocalHashConst.devilfighting_wipe_out_demon), 
                    demon.Key,
                    CampaignModel.MonsterLocalName
                );
                int demonLevel = int.Parse(demon.Key);
                guideItemView.SetContent(content, demonTroopConf.point, () => {
                    this.viewModel.OnCampaignGuidItemClick(demonLevel);
                });
            }
        }

        public void SetDevilFightingOwnInfo(int ownPoints) {
            this.viewPref.stageRewardView.SetStageInfo(
                this.viewModel.ChosenActivity.Melee.IntegralReward, ownPoints,
                 showCanmpaignTips: true);

            //this.viewPref.txtCampaignTitle.text =
            //    this.viewModel.ChosenActivity.GetActivitySubject();

            this.SetDevilFightingGuidVisible(
                this.viewModel.ChosenActivity.Status != Activity.ActivityStatus.Finish);
        }

        public void HideStateRewardDetail() {
            this.viewPref.stageRewardView.HideStateRewardDetail();
        }

        private void SetDevilFightingGuidVisible(bool isVisible) {
            UIManager.SetUICanvasGroupVisible(this.viewPref.devilFightingGuidLabelCG, isVisible);
            UIManager.SetUICanvasGroupEnable(this.viewPref.devilFightingGuidContentCG, isVisible);
        }
    }
}
